/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import MainText from '@ui/components/typography/main-text'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import IconReceipt from '@ui/components/icons/IconReceipt'
import DecimalInput from '@ui/components/inputs/decimal-input'
import { useNavigate } from 'react-router-dom'
import { useCreditsAmount } from '@domain/transactions/store'
import { useEffect, useRef, useState } from 'react'
import { useIsTITOPrinterReady } from '@domain/device/stores'
import { Mediator } from '@src/app/infrastructure/command-system'
import { CommandType } from '@domain/transactions/commands'
import { useTranslation } from '@domain/administration/stores'
import {
  clearPayoutMessages,
  clearDetailedPayoutMessages,
  setPayoutRequestedTotal,
  useCreditsActions
} from '@domain/transactions/store'

const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .pts-header {
    padding-bottom: 1rem;
  }

  & .icon {
    margin-left: -1.25rem;
  }

  & .main-text {
    max-width: 40.5rem;
  }

  & .alt-text {
    color: var(--secondary-dark);
    font-weight: 600;
    font-size: 1rem;
    line-height: 1.125rem;
    letter-spacing: 4%;
    text-transform: uppercase;
    padding-top: 1rem;
  }

  & .pts-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  & .balance {
    font-weight: 700;
    font-size: 1.5rem;
    line-height: 2.375rem;
    letter-spacing: -3%;
    text-align: right;
  }

  & .balance-text {
    color: var(--secondary-dark);
  }

  & .balance-value {
    color: black;
  }

  & .pts-footer {
    margin-top: auto;
    display: flex;
    gap: 4rem;
  }
`

const PrintTicket = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const creditsAmount = useCreditsAmount()
  const isPrinterReady = useIsTITOPrinterReady()
  const [printValue, setPrintValue] = useState(0)
  const { togglePayoutProcessing, setPayoutProcessingCompleted } = useCreditsActions()
  const printAmountInputRef = useRef()

  const handleOnValueChange = (value, names, values) => {
    setPrintValue(value)
  }

  const handleOnClear = () => {
    setPrintValue(0)
  }

  const handlePrintTicket = () => {
    if (printValue > 0) {
      setPayoutProcessingCompleted(false)
      togglePayoutProcessing(false)
      clearPayoutMessages()
      clearDetailedPayoutMessages()
      setPayoutRequestedTotal([], 0)

      Mediator.dispatch(CommandType.RequestPayout, {
        ticketAmount: +printValue,
        creditsAmount: +creditsAmount.amount
      })
      navigate('/payout-processing')
    }
  }

  useEffect(() => {
    if (printAmountInputRef.current) {
      printAmountInputRef.current.focus()
    }
  }, [])

  return (
    <ScreenContainer hasLogo={false}>
      <Container>
        <div className="pts-header">
          <div className="icon">
            <IconReceipt color="var(--secondary-dark)" size="xl" />
          </div>

          <div className="alt-text">{t('Print your ticket')}</div>

          <div className="main-text">
            <MainText>{t('Enter amount to print on ticket')}</MainText>
          </div>
        </div>

        <div className="pts-content">
          <div className="balance">
            <span className="balance-text">{t('Your current balance')}: </span>
            <span className="balance-value">
              {creditsAmount?.amount?.toFixed(creditsAmount?.amountPrecision)}{' '}
              {creditsAmount?.currency}
            </span>
          </div>

          <div className="input">
            <DecimalInput
              ref={printAmountInputRef}
              value={printValue}
              onClear={handleOnClear}
              inputType="number"
              onValueChange={handleOnValueChange}
            />
          </div>
        </div>

        <div className="pts-footer">
          <CircleButton
            icon={(props) => <IconClose {...props} />}
            size="l"
            color="medium"
            textRight={t('Cancel')}
            onClick={() => navigate(-1)}
          />

          <CircleButton
            icon={(props) => <IconRightHalfArrow {...props} />}
            size="l"
            color="dark"
            textRight={t('Accept')}
            onClick={handlePrintTicket}
            disabled={
              !isPrinterReady ||
              printValue <= 0 ||
              printValue > creditsAmount.amount ||
              Number.isNaN(printValue)
            }
          />
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default PrintTicket
