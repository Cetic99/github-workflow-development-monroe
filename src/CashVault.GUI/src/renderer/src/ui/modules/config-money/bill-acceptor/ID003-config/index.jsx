/* eslint-disable react/prop-types */
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
import { DEVICE_TYPE } from '@domain/device/constants'
import { useGetDeviceByType } from '@domain/device/stores'
import lodash from 'lodash'
import { useConfigurationMoneyBillAcceptor } from '@domain/configuration/queries/money'
import { useSaveMoneyBillAcceptorData } from '@domain/configuration/mutations/money'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import DecimalInput from '@ui/components/inputs/decimal-input'

const Container = styled.div`
  font-weight: 600;
  height: 100%;
  display: flex;
  flex-direction: column;
  padding-bottom: 2rem;

  & .title {
    font-size: 1.75rem;
    margin-top: 4rem;
    margin-bottom: 1rem;
  }

  .config-main-footer {
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

const Row = styled.div`
  display: flex;
  gap: 2rem;
  padding: 1rem 0;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid var(--primary-medium);

  & .checkbox-input {
    padding-top: 2.5rem;
  }

  > div {
    width: 50%;
    justify-content: start;
  }
`

const BillAcceptorID003Config = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const device = useGetDeviceByType(DEVICE_TYPE.BILL_ACCEPTOR)

  const [currency, setCurrency] = useState('')
  const [config, setConfig] = useState(null)

  const { data, isLoading } = useConfigurationMoneyBillAcceptor(currency)

  useEffect(() => {
    setCurrency(config?.currentCurrency?.code || '')
    setConfig(data)
  }, [data])

  const command = useSaveMoneyBillAcceptorData()

  const handleChangeCurrency = (value) => {
    setCurrency(config?.supportedCurrencies?.find((item) => item.code === value?.value))

    setConfig((prev) => ({
      ...prev,
      billDenominationConfig: config?.supportedCurrencies?.find(
        (item) => item.currency.code === value?.value
      )?.billDenominations
    }))
  }

  const handlePaperDenominationChange = (denom, newValue) => {
    let temp = lodash.cloneDeep(config?.billDenominationConfig || [])
    temp.find((x) => x.dataValue == denom.dataValue).isEnabled = newValue
    setConfig((prev) => ({
      ...prev,
      billDenominationConfig: temp
    }))
  }

  const disabled = device && device?.isConnected ? false : true

  const onGoBack = () => {
    navigate('/')
  }

  const handleSubmit = () => {
    command.mutate(config)
  }

  const handleAcceptTITOTicketsChange = (value) => {
    setConfig((prev) => ({
      ...prev,
      acceptTITOTickets: value
    }))
  }

  //===================================================

  const BillAcceptorConfigItem = ({ item, onChange }) => {
    const { dataValue, isEnabled } = item
    return (
      <Row>
        <DecimalInput
          size="m"
          label={t('Denomination')}
          hasClearButton={false}
          valuePosition="left"
          value={dataValue}
          disabled={true}
        />

        <CheckboxInput
          className="checkbox-input"
          disabled={disabled}
          label={t('Enabled')}
          value={isEnabled}
          onChange={(e) => onChange(item, e.target.checked)}
        />
      </Row>
    )
  }

  return (
    <Container className="config-device-main">
      <FullPageLoader loading={isLoading || command?.isPending} />
      <Row>
        <DropdownInput
          label={t('Currency')}
          disabled={disabled}
          value={config?.currentCurrency?.code}
          options={(config?.supportedCurrencies || [])?.map((x) => ({
            name: x.currency.code,
            value: x.currency.code
          }))}
          onChange={(value) => handleChangeCurrency(value)}
          info={t('Configure bill denomination for selected currency.')}
        />

        <CheckboxInput
          className="checkbox-input"
          disabled={disabled}
          label={t('Accept TITO tickets')}
          value={config?.acceptTITOTickets || false}
          onChange={(e) => handleAcceptTITOTicketsChange(e.target.checked)}
        />
      </Row>

      <div>
        <div className="title">{t('Paper bill denominations:')}</div>
        {(config?.billDenominationConfig || [])?.map((x, i) => (
          <BillAcceptorConfigItem key={i} item={x} onChange={handlePaperDenominationChange} />
        ))}
        {config?.billDenominationConfig?.length == 0 && (
          <div className="no-data">{t('No data')}</div>
        )}
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

export default BillAcceptorID003Config
