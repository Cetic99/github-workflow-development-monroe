/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import SimpleCardV1 from '@ui/components/cards/simple-card-v1'
import IconBanknote01 from '@icons/IconBanknote01'
import IconBankCard from '@icons/IconBankCard'
import { useTranslation } from '@src/app/domain/administration/stores'

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
    flex-wrap: wrap;
    gap: 1.5rem;
  }
`

const SendSelectPaymentOptionsScreen = () => {
  const navigate = useNavigate()

  const { t } = useTranslation()

  const actions = {
    onBack: () => navigate(-1)
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <>{t('SEND PARCEL')}</>}
          middle={t('Payment options')}
          bottom={() => <>{t('Please select how you want to pay.')}</>}
        />

        <div className="cards">
          <div className="row">
            <SimpleCardV1
              icon={(props) => <IconBankCard {...props} />}
              text={t('Card')}
              onClick={() => navigate('/parcel-locker/card-payment/send')}
            />
            <SimpleCardV1
              icon={(props) => <IconBanknote01 {...props} />}
              text={t('Cash')}
              onClick={() => navigate('/parcel-locker/cash-payment/send')}
            />
          </div>
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default SendSelectPaymentOptionsScreen
