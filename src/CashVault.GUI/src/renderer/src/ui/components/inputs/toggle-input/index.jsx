import { useState } from 'react'
import styled from '@emotion/styled'

const ToggleContainer = styled.div`
  display: flex;
  align-items: center;
  cursor: pointer;
`

const ToggleSwitch = styled.div`
  width: 3.125rem;
  height: 1.563rem;
  background: ${(props) => (props.on ? 'var(--secondary-dark)' : 'var(--text-medium)')};
  border-radius: 1.563rem;
  position: relative;
  transition: background 0.2s;

  &:before {
    content: '';
    position: absolute;
    top: 0.125rem;
    left: ${(props) => (props.on ? '1.625rem' : '0.125rem')};
    width: 1.313rem;
    height: 1.313rem;
    background: white;
    border-radius: 50%;
    transition: left 0.2s;
  }
`

const Label = styled.span`
  margin-left: 0.5rem;
  font-weight: 500;
  color: #020605;
`

const ToggleInput = ({ label = '', onChange = () => {} }) => {
  const [isOn, setIsOn] = useState(false)

  const handleToggle = () => {
    setIsOn((prev) => !prev)
    onChange && onChange(!isOn)
  }

  return (
    <ToggleContainer onClick={handleToggle}>
      <ToggleSwitch on={isOn} />
      {label && <Label>{label}</Label>}
    </ToggleContainer>
  )
}

export default ToggleInput
