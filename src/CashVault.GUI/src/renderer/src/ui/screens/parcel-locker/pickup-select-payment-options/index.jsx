/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import SimpleCardV1 from '@ui/components/cards/simple-card-v1'
import IconBanknote01 from '@icons/IconBanknote01'
import IconCoupon from '@icons/IconCoupon'
import IconBankCard from '@icons/IconBankCard'
import { useTranslation } from '@src/app/domain/administration/stores'
import { usePaymentInfo } from '@src/app/domain/parcel-locker/stores/parcel-store'
import { displayAmountWithCurrency } from '@src/app/domain/parcel-locker/services'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .cards {
    padding: 2rem 0 0 0;
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  & .row {
    display: flex;
    gap: 1.5rem;
  }
`

const PickupSelectPaymentOptionsScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const paymentInfo = usePaymentInfo()

  const actions = {
    onBack: () => navigate(-1)
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <>{t('PICK UP PARCEL')}</>}
          middle={t('Payment required')}
          bottom={() => (
            <>
              {t('Please select a payment method to pay for this package. Payment amount: ')}
              <span style={{ color: '#0D8472', fontWeight: 'bold' }}>
                {displayAmountWithCurrency(paymentInfo?.amount, paymentInfo?.currency)}
              </span>
            </>
          )}
        />

        <div className="cards">
          <div className="row">
            <SimpleCardV1
              icon={(props) => <IconBankCard {...props} />}
              text={t('Card')}
              onClick={() => navigate('/parcel-locker/card-payment/pickup')}
            />
            <SimpleCardV1
              icon={(props) => <IconCoupon {...props} />}
              text={t('Coupon')}
              onClick={() => navigate('/parcel-locker/coupon-payment')}
            />
          </div>

          <div className="row">
            <SimpleCardV1
              icon={(props) => <IconBanknote01 {...props} />}
              text={t('Cash')}
              onClick={() => navigate('/parcel-locker/cash-payment/pickup')}
            />
          </div>
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default PickupSelectPaymentOptionsScreen
