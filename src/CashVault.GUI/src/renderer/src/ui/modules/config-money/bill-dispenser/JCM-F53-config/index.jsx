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
import { useConfigurationMoneyBillDispenser } from '@domain/configuration/queries/money'
import { useSaveMoneyBillDispenserConfigData } from '@domain/configuration/mutations/money'
import CassetteItem from './cassette-item'
import { cloneDeep } from 'lodash'
import DecimalInput from '@ui/components/inputs/decimal-input'
import { useGetDeviceByType } from '@domain/device/stores'
import { DEVICE_TYPE } from '@domain/device/constants'
import Alert from '@ui/components/alert'

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

  .send-low-level-warning-container {
    margin-top: 1rem;
  }
`

const Row = styled.div`
  display: flex;
  gap: 2rem;
  padding: 1rem 0;
  border-bottom: 1px solid var(--primary-medium);
  max-width: 65rem;
`

const BillDispenserJCMF53Config = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { data, isLoading } = useConfigurationMoneyBillDispenser()
  const device = useGetDeviceByType(DEVICE_TYPE.BILL_DISPENSER)

  const command = useSaveMoneyBillDispenserConfigData()

  const [config, setConfig] = useState(null)
  const [dispenseChannels, setDispenseChannelsValue] = useState()

  useEffect(() => {
    if (data?.billCassettes?.length > 0) {
      setDispenseChannelsValue(data?.billCassettes?.length)
    }

    setConfig(data)
  }, [data])

  //=================================================

  const handleCassetteDenomChange = (cassettenumber, { value }) => {
    const denom = config?.supportedDenominations.find((x) => x.value === value)

    const cassette = cloneDeep(
      config.billCassettes.filter((x) => x.cassetteNumber === cassettenumber)
    )

    const copiedList = cloneDeep(
      config.billCassettes.filter((x) => x.cassetteNumber !== cassettenumber)
    )

    copiedList.push({
      ...cassette[0],
      billDenomination: denom
    })

    copiedList.sort((a, b) => a.cassetteNumber - b.cassetteNumber)

    setConfig((prev) => ({
      ...prev,
      billCassettes: copiedList
    }))
  }

  const handleDispenseChannelsChange = ({ value }) => {
    let newDispenseChannels = value
    setDispenseChannelsValue(newDispenseChannels)

    if (newDispenseChannels > dispenseChannels) {
      let addCount = newDispenseChannels - dispenseChannels
      let newCassettes = [...config.billCassettes]

      for (let i = 1; i <= addCount; i++) {
        let lastCassette = newCassettes[config.billCassettes.length - 1]
        let newCassette = {
          ...lastCassette,
          cassetteNumber: lastCassette.cassetteNumber + i
        }
        newCassettes.push(newCassette)
      }
      setConfig((prev) => ({
        ...prev,
        billCassettes: newCassettes
      }))
    } else if (newDispenseChannels < dispenseChannels) {
      let newCassettes = config.billCassettes.filter((x) => x.cassetteNumber <= newDispenseChannels)
      setConfig((prev) => ({
        ...prev,
        billCassettes: newCassettes
      }))
    }
  }

  const handleDenomMagnetChange = (cassetteNumber, denomMagnet, value) => {
    const cassette = cloneDeep(
      config.billCassettes.filter((x) => x.cassetteNumber === cassetteNumber)
    )

    const copiedList = cloneDeep(
      config.billCassettes.filter((x) => x.cassetteNumber !== cassetteNumber)
    )

    copiedList.push({
      ...cassette[0],
      denominationMagnetStatus: {
        ...(cassette[0]?.denominationMagnetStatus || {}),
        [`${denomMagnet}`]: value
      }
    })

    copiedList.sort((a, b) => a.cassetteNumber - b.cassetteNumber)

    setConfig((prev) => ({
      ...prev,
      billCassettes: copiedList
    }))
  }

  const disabled = device && device?.isConnected ? false : true

  const onGoBack = () => {
    navigate('/')
  }

  const handleSubmit = () => {
    command.mutate(config)
  }

  const handleSetMaxCountRejected = (value) => {
    setConfig((prev) => ({
      ...prev,
      maxNumberOfCountReject: +value
    }))
  }

  const handleSetPickRetriesOfCount = (value) => {
    setConfig((prev) => ({
      ...prev,
      pickRetriesOfCount: +value
    }))
  }

  const handleCurrencyChange = ({ value }) => {
    const currency = config?.supportedCurrencies.find((c) => c.code === value)

    setConfig((prev) => ({
      ...prev,
      currentCurrency: currency
    }))
  }

  const getDispenseChannelsOptions = (n) => {
    return Array.from({ length: n }, (_, i) => i + 1)
  }

  //===================================================

  return (
    <Container className="config-device-main">
      <FullPageLoader loading={isLoading || command?.isPending} />
      {disabled && <Alert severity="warning" text={t('Bill dispenser device not available')} />}
      <div className="config-money-bill-dispenser">
        <Row>
          <DecimalInput
            size="m"
            label={t('Max Number Of Count Reject')}
            value={config?.maxNumberOfCountReject || 3}
            onValueChange={(value) => handleSetMaxCountRejected(value)}
            hasClearButton={false}
            valuePosition="left"
            disabled={disabled}
          />
          <DecimalInput
            size="m"
            label={t('Pick Retries Of Count')}
            value={config?.pickRetriesOfCount || 3}
            onValueChange={(value) => handleSetPickRetriesOfCount(value)}
            hasClearButton={false}
            valuePosition="left"
            disabled={disabled}
          />
        </Row>
        <Row>
          <DropdownInput
            label={t('Currency')}
            disabled={disabled || config?.isEmpty === false}
            info={t('Set bill dispenser cassette currency')}
            onChange={handleCurrencyChange}
            value={config?.currentCurrency?.code}
            options={(config?.supportedCurrencies || []).map((c) => ({
              value: c.code,
              name: c.symbol
            }))}
          />
          <DropdownInput
            disabled={disabled || config?.isEmpty === false}
            options={getDispenseChannelsOptions(config?.maxBillCassettes || 6).map((d) => ({
              value: d,
              name: d.toString()
            }))}
            label={t('Dispense channels')}
            info={t('Select the number of channels to dispense bills')}
            onChange={handleDispenseChannelsChange}
            value={dispenseChannels}
          />
        </Row>

        <div className="title">{t('Cassette info')}</div>
        {config?.isEmpty === false && (
          <Alert
            severity="warning"
            text={t(
              'Settings cannot be changed while the dispenser is not empty. Please empty it first.'
            )}
          />
        )}
        {(config?.billCassettes || [])
          ?.sort((a, b) => a.cassetteNumber - b.cassetteNumber)
          .map((x, i) => (
            <CassetteItem
              disabled={disabled || config?.isEmpty === false}
              key={i}
              cassette={x}
              supportedDenominations={config?.supportedDenominations.filter(
                (x) => x.currency.code === config.currentCurrency.code
              )}
              onDenomChange={(value) => handleCassetteDenomChange(x?.cassetteNumber, value)}
              onDenomMagnetChange={handleDenomMagnetChange}
            />
          ))}
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
          disabled={disabled || config?.isEmpty === false}
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

export default BillDispenserJCMF53Config
