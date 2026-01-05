import { useRef, useState } from 'react'
import styled from '@emotion/styled'

const Container = styled.div`
  margin-top: 4.5rem;
  background-color: var(--primary-dark);
  border-radius: 9999px;
  width: 60%;
  height: 8.75rem;
  display: flex;
  justify-content: center;
  align-items: flex-start;

  & .astrix {
    display: flex;
    justify-content: center;
    text-align: center;
    gap: 2rem;
  }
`

const Astrix = styled.span`
  color: ${(p) => (p.currentLength >= p.index ? 'var(--secondary-dark)' : 'var(--bg-medium)')};
  font-size: 7.5rem;
  font-weight: bold;
  text-align: center;
`

const HiddenInput = styled.input`
  position: absolute;
  opacity: 0;
  pointer-events: none;
`

const PinInput = ({ length = 4, onComplete }) => {
  const [pin, setPin] = useState('')
  const inputRef = useRef(null)

  const handleChange = (e) => {
    const value = e.target.value.replace(/\D/g, '')
    const newPin = value.slice(0, length)
    setPin(newPin)
    if (newPin.length === length && onComplete) {
      onComplete(newPin)
      setPin('')
    }
  }

  return (
    <Container onClick={() => inputRef?.current.focus()}>
      <div className="astrix">
        {Array(length)
          .fill(0)
          .map((_, i) => (
            <Astrix currentLength={pin.length} index={i + 1} key={i}>
              *
            </Astrix>
          ))}
      </div>
      <HiddenInput
        id="hidden-pin"
        ref={inputRef}
        type="tel"
        value={pin}
        onChange={handleChange}
        autoFocus
      />
    </Container>
  )
}

export default PinInput
