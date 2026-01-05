/* eslint-disable prettier/prettier */
import FullPageLoader from '@ui/components/full-page-loader'
import styled from '@emotion/styled'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import { useEffect, useState } from 'react'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useTranslation } from '@domain/administration/stores'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useNavigate } from 'react-router-dom'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import { useGetDeviceByType } from '@domain/device/stores'
import { DEVICE_TYPE } from '@domain/device/constants'
import { useConfigurationMoneyTITOPrinter } from '@domain/configuration/queries/money'
import { useSaveMoneyTITOPrinterData } from '@domain/configuration/mutations/money'
import Alert from '@ui/components/alert'
import DecimalInput from '@ui/components/inputs/decimal-input'

const Container = styled.div`
  font-weight: 600;
  height: 100%;
  display: flex;
  flex-direction: column;
  & .title {
    font-size: 1.75rem;
    margin-top: 4rem;
    margin-bottom: 1rem;
  }

  & .config-main-footer {
    padding-top: 1rem;
    margin-top: auto;
    display: flex;
    gap: 4rem;
    flex-grow: 1;
    justify-content: space-between;
    flex-grow: 0;
    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }
`

const Row = styled.div`
  display: flex;
  gap: 2rem;
  padding: 1rem 0;
  border-bottom: 1px solid var(--primary-medium);
  justify-content: space-between;
`
const TITO_MODE_OPTIONS = [
  { value: false, name: 'Local machine tickets' },
  { value: true, name: 'CMS integration' }
]

const TITOPrinterFutureLogicConfiguration = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const [config, setConfig] = useState(null)
  const device = useGetDeviceByType(DEVICE_TYPE.TITO_PRINTER)

  const command = useSaveMoneyTITOPrinterData()
  const { data, isLoading } = useConfigurationMoneyTITOPrinter()

  useEffect(() => {
    setConfig(data)
  }, [data])

  const handleSubmit = (e) => {
    command.mutate(config)
  }

  const disabled = device && device?.isConnected ? false : true

  const onGoBack = () => {
    navigate('/')
  }

  const handleSetBaudRate = (value) => {
    setConfig((prev) => ({ ...prev, baudRate: value }))
  }

  const handleSetIsCMSIntegration = (value) => {
    setConfig((prev) => ({ ...prev, isCasinoManagementSystem: value }))
  }

  const handleSetHasTemplate0 = (value) => {
    setConfig((prev) => ({ ...prev, hasTemplate0: value }))
  }

  const handleSetTicketTakingTimeout = (value) => {
    setConfig((prev) => ({ ...prev, ticketTakingTimeout: value }))
  }

  const handleSetWaitForTicketTaking = (value) => {
    setConfig((prev) => ({ ...prev, waitForTicketTaking: value }))
  }

  //===================================================

  return (
    <Container className="config-device-main">
      <FullPageLoader loading={isLoading || command?.isPending} />
      {disabled && <Alert severity="warning" text={t('Ticket printer device not available')} />}
      <Row>
        <DropdownInput
          disabled={disabled}
          label={t('TITO mode')}
          options={TITO_MODE_OPTIONS}
          value={config?.isCasinoManagementSystem || false}
          onChange={(value) => handleSetIsCMSIntegration(value?.value)}
        />
        <DropdownInput
          disabled={disabled}
          label={t('baud rate')}
          value={config?.baudRate}
          options={config?.supportedBaudRates.map((rate) => ({
            value: rate,
            name: rate.toString()
          }))}
          onChange={(value) => handleSetBaudRate(value?.value)}
        />
      </Row>

      <Row>
        <DecimalInput
          hasClearButton={false}
          value={config?.ticketTakingTimeout}
          size="m"
          disabled={disabled}
          label={t('Ticket taking timeout (milliseconds)')}
          onValueChange={(value) => handleSetTicketTakingTimeout(parseInt(value || 0))}
        />
      </Row>
      <Row>
        <CheckboxInput
          disabled={disabled}
          size="m"
          label={'HasTemplate0'}
          value={config?.hasTemplate0 || false}
          onChange={({ target }) => handleSetHasTemplate0(target.checked)}
        />
        <CheckboxInput
          disabled={disabled}
          size="m"
          label={'Wait for ticket taking'}
          value={config?.waitForTicketTaking}
          onChange={({ target }) => handleSetWaitForTicketTaking(target.checked)}
        />
      </Row>
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
          disabled={disabled}
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

export default TITOPrinterFutureLogicConfiguration
