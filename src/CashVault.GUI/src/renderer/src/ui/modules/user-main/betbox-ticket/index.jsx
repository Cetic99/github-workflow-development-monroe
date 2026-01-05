/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import MainText from '@ui/components/typography/main-text'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useNavigate } from 'react-router-dom'
import { useEffect, useRef, useState } from 'react'
import { useTranslation } from '@domain/administration/stores'
import FullPageLoader from '@ui/components/full-page-loader'
import { useSendBetboxTicketAck, useSendBetboxTicketNack } from '@domain/money/commands'
import { useRedeemBetboxTicket } from '@src/app/domain/money/queries'
import ScanImage from '@ui/assets/images/scan-code.svg'
import { v4 } from 'uuid'

const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
  position: relative;
  gap: 1rem;

  margin-top: 5rem;

  & .pts-header {
    padding-bottom: 1rem;

    & p {
      font-size: 1.625rem;
      font-weight: 400;
      line-height: 2.375rem;
    }
  }

  & .icon {
    margin-left: -1.25rem;
  }

  & .main-text {
    max-width: 60.5rem;
  }

  & .confirm-withdraw-text {
    max-width: 60.5rem;
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

  & .scan-image {
    position: absolute;
    top: 12rem;
    width: 30.5rem;
  }

  & .balance {
    font-size: 4.375rem;
    line-height: 4.375rem;
    font-weight: bold;
    color: var(--secondary-dark);
  }

  & .balance-text {
    color: var(--secondary-dark);
  }

  & .balance-value {
    color: black;
  }

  & form {
    & button {
      display: none;
    }
  }

  & .pts-footer {
    position: fixed;
    bottom: 3rem;
    width: 100%;
    display: flex;
    gap: 4rem;
  }

  & .footer-wrapper {
    position: fixed;
    bottom: 14rem;

    h1 {
      font-size: 3.438rem;
      font-weight: bold;
      line-height: 3.438rem;
    }
  }
`

const BetboxTicket = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [barcode, setBarcode] = useState('')
  const textInputRef = useRef()
  const transactionKey = useRef(v4()) // Used to invalidate previous queries

  const onSuccessAck = () => {
    navigate('/success?success=true')
  }

  const onError = () => {
    navigate('/success?success=false')
  }

  const {
    data: redeemTicketData,
    isLoading,
    isError
  } = useRedeemBetboxTicket(barcode, transactionKey.current)
  const { mutate: sendTicketAck, isPending: isAckPending } = useSendBetboxTicketAck(
    onSuccessAck,
    onError
  )
  const { mutate: sendTicketNack, isPending: isNackPending } = useSendBetboxTicketNack()

  const handleSubmit = (e) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const code = formData.get('barcode')

    setBarcode(code)
    e.currentTarget.reset()
  }

  const handleOnConfirm = () => {
    if (barcode) {
      sendTicketAck({ barcode, id: transactionKey.current })
    }
  }

  const handleOnCancel = () => {
    if (barcode) {
      sendTicketNack({ barcode, id: transactionKey.current })
    }
    setBarcode('')
    navigate('/')
  }

  useEffect(() => {
    if (textInputRef.current) {
      textInputRef.current.focus()
    }
  }, [])

  useEffect(() => {
    if (redeemTicketData?.isValid === false) {
      setBarcode('')
      onError()
    }
    if (isError === true) {
      setBarcode('')
      onError()
    }
  }, [redeemTicketData, isError])

  return (
    <ScreenContainer
      hasLogo={true}
      isLoading={isLoading || isAckPending || isNackPending || redeemTicketData?.totalAmount <= 0}
      loadingText={t('We are preparing your transaction.')}
    >
      <FullPageLoader loading={isLoading || isAckPending || isNackPending} />
      <Container>
        <div className="pts-header">
          <div className="alt-text">{t('Redeem betbox ticket')}</div>

          <div className="main-text">
            <MainText>
              {t(
                redeemTicketData?.totalAmount > 0
                  ? 'Amount to withdraw'
                  : 'Please scan the bar code'
              )}
            </MainText>
          </div>
        </div>

        {!redeemTicketData && <img className="scan-image" src={ScanImage} alt="Scan code" />}

        <div className="pts-content">
          <form onSubmit={handleSubmit}>
            <input
              type="text"
              name="barcode"
              autoComplete="off"
              inputMode="none"
              ref={textInputRef}
              style={{ opacity: 0, position: 'absolute', pointerEvents: 'none' }}
            />
            <button type="submit"></button>
          </form>
        </div>

        <div className="balance">
          {redeemTicketData?.totalAmount > 0 && (
            <span>
              {redeemTicketData?.totalAmount?.toFixed(2)} {redeemTicketData?.currency?.symbol}
            </span>
          )}
        </div>

        <div className="footer-wrapper">
          {redeemTicketData && (
            <h1>
              <p className="confirm-withdraw-text">
                {t('Do you want to apply the voucher to the device credit?')}
              </p>
            </h1>
          )}
          <div className="pts-footer">
            <CircleButton
              icon={(props) => <IconClose {...props} />}
              size="l"
              color="medium"
              textRight={t('Cancel')}
              disabled={isLoading}
              onClick={() => handleOnCancel()}
            />

            <CircleButton
              icon={(props) => <IconRightHalfArrow {...props} />}
              size="l"
              color="dark"
              textRight={t('Continue')}
              onClick={handleOnConfirm}
              disabled={!redeemTicketData?.isValid}
            />
          </div>
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default BetboxTicket
