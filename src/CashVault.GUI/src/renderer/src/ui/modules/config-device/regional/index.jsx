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
import { useConfigurationDeviceRegional } from '@domain/configuration/queries/device'
import { useSaveDeviceRegionalData } from '@domain/configuration/mutations/device'
import { useNavigate } from 'react-router-dom'
import { loadRegionalConfiguration } from '@domain/administration/stores/regional'

const Container = styled.div`
  font-weight: 600;
  height: 100%;

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

  display: flex;
  flex-direction: column;
`

const Row = styled.div`
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 1rem;

  > div {
    width: 100%;
  }
`

const RegionalDeviceConfig = (props) => {
  const { t } = useTranslation()
  const { data, isLoading } = useConfigurationDeviceRegional()
  const [regionalConfig, setRegionalConfig] = useState()
  const navigate = useNavigate()

  useEffect(() => {
    if (data) {
      setRegionalConfig(data)
    }
  }, [data])

  const onSubmitSuccess = () => {
    loadRegionalConfiguration(regionalConfig)
  }

  const command = useSaveDeviceRegionalData(onSubmitSuccess)

  const handleSubmit = () => {
    setRegionalConfig((prev) => ({
      ...prev,
      casinoDayStarts: '08:00'
    }))
    command.mutate(regionalConfig)
  }

  const onGoBack = () => {
    navigate('/')
  }

  const handleFieldChange = (field, value) => {
    setRegionalConfig((prev) => ({
      ...prev,
      [field]: value
    }))
  }

  const render = () => {
    return (
      <>
        <Row>
          <TextInput
            label={t('Name')}
            value={regionalConfig?.caption || ''}
            onChange={(e) => handleFieldChange('caption', e.target.value)}
            size="m"
          />
          <TextInput
            label={t('Machine name')}
            value={regionalConfig?.machineName || ''}
            onChange={(e) => handleFieldChange('machineName', e.target.value)}
            size="m"
          />
        </Row>
        <Row>
          <TextInput
            label={t('location')}
            value={regionalConfig?.locationName || ''}
            onChange={(e) => handleFieldChange('locationName', e.target.value)}
            size="m"
          />
          <TextInput
            label={t('Location address')}
            value={regionalConfig?.locationAddress || ''}
            onChange={(e) => handleFieldChange('locationAddress', e.target.value)}
            size="m"
          />
        </Row>
        <Row>
          <DropdownInput
            label={t('Local time zone')}
            options={regionalConfig?.localTimeZoneOptions || []}
            value={regionalConfig?.localTimeZone}
            onChange={(option) => {
              handleFieldChange('localTimeZone', option?.value)
            }}
          />

          <DropdownInput
            label={t('Default language')}
            options={regionalConfig?.defaultLanguageOptions || []}
            value={regionalConfig?.defaultLanguage}
            onChange={(option) => handleFieldChange('defaultLanguage', option?.value)}
          />
        </Row>
        <Row>
          <DropdownInput
            label={t('Date format')}
            options={regionalConfig?.dateFormatOptions || []}
            value={regionalConfig?.dateFormat}
            onChange={(option) => {
              handleFieldChange('dateFormat', option?.value)
            }}
          />
        </Row>
        <Row>
          <DropdownInput
            label={t('Decimal separator')}
            options={regionalConfig?.numberSeparatorOptions || []}
            value={regionalConfig?.decimalSeparator}
            onChange={(option) => {
              handleFieldChange('decimalSeparator', option?.value)

              let label = option?.label
              if (!option.value) label = null

              handleFieldChange('decimalSeparatorSymbol', label)
            }}
          />

          <DropdownInput
            label={t('Thousand separator')}
            options={regionalConfig?.numberSeparatorOptions || []}
            value={regionalConfig?.thousandSeparator}
            onChange={(option) => {
              handleFieldChange('thousandSeparator', option?.value)

              let label = option?.label
              if (!option.value) label = null

              handleFieldChange('thousandSeparatorSymbol', label)
            }}
          />
        </Row>
      </>
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
          textRight={t('Back')}
          onClick={onGoBack}
          color="dark"
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

export default RegionalDeviceConfig
