/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import MainText from '@ui/components/typography/main-text'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconReceipt from '@ui/components/icons/IconReceipt'
import { useNavigate } from 'react-router-dom'
import Spinner from '@ui/components/spinner'
import TextDivider from '@ui/components/text-divider'
import Divider from '@ui/components/divider'
import IconBanknote01 from '@ui/components/icons/IconBanknote01'
import IconThumbUp from '@ui/components/icons/IconThumbUp'
import IconInfoCircle from '@ui/components/icons/IconInfoCircle'
import { Mediator } from '@src/app/infrastructure/command-system'
import { CommandType } from '@domain/transactions/commands'
import {
  useCreditsActions,
  usePayoutProcessing,
  usePayoutProcessingCompleted,
  usePayoutMessages,
  useDetailedPayoutMessages,
  clearPayoutMessages,
  clearDetailedPayoutMessages,
  setPayoutRequestedTotal,
  usePayoutRequestedTotal
} from '@domain/transactions/store'
import { useEffect, useRef, useState } from 'react'
import { useTranslation } from '@domain/administration/stores'
import { useCreditsAmount } from '@domain/transactions/store'
import IconWarning from '@ui/components/icons/IconWarning.jsx'
import InvalidInputAlert from '@ui/components/invalid-input-alert'
import IconClose from '@ui/components/icons/IconClose'
import CircularTimer from '@ui/components/circle-timer'

const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .timer {
    position: absolute;
    top: 10rem;
    right: 5rem;
  }

  .text-error {
    color: var(--error-dark) !important;
  }

  & .pps-header {
    padding-bottom: 1rem;
  }

  & .icon-margin {
    margin-left: -0.75rem;
  }

  & .main-text {
    display: flex;
    flex-direction: column;
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

  & .pps-content {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  & .requested-amount {
    display: flex;
    gap: 1rem;
    align-items: center;

    & .text {
      color: var(--secondary-dark);
      font-weight: 500;
      font-size: 1.5rem;
      line-height: 2rem;
      letter-spacing: -3%;
    }

    & .amount {
      align-self: end;
      display: flex;
      align-items: center;
      margin-left: auto;

      font-weight: 700;
      font-size: 3.438rem;
      line-height: 3.75rem;
      letter-spacing: -4%;
      text-align: right;

      & span {
        padding-left: 0.5rem;
        font-weight: 500;
        font-size: 1.875rem;
        line-height: 2.375rem;
        letter-spacing: -3%;
        text-align: right;
      }
    }
  }

  & .pps-footer {
    margin-top: auto;
    display: flex;
    gap: 4rem;
  }

  & .timer-button {
    margin-left: auto;
  }
`

const PayoutProcessing = () => {
  const timeout = 30000
  const timerSeconds = 30
  const [autoCloseId, setAutoCloseId] = useState(null) // ID for the auto-close button rendering
  const timeoutRef = useRef()

  const navigate = useNavigate()
  const creditsAmount = useCreditsAmount()
  const { t } = useTranslation()

  const { togglePayoutProcessing, setPayoutProcessingCompleted } = useCreditsActions()
  const isProcessing = usePayoutProcessing()
  const isCompleted = usePayoutProcessingCompleted()
  const messages = usePayoutMessages()
  const detailedMessages = useDetailedPayoutMessages()
  const payoutRequestedTotal = usePayoutRequestedTotal()

  const cashAmountRequested =
    payoutRequestedTotal?.billSpecification?.reduce(
      (total, bill) => total + bill.denomination * bill.count,
      0
    ) || 0.0

  const isSuccess =
    messages?.every((item) => item.failed === false) === true && messages?.length > 0

  const isPartialSuccess =
    messages?.some((item) => item.failed === false) &&
    messages?.some((item) => item.failed === true) &&
    messages?.length > 0

  useEffect(() => {
    if (isCompleted || isProcessing === false) {
      const id = setTimeout(() => onGoBack(), timeout)
      timeoutRef.current = id
      setAutoCloseId(id)
    }
    return () => {
      if (timeoutRef.current) clearTimeout(timeoutRef.current)
    }
  }, [isCompleted, isProcessing])

  const cancelAutoclose = () => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current)
      timeoutRef.current = null
      setAutoCloseId(null)
    }
  }

  const onGoBack = () => {
    if (timeoutRef.current) {
      cancelAutoclose()
    }

    setPayoutProcessingCompleted(false)
    togglePayoutProcessing(false)
    clearPayoutMessages()
    clearDetailedPayoutMessages()
    setPayoutRequestedTotal([], 0)

    Mediator.dispatch(CommandType.GetCreditsAmount, {})

    navigate('/')
  }

  const renderRequestedMoney = () => {
    const hasCashError = messages.some((x) => x.isCache === true && x.failed === true)

    return (
      <div>
        <div className="requested-amount">
          <IconBanknote01
            size="l"
            color={hasCashError ? 'var(--error-dark)' : 'var(--secondary-dark)'}
          />

          <span className={hasCashError ? 'text text-error' : 'text'}>{t('Money')}</span>

          <div className="amount">
            {cashAmountRequested?.toFixed(2)}
            <span>{creditsAmount?.currencySymbol}</span>
          </div>
        </div>
        {messages.some((x) => x.isCache === true && x.failed) && (
          <InvalidInputAlert
            message={t(`${messages.find((x) => x.isCache && x.failed)?.reason}`)}
          />
        )}
      </div>
    )
  }

  const renderRequestedTicket = () => {
    const hasTicketError = messages.some((x) => x.isTicket === true && x.failed === true)

    return (
      <div>
        <div className="requested-amount">
          <IconReceipt
            size="l"
            color={hasTicketError ? 'var(--error-dark)' : 'var(--secondary-dark)'}
          />

          <span className={hasTicketError ? 'text text-error' : 'text'}>{t('Ticket')}</span>

          <div className="amount">
            {payoutRequestedTotal?.ticketAmount?.toFixed(2)}
            <span>{creditsAmount?.currencySymbol}</span>
          </div>
        </div>
        {messages.some((x) => x.isTicket === true && x.failed === true) && (
          <InvalidInputAlert
            message={t(
              `${messages.find((x) => x.isTicket && x.failed)?.reason || 'Unknown error'}`
            )}
          />
        )}
      </div>
    )
  }

  const renderHeaderText = () => {
    if (isSuccess && isCompleted) {
      return (
        <div className="main-text">
          <MainText>{t('Success')}!</MainText>
          <MainText>{t('Please take your money and ticket')}</MainText>
        </div>
      )
    } else if (isPartialSuccess === true) {
      return (
        <div className="main-text">
          <MainText isError={true}>{t('Partial Success!')}</MainText>
        </div>
      )
    } else if (!isSuccess && isCompleted) {
      return (
        <div className="main-text">
          <MainText isError={true}>{t('Something went wrong with withdraw')}</MainText>
        </div>
      )
    }

    return (
      <div className="main-text">
        <MainText>{t('Processing request')}.</MainText>
        <MainText>{t('Please wait')}</MainText>
      </div>
    )
  }

  const renderIcon = () => {
    if (isSuccess && isCompleted) {
      return (
        <div className="icon-margin">
          <IconThumbUp size="xl" color="var(--secondary-dark)" />
        </div>
      )
    }

    if (isPartialSuccess) {
      return (
        <div className="icon-margin">
          <IconWarning size="xl" color="var(--error-dark)" />
        </div>
      )
    }

    if (!isSuccess && isCompleted) {
      return (
        <div className="icon-margin">
          <IconInfoCircle size="xl" color="var(--error-dark)" />
        </div>
      )
    }

    return <div className="icon">{isCompleted === false && <Spinner />}</div>
  }

  return (
    <ScreenContainer hasOnGoBack={false} hasLogo={false}>
      <Container>
        {!!autoCloseId && <CircularTimer className="timer" duration={timerSeconds} />}
        <div className="pps-header">
          {renderIcon()}

          <div className="alt-text">{t('WITHDRAW YOUR MONEY/TICKET')}</div>

          {renderHeaderText()}
        </div>

        {(isCompleted === false || isPartialSuccess == true || messages.some((x) => x.reason)) && (
          <div className="pps-content">
            <TextDivider>{t('REQUESTED AMOUNT')}</TextDivider>
            {renderRequestedMoney()}
            <Divider />
            {renderRequestedTicket()}
          </div>
        )}

        {detailedMessages &&
          detailedMessages?.length > 0 &&
          detailedMessages?.map((message, idx) => <div key={`message_${idx}`}>{message}</div>)}

        {(isCompleted || isProcessing === false) && (
          <div className="pps-footer">
            <CircleButton
              icon={(props) => <IconLeftHalfArrow {...props} />}
              size="l"
              color="dark"
              textRight={t('Back')}
              onClick={() => onGoBack()}
            />

            {!!autoCloseId && (
              <CircleButton
                icon={(props) => <IconClose {...props} />}
                size="l"
                color="dark"
                className="timer-button"
                textRight={t('Cancel autoclose')}
                onClick={() => cancelAutoclose()}
              />
            )}
          </div>
        )}
      </Container>
    </ScreenContainer>
  )
}

export default PayoutProcessing
