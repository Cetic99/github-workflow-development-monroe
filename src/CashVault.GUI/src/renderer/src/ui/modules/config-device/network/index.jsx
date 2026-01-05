/* eslint-disable prettier/prettier */
import FullPageLoader from '@ui/components/full-page-loader'
import styled from '@emotion/styled'
import CircleButton from '@ui/components/circle-button'
import { useTranslation } from '@domain/administration/stores'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useConfigurationDeviceNetwork } from '@domain/configuration/queries/device'
import { useSaveDeviceNetworkData } from '@domain/configuration/mutations/device'
import NetworkAdapter from './network-adapter'
import Alert from '@ui/components/alert'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  font-weight: 600;
  height: 100%;

  display: flex;
  flex-direction: column;
  justify-content: space-between;

  .content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    height: 100%;
  }

  & .config-main-footer {
    padding-top: 3rem;
    margin-top: auto;
    display: flex;
    gap: 4rem;
    justify-content: space-between;
    position: sticky;
    bottom: 0;
    z-index: 10;

    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }

  & .container {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }
`

const NetworkDeviceConfig = (props) => {
  const { t } = useTranslation()
  const { data, isLoading } = useConfigurationDeviceNetwork()
  const command = useSaveDeviceNetworkData()
  const hasAdminPrivilages = data?.[0]?.hasAdminPrivilages === false
  const navigate = useNavigate()

  const onGoBack = () => {
    navigate('/')
  }

  const onSaveAdapter = (data) => {
    command.mutate(data)
  }

  const render = (disabled) => {
    return (
      <div className="container">
        {data
          ?.sort((a, b) => (a.status === 'Up' ? -1 : 1))
          .map((adapter, index) => (
            <NetworkAdapter
              disabled={disabled}
              onSave={onSaveAdapter}
              key={index}
              adapter={adapter}
            />
          ))}
      </div>
    )
  }

  return (
    <Container className="regional-device-config">
      <FullPageLoader loading={command?.isPending || isLoading} />
      <div className="content">
        <div className="warnings">
          {hasAdminPrivilages === true && (
            <Alert
              severity="warning"
              text={t(
                'This device does not have administrative privileges. Some features may be restricted.'
              )}
            />
          )}
        </div>
        {render(isLoading || command?.isPending)}
      </div>

      <div className="config-main-footer">
        <CircleButton
          icon={(props) => <IconLeftHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Back')}
          onClick={onGoBack}
          shadow={true}
        />
      </div>
    </Container>
  )
}

export default NetworkDeviceConfig
