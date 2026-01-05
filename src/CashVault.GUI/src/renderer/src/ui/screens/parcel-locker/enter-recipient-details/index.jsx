/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import TextInput from '@ui/components/inputs/text-input'
import { useTranslation } from '@src/app/domain/administration/stores'
import { useState } from 'react'
import {
  useDeliveryOption,
  useParcelStoreActions,
  usePostalService,
  useRecipientData
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { MaunalAddressDeliveryOption } from '@src/app/domain/parcel-locker/constants'
import SelectedMapMarker from '@ui/components/selected-map-marker'
import selectedIconUrl from '@ui/assets/images/parcel-locker/map-marker-selected.svg'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4rem;
  padding: 4rem 2rem 6rem 2rem;

  & .form {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  & .row {
  }

  & .row-2 {
    display: grid;
    grid-template-columns: 1fr 1fr;
    column-gap: 1.5rem;
  }
`

const EnterRecipientDetailsScreen = () => {
  const navigate = useNavigate()

  const { t } = useTranslation()

  const recipientDataFromStore = useRecipientData()
  const deliveryOption = useDeliveryOption()
  const { setRecipientData } = useParcelStoreActions()
  const postalService = usePostalService()

  const [recipient, setRecipient] = useState(recipientDataFromStore)

  const showAddressInput = MaunalAddressDeliveryOption === deliveryOption?.code

  const updateRecepientData = (field, value) => {
    setRecipient((prevValue) => {
      return {
        ...prevValue,
        [field]: value
      }
    })
  }

  const handleNextStep = () => {
    setRecipientData(recipient)
    navigate('/parcel-locker/enter-sender-phone-number')
  }

  const actions = {
    onBack: () => navigate(-1),
    onProceed: handleNextStep
  }

  return (
    <>
      <ScreenContainerTop actions={actions}>
        <Container>
          <ScreenHeading
            bottom={() => <>{t('Please enter the recipient’s name and address.')}</>}
          />

          <div className="form">
            <div className="row row-2">
              <TextInput
                size="m"
                label={t('First name')}
                value={recipient?.firstName}
                onChange={({ target }) => updateRecepientData('firstName', target.value)}
              />
              <TextInput
                size="m"
                label={t('Last name')}
                value={recipient?.lastName}
                onChange={({ target }) => updateRecepientData('lastName', target.value)}
              />
            </div>

            {showAddressInput && (
              <>
                <div className="row">
                  <TextInput
                    size="m"
                    label={t('Address')}
                    value={recipient?.address}
                    //disabled={showAddressInput}
                    onChange={({ target }) => updateRecepientData('address', target.value)}
                  />
                </div>

                <div className="row row-2">
                  <TextInput
                    size="m"
                    label={t('Postal code')}
                    value={recipient?.postalCode}
                    //disabled={showAddressInput}
                    onChange={({ target }) => updateRecepientData('postalCode', target.value)}
                  />
                  <TextInput
                    size="m"
                    label={t('City')}
                    value={recipient?.city}
                    //disabled={showAddressInput}
                    onChange={({ target }) => updateRecepientData('city', target.value)}
                  />
                </div>
              </>
            )}
          </div>
        </Container>
      </ScreenContainerTop>

      {!showAddressInput && (
        <SelectedMapMarker
          selectedMapMarker={{
            postalService,
            locationType: t(deliveryOption?.code) || deliveryOption?.name,
            streetName: recipient?.address,
            postalCode: recipient?.postalCode,
            city: recipient?.city
          }}
          showCloseButton={false}
          iconUrl={selectedIconUrl}
          customFooterActions={
            <>
              <CircleButton
                size="l"
                color="medium"
                icon={(props) => <IconLeftHalfArrow {...props} />}
                textRight={t('Back')}
                onClick={() => navigate(-1)}
              />
              <CircleButton
                size="l"
                icon={(props) => <IconRightHalfArrow {...props} />}
                textRight={t('Accept')}
                onClick={handleNextStep}
              />
            </>
          }
        />
      )}
    </>
  )
}

export default EnterRecipientDetailsScreen
