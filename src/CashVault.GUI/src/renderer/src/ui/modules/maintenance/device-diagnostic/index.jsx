/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import DeviceCommandExecutionResult from '@ui/components/device-command-execution-result'
import styled from '@emotion/styled'
import Button from '@ui/components/button'
import DeviceImage from '@ui/components/images/device-image'
import BarLoader from '@ui/components/bar-loader'

const Container = styled.div`
  display: grid;
  grid-template-columns: 1fr 2fr;
  grid-template-rows: 1fr 2fr;
  gap: 1rem;
  height: 100%;

  & .device-image {
    width: 20rem;
    height: 10rem;
    object-fit: contain;
  }

  & .button-container {
    display: flex;
    justify-content: flex-start;
    flex-direction: column;
    gap: 1rem;
    margin-bottom: 0.5rem;
    grid-column: 1 / 2;
    grid-row: 2 / 3;
    overflow: auto;

    & .spanned-button {
      width: 100%;
      text-align: center;
      justify-content: center;
    }
  }

  & .device-image-container {
    padding: 0.5rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    grid-column: 1 / 2;
    grid-row: 1 / 2;
    margin-bottom: 0.5rem;
    width: 100%;
    height: 13rem;
    border-radius: 10px;
    background-color: var(--bg-dark);
  }
`

const DeviceDiagnostic = ({
  name,
  result,
  onClearResult,
  diagnosticCommands,
  onRunCommand,
  commandInProgress
}) => {
  return (
    <Container>
      <div className="device-image-container">
        {name && <DeviceImage name={name} className="device-image" alt={name} />}
      </div>
      <DeviceCommandExecutionResult result={result} onClear={onClearResult} />
      <div className="button-container">
      <BarLoader loading={commandInProgress} />
        {(Array.isArray(diagnosticCommands) ? diagnosticCommands : []).map((command) => (
          <Button
            key={command.code}
            className="spanned-button"
            onClick={() => onRunCommand(command.code)}
          >
            {command.name}
          </Button>
        ))}
      </div>
    </Container>
  )
}

export default DeviceDiagnostic
