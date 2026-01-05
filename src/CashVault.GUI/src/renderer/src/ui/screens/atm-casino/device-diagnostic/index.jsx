/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'
import { useParams } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { useOperationMessages } from '@domain/device/stores'
import useDevices from '@domain/administration/hooks/use-devices'
import DeviceDiagnostic from '@ui/modules/maintenance/device-diagnostic'
import { isNullOrEmptyOrSpaces } from '@domain/global/services'
import { getPreciseTimestamp } from '@src/app/services/utils'
import { useGetDeviceDiagnosticCommands } from '@domain/administration/queries'
import { useRunDeviceDiagnosticCommand } from '@domain/administration/commands'
import { useGetDeviceByType } from '@domain/device/stores'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'
const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
  overflow: hidden;

  & .content {
    height: 100%;
    overflow: auto;
  }
`
const DeviceDiagnosticScreen = () => {
  const [result, setResult] = useState('')
  const [device, setDevice] = useState(null)
  const { t } = useTranslation()
  const { type } = useParams()
  const { data: devices } = useDevices()
  const { messages, dequeueLastMessage, clearMessages } = useOperationMessages()
  const { data: diagnosticCommands } = useGetDeviceDiagnosticCommands({ type })
  const { mutate: runDiagnosticCommand } = useRunDeviceDiagnosticCommand()
  const deviceFromStore = useGetDeviceByType(type)

  useEffect(() => {
    if (messages?.length > 0) {
      const message = dequeueLastMessage()
      if (!message) return

      const now = new Date()
      setResult(result + `\n > ${getPreciseTimestamp(now)} ${message.message}`)
    }
  }, [messages, dequeueLastMessage])

  useEffect(() => {
    if (devices?.length > 0) {
      const tempDevice = devices.find((device) => device.type === type)

      if (!tempDevice || device?.name === tempDevice.name) {
        return
      }

      setDevice(tempDevice)
    }
  }, [devices])

  const getName = (name) => {
    if (isNullOrEmptyOrSpaces(name)) return name

    if (type === DEVICE_TYPE.BILL_DISPENSER) return 'Bill dispenser'
    if (type === DEVICE_TYPE.TITO_PRINTER) return 'Ticket printer'
    if (type === DEVICE_TYPE.BILL_ACCEPTOR) return 'Bill acceptor'
    if (type === DEVICE_TYPE.CARD_READER) return 'Card reader'
    return t(type)
  }

  const clearResult = () => {
    setResult('')
    clearMessages()
  }

  const handleRunCommand = (command) => {
    runDiagnosticCommand({ deviceType: type, commandCode: command })
  }

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <ScreenBreadcrumbs names={[t('Diagnostics'), getName(type)]} />

        <div className="content">
          <DeviceDiagnostic
            name={device?.name ?? ''}
            onRunCommand={handleRunCommand}
            result={result}
            onClearResult={() => clearResult()}
            diagnosticCommands={diagnosticCommands || []}
            commandInProgress={deviceFromStore?.commandInProgress || false}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default DeviceDiagnosticScreen
