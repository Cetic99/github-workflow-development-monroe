/* eslint-disable prettier/prettier */
import FullPageLoader from '@ui/components/full-page-loader'
import TextInput from '@ui/components/inputs/text-input'
import styled from '@emotion/styled'
import { useEffect, useState } from 'react'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useTranslation } from '@domain/administration/stores'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useConfigurationDeviceOnlineConfig } from '@domain/configuration/queries/device'
import { useSaveDeviceOnlineConfigData } from '@domain/configuration/mutations/device'
import IconToggleOff from '@ui/components/icons/IconToggleOff'
import IconToggleOn from '@ui/components/icons/IconToggleOn'
import Button from '@ui/components/button'
import DecimalInput from '@ui/components/inputs/decimal-input'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  font-weight: 600;
  height: 100%;

  display: flex;
  flex-direction: column;
  justify-content: space-between;

  .online-config-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-bottom: 4rem;
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
`

const OnlineDeviceConfig = (props) => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [onlineConfig, setOnlineConfig] = useState({})
  const { data, isLoading } = useConfigurationDeviceOnlineConfig()
  const command = useSaveDeviceOnlineConfigData()
  useEffect(() => {
    if (data) {
      setOnlineConfig(data)
    }
  }, [data])

  //================================
  const handleItemChange = (name, value) => {
    setOnlineConfig((prev) => ({
      ...prev,
      [name]: value
    }))
  }

  const handleSubmit = () => {
    command.mutate(onlineConfig)
  }

  const onGoBack = () => {
    navigate('/')
  }

  //================================
  const render = () => {
    return (
      <div className="online-config-content">
        <Button
          name="casinoManagementSystem"
          color={onlineConfig?.casinoManagementSystem === true ? 'dark' : 'light'}
          className="action-button"
          icon={(props) => {
            if (onlineConfig?.casinoManagementSystem) return <IconToggleOn {...props} />
            if (!onlineConfig?.casinoManagementSystem) return <IconToggleOff {...props} />
          }}
          onClick={() =>
            handleItemChange('casinoManagementSystem', !onlineConfig?.casinoManagementSystem)
          }
        >
          {onlineConfig?.casinoManagementSystem && t('Enabled')}
          {!onlineConfig?.casinoManagementSystem && t('Disabled')}
        </Button>
        <TextInput
          name="url"
          label="URL"
          value={onlineConfig?.url ?? ''}
          onChange={(e) => handleItemChange('url', e.target.value)}
          size="m"
        />
        <TextInput
          name="deviceId"
          label="Device ID"
          value={onlineConfig?.deviceId ?? ''}
          onChange={(e) => handleItemChange('deviceId', e.target.value)}
          size="m"
        />
        <TextInput
          name="secretKey"
          label="Secret Key"
          value={onlineConfig?.secretKey ?? ''}
          onChange={(e) => handleItemChange('secretKey', e.target.value)}
          size="m"
        />
        <DecimalInput
          name="timeoutInSeconds"
          label={t('Timeout (seconds)')}
          value={onlineConfig?.timeoutInSeconds ?? ''}
          onValueChange={(value) => handleItemChange('timeoutInSeconds', value)}
          onClear={() => handleItemChange('timeoutInSeconds', 0)}
          hasClearButton={false}
          valuePosition="left"
          size="m"
        />
      </div>
    )
  }

  return (
    <Container className="regional-device-config">
      <FullPageLoader loading={command?.isPending || isLoading} />
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

export default OnlineDeviceConfig
