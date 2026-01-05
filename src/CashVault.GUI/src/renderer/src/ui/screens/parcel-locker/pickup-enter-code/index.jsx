/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate, useParams } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import NumericKeyboard from '@ui/components/numeric-keyboard'
import { useTranslation } from '@src/app/domain/administration/stores'
import { useParcelStoreActions } from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useEffect } from 'react'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;
`

const PickupEnterCodeScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const { admin } = useParams()

  const { setShipmentAccessCode, resetShipmentData } = useParcelStoreActions()

  const actions = {
    onBack: () => navigate(-1),
    onProceed: () => navigate('/parcel-locker/processing/pickup-order/Processing your order...')
  }

  // on every enter, pickup code in store should be reset
  useEffect(() => {
    resetShipmentData()
  }, [])

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <>{t('PICK UP PARCEL')}</>}
          middle={t('Enter code')}
          bottom={() => <>{t('Please enter the PIN code sent to you via mobile phone.')}</>}
        />

        <NumericKeyboard maxLength={7} onChange={setShipmentAccessCode} />
      </Container>
    </ScreenContainerTop>
  )
}

export default PickupEnterCodeScreen
