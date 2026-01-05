/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */

import { useEffect, useRef } from 'react'
import styled from '@emotion/styled'
import TextareaInput from '@ui/components/inputs/textarea-input'

const Container = styled.div`
  grid-column: 2 / 3;
  grid-row: 1 / 3;
  height: 100%;

  & .text-area {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
  }

  & .text-area-container {
    flex: 1;
    height: auto;
  }

  & .text-area-input {
    font-size: 0.8rem;
  }
`

const DeviceCommandExecutionResult = ({ result, onClear }) => {
  const textAreaRef = useRef()

  useEffect(() => {
    textAreaRef.current.scrollTop = textAreaRef.current.scrollHeight
  }, [result])

  return (
    <Container>
      <TextareaInput
        className="text-area-input"
        outerContainerClassName="text-area"
        inputContainerClassName="text-area-container"
        value={result}
        size="m"
        ref={textAreaRef}
        disabled
        onClear={onClear}
        hasClearButton={true}
      />
    </Container>
  )
}

export default DeviceCommandExecutionResult
