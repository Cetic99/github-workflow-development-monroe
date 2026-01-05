/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import { useTranslation } from '@src/app/domain/administration/stores'
import {
  useLockerSize,
  usePostalServiceData,
  useRecipientData
} from '@src/app/domain/parcel-locker/stores/parcel-store'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4rem;
  padding: 4rem 2rem 6rem 2rem;

  & .form {
    display: flex;
    flex-direction: column;
  }

  & .form-field {
    display: flex;
    align-items: center;
    border-radius: 10px;
    padding: 1rem;

    & label {
      display: flex;
      align-items: center;
      width: 15rem;
      font-weight: 600;
      font-style: SemiBold;
      font-size: 1rem;
      line-height: 1.25rem;
      text-transform: uppercase;
      color: #6a6a6a;
    }

    & .value {
      font-weight: 400;
      font-style: Regular;
      font-size: 1.625rem;
      line-height: 2rem;
    }
  }

  & .form-field-light {
    background: white;
  }
`

const ConfirmDeliveryScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const recipient = useRecipientData()
  const postalService = usePostalServiceData()
  const lockerSize = useLockerSize()

  const actions = {
    onBack: () => navigate(-1),
    onProceed: () =>
      navigate('/parcel-locker/processing/send-open-locker/We are opening your locker...')
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading top={() => <>{t('SEND PARCEL')}</>} middle={t('Confirm your delivery')} />

        <div className="form">
          <div className="form-field form-field-light">
            <label>{t('Name')}</label>
            <div className="value">{`${recipient?.firstName} ${recipient?.lastName}`}</div>
          </div>

          <div className="form-field">
            <label>{t('Address')}</label>
            <div className="value">{recipient?.address}</div>
          </div>

          <div className="form-field form-field-light">
            <label>{t('Post code')}</label>
            <div className="value">{recipient?.postalCode}</div>
          </div>

          <div className="form-field">
            <label>{t('City')}</label>
            <div className="value">{recipient?.city}</div>
          </div>

          <div className="form-field form-field-light">
            <label>{t('Parcel size')}</label>
            <div className="value">{`${lockerSize?.name} (${lockerSize?.description})`}</div>
          </div>

          <div className="form-field">
            <label>{t('Courier company')}</label>
            <div className="value">{postalService?.name}</div>
          </div>
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default ConfirmDeliveryScreen
