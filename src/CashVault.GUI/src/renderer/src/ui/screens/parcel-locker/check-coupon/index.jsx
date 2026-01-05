import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import styled from '@emotion/styled'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  padding: 4rem 2rem 6rem 2rem;

  h3 {
    font-size: 1.5rem;
    text-transform: uppercase;
    color: var(--secondary-dark);
    font-weight: 600;
    margin-bottom: 1.5rem;
  }

  h1 {
    font-size: 4.375rem;
    color: #020605;
    font-weight: 700;
    margin-bottom: 1.5rem;
  }

  p {
    font-size: 1.625rem;
    font-weight: normal;
    color: black;
    margin-bottom: 3rem;
  }

  & .amount {
    display: flex;
    justify-content: space-between;
    align-items: center;

    & .divider {
      height: 6.875rem;
      width: 1px;
      background-color: #ccd3c7;
    }

    & .payment-amount {
      h2 {
        font-size: 1.563rem;
        font-weight: normal;
        margin-bottom: 1rem;
      }

      span {
        font-size: 3rem;
        color: var(--secondary-dark);
        font-weight: 700;
      }
    }
  }
`

const Outer = styled.div`
  padding: 4rem 2rem 6rem 2rem;

  h2 {
    font-size: 3.438rem;
    line-height: 3.438rem;
    font-weight: 700;
  }
`

const CheckCouponScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()
  const actions = {
    onBack: () => navigate(-1),
    onProceed: () =>
      navigate('/parcel-locker/processing/pickup-payment/We are preparing your package...')
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container
        onClick={() =>
          navigate('/parcel-locker/processing/pickup-payment/We are preparing your package...')
        }
      >
        <h3>{t('Pick up parcel')}</h3>
        <h1>{t('Coupon payment')}</h1>
        <p>
          {t('Your ticket exceeds the amount needed for payment.')}
          <br />
          {t('We can issue a ticket for the remaining amount.')}
        </p>

        <div className="amount">
          <div className="payment-amount">
            <h2>{t('Payment amount:')}</h2>
            <span>€5.00</span>
          </div>
          <div className="divider"></div>
          <div className="payment-amount">
            <h2>{t('Coupon amount:')}</h2>
            <span>€5.00</span>
          </div>
        </div>
      </Container>
      <Outer>
        <h2>{t('Would you like to continue and print the ticket as change?')}</h2>
      </Outer>
    </ScreenContainerTop>
  )
}

export default CheckCouponScreen
