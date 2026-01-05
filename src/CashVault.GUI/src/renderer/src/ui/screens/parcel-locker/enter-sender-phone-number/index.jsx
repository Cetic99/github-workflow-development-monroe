/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import NumericKeyboard from '@ui/components/numeric-keyboard'
import { useTranslation } from '@src/app/domain/administration/stores'
import {
  useParcelStoreActions,
  useSenderData
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useState } from 'react'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;
`

const EnterSenderPhoneNumberScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const senderData = useSenderData()
  const { setSenderData } = useParcelStoreActions()

  const [phoneNumber, setPhoneNumber] = useState(senderData?.phoneNumber)

  const handleNextStep = () => {
    setSenderData({
      ...senderData,
      phoneNumber: phoneNumber
    })

    navigate('/parcel-locker/enter-recipient-phone-number')
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
          middle={t('Your phone number')}
          bottom={() => <>{t('Please enter your mobile phone number.')}</>}
        />

        <NumericKeyboard prefix={387} defaultValue={phoneNumber} onChange={setPhoneNumber} />
      </Container>
    </ScreenContainerTop>
  )
}

export default EnterSenderPhoneNumberScreen
