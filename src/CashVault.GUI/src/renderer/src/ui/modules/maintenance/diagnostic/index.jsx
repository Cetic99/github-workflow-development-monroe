/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import styled from '@emotion/styled'
import Alert from '@ui/components/alert'
import ManageDevice from '@ui/modules/maintenance/manage-device'
import Button from '@ui/components/button'
import IconRefresh from '@ui/components/icons/IconRefresh'
import { useEffect, useMemo, useState } from 'react'
import { useConnectedGetDevices } from '@domain/device/stores'
import { getDeviceByType } from '@domain/device/services'
import useDevices from '@domain/administration/hooks/use-devices'
import FullPageLoader from '@ui/components/full-page-loader'
import { useOperationMessages } from '@domain/device/stores'
import Notifications from '@ui/utility/notifications'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .back-action {
    margin-top: auto;
    padding: 0.5rem 0 0 0;
    position: sticky;
    bottom: 0;
    z-index: 10;

    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }

  & .tabs {
    padding-top: 2rem;
  }

  & .reset-all {
    display: flex;
    align-items: center;
    padding: 1rem 0;

    & button {
      margin-left: auto;
    }
  }

  & .devices {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }
`

const Diagnostic = () => {
  const { t } = useTranslation()
  const { data, onReset, onResetAll, onEnableDisable, isLoading } = useDevices()
  const connectedDevices = useConnectedGetDevices()
  const { messages, dequeueLastMessage, clearMessages } = useOperationMessages()
  const [loadingMessage, setLoadingMessage] = useState('')

  useEffect(() => {
    if (messages?.length > 0) {
      const message = dequeueLastMessage()
      if (!message) return

      if (message.isSuccess) {
        Notifications.success(message.message || 'Operation completed successfully!')
      } else {
        Notifications.error(message.message || 'Operation failed!')
      }
    }
  }, [messages, dequeueLastMessage])

  useEffect(() => {
    return () => {
      clearMessages()
    }
  }, [])

  const preparedData = useMemo(() => {
    if (connectedDevices?.length > 0) {
      return (data || []).map((device) => {
        if (device?.type) {
          const deviceData = getDeviceByType(connectedDevices, device?.type)
          return {
            ...device,
            error: deviceData?.errorMessage || '',
            warning: deviceData?.warningMessage || ''
          }
        }
        return device
      })
    }
    return []
  }, [data, connectedDevices])

  //===========================================================================

  const handleResetAll = () => {
    setLoadingMessage('Resetting all devices...')
    onResetAll()
  }

  const handleReset = (deviceType) => {
    setLoadingMessage(`Resetting ${deviceType}...`)
    onReset(deviceType)
  }

  const handleEnableDisable = (isEnabled, deviceType) => {
    setLoadingMessage(`${isEnabled ? 'Disabling' : 'Enabling'} ${deviceType}...`)
    onEnableDisable(isEnabled, deviceType)
  }

  return (
    <Container>
      <FullPageLoader loading={isLoading} message={loadingMessage} />
      <div className="maintenance-header">
        <div className="reset-all">
          <Button
            disabled={preparedData?.length === 0}
            icon={(props) => <IconRefresh {...props} />}
            onClick={() => handleResetAll()}
          >
            {t('Reset all')}
          </Button>
        </div>
      </div>

      <>
        {preparedData?.length === 0 && <Alert severity="warning" text="No devices available..." />}
        <div className="devices">
          {preparedData?.map((x, i) => (
            <ManageDevice
              key={`${x?.name}__${i}`}
              name={x?.type}
              type={x?.type}
              status={x?.status}
              enabled={x?.isEnabled}
              connected={x?.isConnected}
              active={x?.isActive}
              desc={x?.name}
              detailedInfo={x?.status}
              error={x?.errorMessage}
              warning={x?.warningMessage}
              onEnableDisable={handleEnableDisable}
              onReset={handleReset}
            />
          ))}
        </div>
      </>
    </Container>
  )
}

export default Diagnostic
