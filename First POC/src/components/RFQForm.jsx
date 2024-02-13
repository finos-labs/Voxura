import { useState, useEffect, useRef } from 'react';
import styled from 'styled-components';

const Container = styled.div`
    display: flex;
    flex-direction: column;
`;

const InputContainer = styled.div`
    display: flex;
    align-items: center;
    transition: 1000ms;
    background-color: ${({ $changed }) => $changed ? 'yellow' : 'transparent'};
    margin-bottom: 0.5rem;
    padding: 0.75rem;
    width: 70%;
    border: 1px solid rgba(0, 0, 0, 0.1);
    border-radius: 0.25rem;
`;

const Label = styled.div`
    width: 35%;
`;

const InputText = styled.input`
    padding: 0.5rem;
    border: 1px solid rgba(0, 0, 0, 0.5);
    border-radius: 0.25rem;
    width: 60%;
`;

const InputNumber = styled.input`
    padding: 0.5rem;
    border: 1px solid rgba(0, 0, 0, 0.5);
    border-radius: 0.25rem;
    width: 60%;
`;

const InputDate = styled.input`
    padding: 0.5rem;
    border: 1px solid rgba(0, 0, 0, 0.5);
    border-radius: 0.25rem;
    width: 60%;
`;

const Select = styled.select`
    padding: 0.5rem;
    border: 1px solid rgba(0, 0, 0, 0.5);
    border-radius: 0.25rem;
    width: 40%;
`;

const Textarea = styled.textarea`
    padding: 0.5rem;
    border: 1px solid rgba(0, 0, 0, 0.5);
    border-radius: 0.25rem;
    width: 60%;
    height: 100px;
`;

export default function RFQForm({ json }) {
    const [contactEmail, setContactEmail] = useState('');
    const [contactName, setContactName] = useState('');
    const [organizationPermid, setOrganizationPermid] = useState('');
    const [organizationName, setOrganizationName] = useState('');
    const [direction, setDirection] = useState('');
    const [notional, setNotional] = useState(0);
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [rollConvention, setRollConvention] = useState('');
    const [product, setProduct] = useState('');
    const [notes, setNotes] = useState('');

    const [contactEmailChanged, setContactEmailChanged] = useState(false);
    const [contactNameChanged, setContactNameChanged] = useState(false);
    const [organizationPermidChanged, setOrganizationPermidChanged] = useState(false);
    const [organizationNameChanged, setOrganizationNameChanged] = useState(false);
    const [directionChanged, setDirectionChanged] = useState(false);
    const [notionalChanged, setNotionalChanged] = useState(false);
    const [startDateChanged, setStartDateChanged] = useState(false);
    const [endDateChanged, setEndDateChanged] = useState(false);
    const [rollConventionChanged, setRollConventionChanged] = useState(false);
    const [productChanged, setProductChanged] = useState(false);
    const [notesChanged, setNotesChanged] = useState(false);

    const animateBackground = (which, fn) => {
        if (which) {
            fn(true);
            setTimeout(() => {
                fn(false);
            }, 1000);
        }
    };

    useEffect(() => {
        try {
            const RFQObject = JSON.parse(json);

            setContactEmail(RFQObject?.Requestor?.Id?.email || '');
            setContactName(RFQObject?.Requestor?.Name || '');
            setOrganizationPermid(RFQObject?.Requestor?.Id?.PERMID || '');
            setOrganizationName(RFQObject?.Requestor?.Name || '');
            setDirection(RFQObject?.Direction || '');
            setNotional(RFQObject?.Notional || 0);
            setStartDate(RFQObject?.StartDate || '');
            setEndDate(RFQObject?.EndDate || '');
            setRollConvention(RFQObject?.RollConvention || '');
            setProduct(RFQObject?.Trade?.Product || '');
            setNotes(RFQObject?.Notes || '');
        } catch (e) {

        }
    }, [json]);

    useEffect(() => {
        animateBackground(contactEmail, setContactEmailChanged);
    }, [contactEmail]);

    useEffect(() => {
        animateBackground(contactName, setContactNameChanged);
    }, [contactName]);

    useEffect(() => {
        animateBackground(organizationPermid, setOrganizationPermidChanged);
    }, [organizationPermid]);

    useEffect(() => {
        animateBackground(organizationName, setOrganizationNameChanged);
    }, [organizationName]);

    useEffect(() => {
        animateBackground(direction, setDirectionChanged);
    }, [direction]);

    useEffect(() => {
        animateBackground(notional, setNotionalChanged);
    }, [notional]);

    useEffect(() => {
        animateBackground(startDate, setStartDateChanged);
    }, [startDate]);

    useEffect(() => {
        animateBackground(endDate, setEndDateChanged);
    }, [endDate]);

    useEffect(() => {
        animateBackground(rollConvention, setRollConventionChanged);
    }, [rollConvention]);

    useEffect(() => {
        animateBackground(product, setProductChanged);
    }, [product]);

    useEffect(() => {
        animateBackground(notes, setNotesChanged);
    }, [notes]);

    return (
        <Container data-component="RFQForm">

            <>
                <InputContainer $changed={contactEmailChanged}>
                    <Label>Email:</Label>
                    <InputText value={contactEmail} onChange={() => { }} placeholder="Email" />
                </InputContainer>
                <InputContainer $changed={contactNameChanged}>
                    <Label>Name:</Label>
                    <InputText value={contactName} onChange={() => { }} placeholder="Name" />
                </InputContainer>
            </>

            {/* {organizationPermid && (
                <>
                    <InputContainer $changed={organizationPermidChanged}>
                        <Label>PERMID:</Label>
                        <InputText value={organizationPermid} onChange={() => { }} placeholder="PERMID" />
                    </InputContainer>
                    <InputContainer $changed={organizationNameChanged}>
                        <Label>Name:</Label>
                        <InputText value={organizationName} onChange={() => { }} placeholder="Name" />
                    </InputContainer>
                </>
            )} */}

            <InputContainer $changed={directionChanged}>
                <Label>Direction:</Label>
                <Select value={direction} onChange={() => { }}>
                    <option value="">-- choose --</option>
                    <option value="BUY">BUY</option>
                    <option value="SELL">SELL</option>
                </Select>
            </InputContainer>

            <InputContainer $changed={notionalChanged}>
                <Label>Notional</Label>
                <InputNumber type="number" value={notional} onChange={() => { }} />
            </InputContainer>

            <InputContainer $changed={startDateChanged}>
                <Label>Start Date:</Label>
                <InputDate type="date" value={startDate} onChange={() => { }} />
            </InputContainer>
            <InputContainer $changed={endDateChanged}>
                <Label>End Date:</Label>
                <InputDate type="date" value={endDate} onChange={() => { }} />
            </InputContainer>

            <InputContainer $changed={rollConventionChanged}>
                <Label>Roll Convention:</Label>
                <Select value={rollConvention} onChange={() => { }}>
                    <option value="">-- choose --</option>
                    <option value="Following">Following</option>
                    <option value="Modified Following">Modified Following</option>
                    <option value="Preceding">Preceding</option>
                </Select>
            </InputContainer>

            <InputContainer $changed={productChanged}>
                <Label>Product:</Label>
                <InputText value={product} onChange={() => { }} placeholder="Product" />
            </InputContainer>

            <InputContainer $changed={notesChanged}>
                <Label>Notes:</Label>
                <Textarea value={notes} onChange={() => { }} placeholder="Notes" />
            </InputContainer>
        </Container>
    );
}