import { useState } from 'react'
import styled from '@emotion/styled'

const Container = styled.div`
  background-color: var(--primary-dark);
  border-radius: 9999px;
  width: 60%;
  height: 8.75rem;
  display: flex;
  justify-content: center;
  align-items: center; /* da broj bude centriran i po vertikali */
`

const StyledInput = styled.input`
  background: transparent;
  border: none;
  outline: none;
  text-align: center;
  color: var(--bg-medium);
  font-size: 4.375rem;
  font-weight: bold;
  width: 100%;

  -moz-appearance: textfield;
  &::-webkit-outer-spin-button,
  &::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
  }
`

const NumberInput = ({ onChange }) => {
  const [number, setNumber] = useState('')

  const handleChange = (e) => {
    const value = e.target.value.replace(/\D/g, '') // samo brojevi
    setNumber(value)
    onChange?.(value)
  }

  return (
    <Container>
      <StyledInput type="number" value={number} onChange={handleChange} autoFocus />
    </Container>
  )
}

export default NumberInput
