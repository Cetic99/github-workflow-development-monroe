import styled from '@emotion/styled'
import ScreenContainerBottom from '@ui/layouts/screen-container-bottom'
import ScreenHeadingV2 from '@ui/components/screen-heading-v2'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@icons/IconLeftHalfArrow'
import { useNavigate } from 'react-router-dom'
import IconCouponPayment from '@ui/components/icons/IconCouponPayment'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  flex-grow: 100;

  gap: 2rem;
  padding: 6rem;
  position: relative;

  & .content {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  & .payment-amount {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    & .text {
      font-weight: 400;
      font-style: Regular;
      font-size: 1.625rem;
      line-height: 2rem;
      color: var(--primary-light);
    }

    & .amount {
      font-weight: 700;
      font-style: Bold;
      font-size: 3rem;
      line-height: 3.5rem;
      color: white;
    }
  }

  & .action {
    padding-top: 2rem;
  }
`

const CouponPaymentScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  return (
    <ScreenContainerBottom>
      <Container onClick={() => navigate('/parcel-locker/check-coupon')}>
        <div className="content">
          <ScreenHeadingV2
            top={() => <IconCouponPayment />}
            middle={t('Coupon payment')}
            bottom={() => (
              <>{t('To complete the payment, please scan the QR code from your coupon.')}</>
            )}
          />

          <div className="payment-amount">
            <div className="text">{t('Payment amount:')}</div>
            <div className="amount">$5,00</div>
          </div>

          <div className="action">
            <CircleButton
              size="l"
              color="inverted"
              icon={(props) => <IconLeftHalfArrow {...props} />}
              textRight={t('Back')}
              disabled={false}
              onClick={() => navigate(-1)}
            />
          </div>
        </div>
      </Container>
    </ScreenContainerBottom>
  )
}

export default CouponPaymentScreen
