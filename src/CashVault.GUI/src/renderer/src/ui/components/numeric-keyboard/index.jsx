/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
import Numpad from '@ui/components/numpad'
import { useState } from 'react'
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2.5rem;
  width: fit-content;

  & .value {
    display: flex;
    justify-content: center;
    gap: 1rem;
    align-items: center;
    border-radius: 0.625rem;
    min-height: 6.25rem;
    background-color: var(--bg-dark);
    font-family: Poppins;
    font-weight: 700;
    font-style: Bold;
    font-size: 2.75rem;
    line-height: 2.75rem;
    text-align: center;
    color: black;

    & .prefix {
      color: var(--text-medium);
    }
  }
`

const OuterContainer = styled.div`
  padding: 2rem 0 0 0;
  display: flex;
  justify-content: center;
  align-items: center;
`

const NumericKeyboard = ({ maxLength = 16, onChange = () => {}, defaultValue, prefix }) => {
  const [value, setValue] = useState(defaultValue ?? '')

  // TODO: Check double tap on number input (it maximizes screen)

  const changeValue = (val) => {
    setValue(val)
    onChange(val)
  }

  const onClear = () => {
    changeValue('')
  }

  const onDelete = () => {
    changeValue(value.slice(0, -1))
  }

  const onNumber = (num) => {
    if (value?.length == maxLength) return

    changeValue(value + '' + num)
  }

  return (
    <OuterContainer>
      <Container>
        <div className="value">
          {prefix ? <span className="prefix">+{prefix}</span> : null}
          <span>{value}</span>
        </div>
        <Numpad onNumber={onNumber} onClear={onClear} onDelete={onDelete} />
      </Container>
    </OuterContainer>
  )
}

export default NumericKeyboard
