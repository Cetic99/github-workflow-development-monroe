/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import MainText from '@ui/components/typography/main-text'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import DecimalInput from '@ui/components/inputs/decimal-input'
import { useNavigate } from 'react-router-dom'
import TextDivider from '@ui/components/text-divider'
import DenominationConfig from '@ui/components/denomination-config'
import { useGetBillDispenseOptions } from '@domain/money/queries'
import { useCreditsAmount } from '@domain/transactions/store'
import FullPageLoader from '@ui/components/full-page-loader'
import { useEffect, useState } from 'react'
import { useIsTITOPrinterReady, useIsBillDispenserReady } from '@domain/device/stores'
import { Container } from './style'
import { Mediator } from '@src/app/infrastructure/command-system'
import { CommandType } from '@domain/transactions/commands'
import { useTranslation } from '@domain/administration/stores'
import { computeDifferenceToFixed } from '@src/app/domain/money/services'
import {
  clearPayoutMessages,
  clearDetailedPayoutMessages,
  setPayoutRequestedTotal,
  useCreditsActions
} from '@domain/transactions/store'

const WithdrawMoney = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()
  const creditsAmount = useCreditsAmount()
  const { togglePayoutProcessing, setPayoutProcessingCompleted } = useCreditsActions()

  const isPrinterReady = useIsTITOPrinterReady()
  const isBillDispenserReady = useIsBillDispenserReady()

  const { data, isLoading } = useGetBillDispenseOptions(() => {})
  const prefilledAmountToPrint = data?.prefilledAmountToPrint?.toFixed(data?.amountPrecision || 2)
  const totalAmount = creditsAmount?.amount || 0
  const formatAmountToDisplay = (amount, amountPrecision = 2, currency = 'KM') => {
    const numericAmount = parseFloat(amount)
    if (isNaN(numericAmount)) {
      return `0.00 ${currency}`
    }
    return `${numericAmount.toFixed(amountPrecision)} ${currency}`
  }

  // ============================================ //
  // State to manage denominations and their counts
  const [selectedDenominations, setSelectedDenominations] = useState([])

  useEffect(() => {
    if (data && data?.denominationDispenseOptions) {
      setSelectedDenominations(
        (data?.denominationDispenseOptions || [])?.reduce((acc, denom) => {
          acc[denom.billDenomination] = 0
          return acc
        }, {})
      )
    }
  }, [data])

  const [amountToPrint, setAmountToPrint] = useState(null)
  // =============================================
  const handleIncrement = (denom) => {
    if (getTotalDispenseAmount() + denom.billDenomination <= totalAmount) {
      setSelectedDenominations((prev) => ({
        ...prev,
        [denom.billDenomination]:
          (isNaN(prev[denom.billDenomination]) ? 0 : prev[denom.billDenomination]) + 1
      }))
    }
  }

  const handleClear = (denom) => {
    setSelectedDenominations((prev) => ({
      ...prev,
      [denom.billDenomination]: 0
    }))
  }

  const handleDecrement = (denom) => {
    setSelectedDenominations((prev) => ({
      ...prev,
      [denom.billDenomination]: Math.max(prev[denom.billDenomination] - 1, 0)
    }))
  }

  const getTotalDispenseAmount = () => {
    return Object.entries(selectedDenominations).reduce(
      (sum, [billDenomination, count]) =>
        sum + (isNaN(billDenomination) ? 0 : billDenomination) * (isNaN(count) ? 0 : count),
      0
    )
  }

  const [isManualInput, setIsManualInput] = useState(false)

  const handleAmountToPrintChange = (value) => {
    setIsManualInput(true)

    if (value < 0) {
      setAmountToPrint(0)
      return
    }

    if (value > totalAmount - getTotalDispenseAmount()) {
      setAmountToPrint(
        computeDifferenceToFixed(
          totalAmount,
          getTotalDispenseAmount(),
          creditsAmount?.amountPrecision || 2
        )
      )
      return
    }
    setAmountToPrint(value)
  }

  const handleSelectedDenominationChange = (denom, value) => {
    const parsedValue = value ? parseInt(value) : undefined

    if (parsedValue < 0) return

    if (value > denom.maxBillCount) return

    if (parsedValue * denom.billDenomination > totalAmount - getTotalDispenseAmount()) return

    setSelectedDenominations((prev) => ({
      ...prev,
      [denom.billDenomination]: parsedValue
    }))
  }

  useEffect(() => {
    if (data?.prefilledCombinations) {
      setSelectedDenominations(
        data?.prefilledCombinations?.reduce((acc, denom) => {
          acc[denom.denomination] = denom.count
          return acc
        }, {})
      )
    }

    if (prefilledAmountToPrint) {
      if (!isPrinterReady) {
        setAmountToPrint(0)
      } else {
        setAmountToPrint(prefilledAmountToPrint)
      }
    }
  }, [data?.prefilledCombinations, prefilledAmountToPrint])

  useEffect(() => {
    if (!isPrinterReady) {
      setAmountToPrint(0)
    }

    if (!isBillDispenserReady) {
      setSelectedDenominations(
        data?.denominationDispenseOptions?.reduce((acc, denom) => {
          acc[denom.billDenomination] = 0
          return acc
        }, {})
      )
    }
  }, [isPrinterReady, isBillDispenserReady])

  useEffect(() => {
    if (amountToPrint === 0 || isManualInput) {
      return
    }

    setAmountToPrint(
      computeDifferenceToFixed(
        totalAmount,
        getTotalDispenseAmount(),
        creditsAmount?.amountPrecision || 2
      )
    )

    if (!isPrinterReady) {
      setAmountToPrint(0)
    }
  }, [selectedDenominations, creditsAmount])

  // ================== Actions ======================== //
  const handleConfirm = () => {
    setIsManualInput(false)
    setPayoutProcessingCompleted(false)
    togglePayoutProcessing(false)
    clearPayoutMessages()
    clearDetailedPayoutMessages()
    setPayoutRequestedTotal([], 0)

    const billSpecification = Object.entries(selectedDenominations)
      .map(([billDenomination, count]) => ({
        denomination: parseInt(billDenomination),
        count
      }))
      .filter((item) => item.count > 0)

    Mediator.dispatch(CommandType.RequestPayout, {
      ticketAmount: parseFloat(amountToPrint) || 0,
      billSpecification: billSpecification
    })

    navigate('/payout-processing')
  }

  const handleCancel = () => {
    setIsManualInput(false)
    setSelectedDenominations(
      data?.denominationDispenseOptions?.reduce((acc, denom) => {
        acc[denom.billDenomination] = 0
        return acc
      }, {})
    )
    navigate('/')
  }

  return (
    <ScreenContainer hasLogo={false}>
      <FullPageLoader loading={isLoading} />
      <Container>
        <div className="wms-header">
          <div className="alt-text">{t('WITHDRAW YOUR MONEY')}</div>

          <div className="header-content">
            <div className="main-left">
              <MainText>{t('Enter amount')}</MainText>
            </div>

            <div className="current-balance">
              <div className="text">{t('Your current balance')}:</div>
              <div className="value">
                {creditsAmount?.amount.toFixed(creditsAmount?.amountPrecision)}{' '}
                {creditsAmount.currency}
              </div>
            </div>
          </div>
        </div>

        <div className="wms-content">
          <TextDivider>{t('REQUESTED AMOUNT')}</TextDivider>

          <div className="configuration">
            {data?.denominationDispenseOptions?.map((denom, index) => (
              <DenominationConfig
                key={index}
                denom={denom?.billDenomination}
                denomConfig={denom}
                maxCount={denom?.maxCount}
                currency={data?.currencySymbol}
                count={selectedDenominations[denom.billDenomination]}
                increaseDisabled={
                  getTotalDispenseAmount() + denom.billDenomination > totalAmount ||
                  selectedDenominations[denom.billDenomination] >= denom.maxBillCount ||
                  !isBillDispenserReady
                }
                decreaseDisabled={
                  selectedDenominations[denom.billDenomination] === 0 ||
                  isNaN(selectedDenominations[denom.billDenomination]) ||
                  !isBillDispenserReady
                }
                onDecrease={handleDecrement}
                onIncrease={handleIncrement}
                onClear={handleClear}
                onChange={handleSelectedDenominationChange}
              />
            ))}
          </div>

          <TextDivider>{t('AMOUNT TO WITHDRAW')}</TextDivider>

          <div className="withdraw-amount">
            <div className="main">
              <div className="text">{t('Money withdrawal')}</div>
              <div className="value">
                {formatAmountToDisplay(
                  getTotalDispenseAmount(),
                  data?.amountPrecision,
                  data?.currencySymbol
                )}
              </div>
            </div>

            <div className="info">
              <span className="text">{t('MAXIMUM AMOUNT FOR WITHDRAWAL')}: </span>
              <span className="value">
                {formatAmountToDisplay(
                  data?.maximumAmount || 0,
                  data?.amountPrecision,
                  data?.currencySymbol
                )}
              </span>
            </div>
          </div>

          <TextDivider>{t('TICKET AMOUNT TO PRINT')}</TextDivider>

          <div className="print-amount">
            <div className="input">
              <DecimalInput
                size="m"
                decimalsLimit={2}
                value={amountToPrint ?? 0}
                onClear={() => setAmountToPrint(0)}
                onValueChange={handleAmountToPrintChange}
                disabled={!isPrinterReady}
              />
            </div>

            <div className="value">
              {formatAmountToDisplay(amountToPrint, data?.amountPrecision, data?.currencySymbol)}
            </div>
          </div>
        </div>

        <div className="wms-footer">
          <CircleButton
            icon={(props) => <IconClose {...props} />}
            size="l"
            color="medium"
            textRight={t('Cancel')}
            onClick={() => handleCancel()}
          />

          <CircleButton
            icon={(props) => <IconRightHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Accept')}
            disabled={
              (getTotalDispenseAmount() == 0 && amountToPrint == 0) ||
              (!isPrinterReady && !isBillDispenserReady)
            }
            onClick={() => handleConfirm()}
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default WithdrawMoney
