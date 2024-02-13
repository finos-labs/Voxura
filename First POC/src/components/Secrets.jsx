import { useState } from 'react';
import styled from 'styled-components';
import { SecretService } from '../services/secretService.js';

const Container = styled.div`
    display: flex;
    padding: 1rem;
    border: 1px solid gray;
    border-radius: 0.25rem;
    flex-direction: column;
    width: 66%;
    align-items: center;
`;

const Title = styled.h1`
    letter-spacing: 2px;
`;

const ProviderSelectorContainer = styled.div`
    display: flex;
    gap: 2rem;
`;

const ProviderSelector = styled.div`
    display: flex;
    gap: 0.5rem;
`;

const Label = styled.label`
    display: flex;
    align-items: center;
    flex-direction: column;
    justify-content: space-between;
    width: 100%;
    margin-top: 1rem;
`;

const Input = styled.input`
    border-radius: 0.25rem;
    width: 50%;
    border: 2px solid gray;
    padding: 1rem 0;
`;

const Button = styled.button`
    padding: 1rem 0;
    border-radius: 0.25rem;
    border: 2px solid gray;
    width: 50%;
    margin-top: 2rem;
    outline: none;
    cursor: pointer;
    background-color: white;
    font-size: 1rem;
`;

export default function Secrets() {
    const [provider, setProvider] = useState('azure'); // openai | azure
    const [apiKey, setApiKey] = useState('');
    const [organization, setOrganization] = useState('');

    const handleSavingSecrets = () => {
        SecretService.set({
            apiKey: apiKey.trim(),
            organization: organization.trim(),
            provider,
        });
        location.reload();
    };

    return (
        <Container data-component="Secrets">
            <Title>SECRETS</Title>

            <ProviderSelectorContainer>
                <ProviderSelector>
                    OpenAI <input type='radio' checked={provider === 'openai'} onChange={() => setProvider('openai')} />
                </ProviderSelector>
                <ProviderSelector>
                    Azure OpenAI <input type='radio' checked={provider === 'azure'} onChange={() => setProvider('azure')} />
                </ProviderSelector>
            </ProviderSelectorContainer>

            {provider == 'openai' &&
                <div>
                    <Label>
                        <h2>OpenAI Api Key</h2>
                        <Input value={apiKey} onChange={(e) => setApiKey(e.target.value)} />
                    </Label>

                    <Label>
                        <h2>Organization</h2>
                        <Input value={organization} onChange={(e) => setOrganization(e.target.value)} />
                    </Label>
                </div>
            }
            {provider == 'azure' &&
                <div>
                    <Label>
                        <h2>Azure Api Key</h2>
                        <Input value={apiKey} onChange={(e) => setApiKey(e.target.value)} />
                    </Label>
                </div>
            }
            <Button onClick={() => handleSavingSecrets()}>Save secrets</Button>
        </Container>
    );
}
