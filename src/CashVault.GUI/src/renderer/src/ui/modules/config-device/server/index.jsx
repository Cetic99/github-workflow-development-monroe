/* eslint-disable prettier/prettier */
import FullPageLoader from '@ui/components/full-page-loader'
import TextInput from '@ui/components/inputs/text-input'
import styled from '@emotion/styled'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import { useEffect, useState } from 'react'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useTranslation } from '@domain/administration/stores'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconToggleOff from '@ui/components/icons/IconToggleOff'
import IconToggleOn from '@ui/components/icons/IconToggleOn'
import Button from '@ui/components/button'
import { useConfigurationDeviceServer } from '@domain/configuration/queries/device'
import { useSaveDeviceServerData } from '@domain/configuration/mutations/device'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  font-weight: 600;
  height: 100%;

  display: flex;
  flex-direction: column;
  justify-content: space-between;

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

  & .row {
    display: flex;
    align-items: center;
    gap: 1rem;
    margin-bottom: 2rem;
  }

  .server-config-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-bottom: 4rem;
  }
`

const ServerDeviceConfig = (props) => {
  const { t } = useTranslation()
  const [serverConfig, setServerConfig] = useState({})
  const { data, isLoading } = useConfigurationDeviceServer()
  const command = useSaveDeviceServerData()
  const navigate = useNavigate()
  useEffect(() => {
    if (data) {
      setServerConfig(data)
    }
  }, [data])

  //================================
  const handleItemChange = (name, value) => {
    setServerConfig((prev) => ({
      ...prev,
      [name]: value
    }))
  }

  const handleSubmit = () => {
    command.mutate(serverConfig)
  }

  const onGoBack = () => {
    navigate('/')
  }

  //================================
  const render = () => {
    return (
      <div className="server-config-content">
        <Button
          name="isEnabled"
          color={serverConfig?.isEnabled === true ? 'dark' : 'light'}
          className="action-button"
          icon={(props) => {
            if (serverConfig?.isEnabled) return <IconToggleOn {...props} />
            if (!serverConfig?.isEnabled) return <IconToggleOff {...props} />
          }}
          onClick={() => handleItemChange('isEnabled', !serverConfig?.isEnabled)}
        >
          {serverConfig?.isEnabled && t('Enabled')}
          {!serverConfig?.isEnabled && t('Disabled')}
        </Button>
        <TextInput
          name="url"
          label={t('Server url')}
          value={serverConfig.url ?? ''}
          onChange={(e) => handleItemChange('url', e.target.value)}
          size="m"
        />
        <TextInput
          name="deviceId"
          label={t('Device ID')}
          value={serverConfig.deviceId ?? ''}
          onChange={(e) => handleItemChange('deviceId', e.target.value)}
          size="m"
        />
        <DropdownInput
          name="minimalLogLevel"
          label={t('Min log level')}
          options={serverConfig?.logLevels}
          value={serverConfig?.minimalLogLevel + ''}
          onChange={(option) => handleItemChange('minimalLogLevel', option?.value)}
        />
      </div>
    )
  }
  return (
    <Container className="regional-device-config">
      <FullPageLoader loading={isLoading || command?.isPending} />
      {render()}
      <div className="config-main-footer">
        <CircleButton
          icon={(props) => <IconLeftHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Back')}
          onClick={onGoBack}
          shadow={true}
        />

        <CircleButton
          icon={(props) => <IconRightHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Accept')}
          onClick={(e) => handleSubmit(e)}
          shadow={true}
        />
      </div>
    </Container>
  )
}

export default ServerDeviceConfig
