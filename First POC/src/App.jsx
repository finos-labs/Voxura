import { useState, useLayoutEffect } from 'react';
import styled from 'styled-components';
import Secrets from './components/Secrets.jsx';
import SpeechToText from './components/SpeechToText.jsx';
import { SecretService } from './services/secretService.js';

const Container = styled.div`
  width: 100%;
  height: 100%;
`;

export default function App() {
  const [route, setRoute] = useState('secrets'); // secrets | speechToText

  useLayoutEffect(() => {
    setRoute(SecretService.get() === null ? 'secrets' : 'speechToText');
  }, []);

  return (
    <Container data-component="App">
      {route === 'secrets' && <Secrets />}
      {route === 'speechToText' && <SpeechToText />}
    </Container>
  );
}
