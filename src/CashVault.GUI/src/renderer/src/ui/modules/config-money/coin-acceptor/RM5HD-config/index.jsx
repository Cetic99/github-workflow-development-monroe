/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'

import { useTranslation } from '@domain/administration/stores'
import { useSaveMoneyCoinAcceptorData } from '@src/app/domain/configuration/mutations/money'
import { useConfigurationMonenyCoinAcceptorData } from '@src/app/domain/configuration/queries/money'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'
import { useGetDeviceByType } from '@src/app/domain/device/stores'
import lodash from 'lodash'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'

import CheckboxInput from '@ui/components/inputs/checkbox-input'
import DecimalInput from '@ui/components/inputs/decimal-input'
import FullPageLoader from '@ui/components/full-page-loader'
import Alert from '@ui/components/alert'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'

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

const CoinAcceptorRM5HDConfig = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const device = useGetDeviceByType(DEVICE_TYPE.COIN_ACCEPTOR)

  const [currency, setCurrency] = useState('')
  const [config, setConfig] = useState(null)

  const { data, isLoading } = useConfigurationMonenyCoinAcceptorData(currency)

  const command = useSaveMoneyCoinAcceptorData()

  const disabled = device && device?.isConnected ? false : true

  useEffect(() => {
    setCurrency(config?.currentCurrency?.code || '')
    setConfig(data)
  }, [data])

  const handleChangeCurrency = (value) => {
    setCurrency(config?.supportedCurrencies?.find((item) => item.code === value?.value))

    setConfig((prev) => ({
      ...prev,
      coinDenominationConfig: config?.supportedCurrencies?.find(
        (item) => item.currency.code === value?.value
      )?.coinDenominations
    }))
  }

  const handleCoinDenominationChange = (denom, newValue) => {
    let temp = lodash.cloneDeep(config?.coinDenominationConfig || [])
    temp.find((x) => x.dataValue == denom.dataValue).isEnabled = newValue
    setConfig((prev) => ({
      ...prev,
      coinDenominationConfig: temp
    }))
  }

  const onGoBack = () => {
    navigate('/')
  }

  const handleSubmit = () => {
    command.mutate(config)
  }

  const CoinAcceptorConfigItem = ({ item, onChange }) => {
    const { value, isEnabled } = item
    return (
      <Row>
        <DecimalInput
          size="m"
          label={t('Denomination')}
          hasClearButton={false}
          valuePosition="left"
          value={value}
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
    <Container className="money-config-coin-acc-main">
      <FullPageLoader loading={isLoading || command?.isPending} />
      {disabled && <Alert severity="warning" text={t('Coin acceptor device not available')} />}

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
          info={t('Configure coin denomination for selected currency.')}
        />
      </Row>

      <div>
        <div className="title">{t('Coin denominations:')}</div>
        {(config?.coinDenominationConfig || [])?.map((x, i) => (
          <CoinAcceptorConfigItem key={i} item={x} onChange={handleCoinDenominationChange} />
        ))}
        {config?.coinDenominationConfig === 0 && <div className="no-data">{t('No data')}</div>}
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

export default CoinAcceptorRM5HDConfig
