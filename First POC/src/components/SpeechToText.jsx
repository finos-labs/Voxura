import { useState, useEffect } from 'react';
import styled, { keyframes } from 'styled-components';
import OpenAI from 'openai';
import { SecretService } from '../services/secretService.js';
import { getId } from '../services/idService.js';
import { getFunctionDescriptions, prepareChatMessagesForSending } from '../utils/utils.js';
import RFQForm from './RFQForm.jsx';
import hexaLogo from '../images/hexa-logo.png';

let recognition = null;

let openai = null;

const languages = ['hu', 'en'];

const defaultSystemPrompt = `
    Below is a raw transcript of a user's verbal instructions to fill a form. Convert it to a JSON object that conforms the RFQ TypeScript interface below. Ignore anything else.
    Answer only with the required object and nothing else!

    interface Contact {
        Id: {
            email: string as Email;
        };
        Name?: string;
    }

    interface Organization {
        Id: {
            PERMID: string;
        };
        Name?: string;
    }

    interface RFQ {
        Requestor: Contact | Organization;
        Direction: 'BUY' | 'SELL';
        Notional: number as Int;
        StartDate: date;
        EndDate: date;
        RollConvention: 'Following' | 'Modified Following' | 'Preceding';
        Trade: {
            Product: string;  // The product or currency the user wants to buy or sell
        };
        Notes: string;  // Any other information not captured by the above fields
    }
`;

const Container = styled.div`
    width: 100%;
    height: 100%;
`;

const Title = styled.h2`
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 0.5rem;
    margin: 1rem 0 0 0;
`;

const LogoContainer = styled.a`
    display: flex;
    align-items: center;
    cursor: pointer;
    text-decoration: none;
`;

const HexaLogo = styled.img`
    width: 2.5rem;
    aspect-ratio: 1 / 1;
    transition: 400ms;

    &:hover {
        transform: scale(1.1);
    }
`;

const MainContent = styled.div`
    display: flex;
`;

const LeftContainer = styled.div`
    width: 55%;
    padding: 0 1rem;
`;

const ControlsContainer = styled.div`
    display: flex;
    gap: 0.5rem;
    align-items: center;
    padding: 1rem 0;
    margin-bottom: 1rem;
`;

const Select = styled.select`
    padding: 0.5rem;
    border: 1px solid rgba(0, 0, 0, 0.5);
    border-radius: 0.25rem;
`;

const Button = styled.button`
    margin: 0;
    padding: 0.5rem 1rem;
    background-color: transparent;
    border: 1px solid rgba(0, 0, 0, 0.5);
    border-radius: 0.25rem;
    outline: none;
    cursor: pointer;
    letter-spacing: 0.125rem;
`;

const rotateAnimation = keyframes`
    from {
        transform: rotate(0deg);
    }
    to {
        transform: rotate(360deg);
    }
`;

const LoadingIndicator = styled.div`
    i {
        font-size: 1rem;
        animation: ${rotateAnimation} 2s linear infinite;
    }
`;

const RightContainer = styled.div`
    width: 40%;
    padding: 1rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
`;

const SystemPromptBox = styled.textarea`
    width: 100%;
    height: 200px;
`;

const PhaseContainer = styled.div`
    display: flex;
    gap: 0.5rem;
    justify-content: center;
`;

const Phase = styled.div`
    display: flex;
    align-items: center;
`;

const Icon = styled.i`
    font-size: 1rem;
`;

const InterimTranscript = styled.div`
    font-style: italic;
`;

const Transcript = styled.div`

`;

export default function SpeechToText() {
    const [systemPrompt, setSystemPrompt] = useState(defaultSystemPrompt);
    const [isContinuous, setIsContinuous] = useState(true);
    const [interimResultsIncluded, setInterimResultsIncluded] = useState(true);
    const [language, setLanguage] = useState('en');
    const [phase, setPhase] = useState('');
    const [interimText, setInterimText] = useState('');
    const [text, setText] = useState('');
    const [textBuffer, setTextBuffer] = useState('');
    const [chatMessages, setChatMessages] = useState([]);
    const [lastAnswer, setLastAnswer] = useState('');
    const [model, setModel] = useState('gpt-3.5-turbo');
    const [temperature, setTemperature] = useState(0);
    const [isWaitingForResponse, setIsWaitingForResponse] = useState(false);
    const [isDebugPanelOpen, setIsDebugPanelOpen] = useState(false);

    const onRecognitionStart = () => setPhase(() => 'start');
    const onRecognitionAudioStart = () => setPhase(() => 'audiostart');
    const onRecognitionSoundStart = () => setPhase(() => 'soundstart');
    const onRecognitionSpeechStart = () => setPhase(() => 'speechstart');
    const onRecognitionNoMatch = () => setPhase(() => 'nomatch');
    const onRecognitionResult = (e) => {
        setPhase(() => 'result');
        let transcript = '';
        Array.from(e.results).forEach((result) => {
            if (result.isFinal) {
                setInterimText('');
                transcript = `${transcript} ${result[0].transcript}`;

            } else {
                setInterimText(result[0].transcript);
                if (interimResultsIncluded) {
                    transcript = `${transcript} ${result[0].transcript}`;
                }
            }
        });
        if (!isWaitingForResponse) {
            setText(transcript);
        } else {
            setTextBuffer(transcript);
        }
    };
    const onRecognitionSpeechEnd = () => setPhase(() => 'speechend');
    const onRecognitionSoundEnd = () => setPhase(() => 'soundend');
    const onRecognitionAudioEnd = () => setPhase(() => 'audioend');
    const onRecognitionEnd = () => setPhase(() => 'end');

    const recognitionEvents = {
        start: onRecognitionStart,
        audiostart: onRecognitionAudioStart,
        soundstart: onRecognitionSoundStart,
        speechstart: onRecognitionSpeechStart,
        nomatch: onRecognitionNoMatch,
        result: onRecognitionResult,
        speechend: onRecognitionSpeechEnd,
        soundend: onRecognitionSoundEnd,
        audioend: onRecognitionAudioEnd,
        end: onRecognitionEnd,
    };

    const createRecognition = () => {
        recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
    };

    const destroyRecognition = () => {
        recognition = null;
    };

    const addRecognitionEventListeners = () => {
        Object.entries(recognitionEvents).forEach(([eventName, fn]) => recognition.addEventListener(eventName, fn));
    };

    const removeRecognitionEventListeners = () => {
        Object.entries(recognitionEvents).forEach(([eventName, fn]) => recognition.removeEventListener(eventName, fn));
    };

    const setRecognitionSettings = () => {
        recognition.continuous = isContinuous;
        recognition.lang = language;
        recognition.interimResults = true;
    };

    const startRecognition = () => {
        recognition.start();
    };

    const abortRecognition = () => {
        recognition.abort();
        setText('');
    };

    const stopRecognition = () => {
        recognition.stop();
        setText('');
    };

    const handleClearingSecrets = () => {
        SecretService.set(null);
        location.reload();
    };

    const setProductNameFilter = ({ productName }) => {
        return `Selected this product name: ${productName}`;
    };

    const setBackgroundColor = ({ red, green, blue }) => {
        return `The background has been set to ${red}, ${green}, ${blue}`;
    };

    const setDayOrNightMode = ({ mode }) => {
        return `${mode} mode has been set`;
    };

    const availableFunctions = {
        setProductNameFilter,
        setBackgroundColor,
        setDayOrNightMode
    };

    const handleSend = () => {
        const messageToSend = {
            id: getId(),
            role: 'user',
            content: text.trim(),
        };

        setChatMessages([...chatMessages, messageToSend]);
    };

    useEffect(() => {
        const { apiKey, organization, provider } = SecretService.get();

        if (provider === 'openai') {
            openai = new OpenAI({
                apiKey,
                organization,
                dangerouslyAllowBrowser: true,
            });

        } else if (provider === 'azure') {
            setModel("gpt35");
            openai = new OpenAI({
                apiKey,
                baseURL: `https://hexaio-ai-switznorth.openai.azure.com/openai/deployments/gpt35`,
                defaultQuery: { 'api-version': '2023-07-01-preview' },
                defaultHeaders: { 'api-key': apiKey },
                dangerouslyAllowBrowser: true,
            });
        }

        return () => {
            openai = null;
        };
    }, []);

    useEffect(() => {
        createRecognition();

        return () => {
            destroyRecognition();
        };
    }, []);

    useEffect(() => {
        addRecognitionEventListeners();

        return () => {
            removeRecognitionEventListeners();
        };
    }, [isWaitingForResponse, interimResultsIncluded]);

    useEffect(() => {
        abortRecognition();
        setRecognitionSettings();
    }, [isContinuous, language]);

    useEffect(() => {
        if (!isWaitingForResponse && textBuffer) {
            if (textBuffer.length > text.length) {
                setText(textBuffer);
            }
            setTextBuffer('');
        }
    }, [isWaitingForResponse, textBuffer]);

    useEffect(() => {
        const newChatMessages = structuredClone(chatMessages);
        newChatMessages[0] = {
            id: 1,
            role: 'system',
            content: systemPrompt
        }
        setChatMessages(newChatMessages);
    }, [systemPrompt]);

    useEffect(() => {
        if (text) {
            handleSend();
        }
    }, [text]);

    useEffect(() => {
        const lastMessage = chatMessages[chatMessages.length - 1];
        console.log(chatMessages);

        if (
            lastMessage &&
            (lastMessage.role === 'user' || lastMessage.role === 'function')
        ) {
            const sendRequest = async () => {
                let errorCode = '';

                setIsWaitingForResponse(true);
                const completion = await openai.chat.completions.create({
                    messages: prepareChatMessagesForSending(chatMessages),
                    model,
                    temperature,
                    top_p: 0.95,
                    // functions: getFunctionDescriptions(),
                    // function_call: 'auto',
                }).catch((e) => {
                    if (e instanceof OpenAI.APIError) {
                        console.log(e.error)
                        // e.error.code "context_length_exceeded", "content_filter"
                        errorCode = e?.error?.code;
                        if (errorCode === 'content_filter') {
                            const contentFilterResult = e?.error?.innererror?.content_filter_result;
                            let alertText = 'This request could not be served because of';
                            console.log(contentFilterResult);
                            Object.entries(contentFilterResult).forEach(([reason, details]) => {
                                if (details.filtered) {
                                    alertText += ` ${details.severity} possibility of ${reason}`
                                }
                            });
                            alertText += '!';
                            console.log(alertText);
                            // alert(alertText);
                        }

                    } else {
                        throw e;
                    }

                });
                setIsWaitingForResponse(false);

                if (completion) {
                    const message = completion.choices[0].message;

                    const assistantAnswer = {
                        id: getId(),
                        ...message,
                    };

                    if (!message.function_call) {
                        setChatMessages([...chatMessages, assistantAnswer]);
                    } else {
                        const functionName = message.function_call.name;
                        const functionToCall = availableFunctions[functionName];
                        const functionArgs = JSON.parse(message.function_call.arguments);
                        const functionResponse = functionToCall(functionArgs);

                        const functionAnswer = {
                            id: getId(),
                            role: 'function',
                            name: functionName,
                            content: functionResponse,
                        };

                        setChatMessages([...chatMessages, assistantAnswer, functionAnswer]);
                    }
                } else {
                    // primitive v0.1 error handling
                    if (errorCode === 'content_filter') {
                        abortRecognition();
                        setTimeout(() => {
                            startRecognition();
                        }, 1000);
                        setChatMessages([{
                            id: 1,
                            role: 'system',
                            content: systemPrompt
                        }]);
                    } else if (errorCode === 'context_length_exceeded') {
                        setChatMessages([{
                            id: 1,
                            role: 'system',
                            content: systemPrompt
                        }]);
                    }

                }
            };

            sendRequest();
        }
    }, [chatMessages]);

    useEffect(() => {
        setLastAnswer(chatMessages.findLast((item) => item.role === 'assistant')?.content);
    }, [chatMessages]);

    return (
        <Container data-component="SpeechToText">

            <Title>
                <LogoContainer href="https://hexaio.com/" target="_blank" rel="noopener">
                    <HexaLogo src={hexaLogo} alt="hexa logo" />
                </LogoContainer>
                Speech To Form Demo
            </Title>

            <MainContent>
                <LeftContainer>
                    <ControlsContainer>
                        language:
                        <Select value={language} onChange={(e) => setLanguage(e.target.value)}>
                            {languages.map((item) => {
                                return <option key={item} value={item}>{item}</option>;
                            })}
                        </Select>

                        <Button onClick={() => startRecognition()} disabled={isWaitingForResponse || !(phase === '' || phase === 'end')}>start</Button>
                        <Button onClick={() => abortRecognition()} disabled={phase === '' || phase === 'end'}>abort</Button>
                        <Button onClick={() => stopRecognition()} disabled={phase === '' || phase === 'end'}>stop</Button>
                        <Button onClick={() => setIsDebugPanelOpen(!isDebugPanelOpen)}>debug</Button>
                        {isWaitingForResponse &&
                            <LoadingIndicator>
                                <i className="material-icons">loop</i>
                            </LoadingIndicator>
                        }
                    </ControlsContainer>

                    <RFQForm json={lastAnswer} />
                </LeftContainer>

                {isDebugPanelOpen &&
                    <RightContainer>
                        <div>
                            <span onClick={() => handleClearingSecrets()}>CLEAR SECRETS</span>
                            {' ------- '}continuous recognition: <input type="checkbox" checked={isContinuous} onChange={() => setIsContinuous(!isContinuous)} />
                            {' ------- '}interim results included: <input type="checkbox" checked={interimResultsIncluded} onChange={() => setInterimResultsIncluded(!interimResultsIncluded)} />
                        </div>

                        <SystemPromptBox value={systemPrompt} onChange={(e) => setSystemPrompt(e.target.value)} placeholder="system prompt" />

                        <PhaseContainer>
                            <Phase>start {phase === 'start' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>audiostart {phase === 'audiostart' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>soundstart {phase === 'soundstart' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>speechstart {phase === 'speechstart' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>nomatch {phase === 'nomatch' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>result {phase === 'result' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>speechend {phase === 'speechend' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>soundend {phase === 'soundend' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>audioend {phase === 'audioend' && <Icon className="material-icons">check</Icon>}</Phase>
                            <Phase>end {phase === 'end' && <Icon className="material-icons">check</Icon>}</Phase>
                        </PhaseContainer>

                        <div>
                            <InterimTranscript>INTERIM: {interimText}</InterimTranscript>

                            <Transcript>USER: {textBuffer ? textBuffer : text}</Transcript>

                            ASSISTANT: <pre>{lastAnswer}</pre>
                        </div>
                    </RightContainer>
                }
            </MainContent>
        </Container>
    );
}