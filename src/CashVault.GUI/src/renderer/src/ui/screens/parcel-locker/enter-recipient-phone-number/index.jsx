/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import NumericKeyboard from '@ui/components/numeric-keyboard'
import { useTranslation } from '@src/app/domain/administration/stores'
import {
  useParcelStoreActions,
  useRecipientData
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useState } from 'react'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;
`

const EnterRecipientPhoneNumberScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const recipientData = useRecipientData()
  const { setRecipientData } = useParcelStoreActions()

  const [phoneNumber, setPhoneNumber] = useState(recipientData?.phoneNumber)

  const handleNextStep = () => {
    setRecipientData({
      ...recipientData,
      phoneNumber: phoneNumber
    })

    navigate('/parcel-locker/send-select-payment-options')

    // TODO
    //if (deliveryOption?.paymentRequired === true) {
    //  // we need to nagivate to payment options only if delivery option requires payment
    //  navigate('/parcel-locker/send-select-payment-options')
    //}
  }

  const actions = {
    onBack: () => navigate(-1),
    onProceed: handleNextStep
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <>{t('SEND PARCEL')}</>}
          middle={t('Recipient phone number')}
          bottom={() => <>{t('Please enter recipient’s mobile phone number.')}</>}
        />

        <NumericKeyboard prefix={387} onChange={setPhoneNumber} defaultValue={phoneNumber} />
      </Container>
    </ScreenContainerTop>
  )
}

export default EnterRecipientPhoneNumberScreen
