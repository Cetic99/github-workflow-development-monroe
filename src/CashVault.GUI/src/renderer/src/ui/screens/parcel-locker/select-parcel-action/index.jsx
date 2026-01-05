/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import ImageCardV1 from '@ui/components/cards/image-card-v1'
import SimpleCardV1 from '@ui/components/cards/simple-card-v1'
import ImagePickupParcel from '@icons/ImagePickupParcel'
import ImageSendParcel from '@icons/ImageSendParcel'
import IconQrCode from '@icons/IconQrCode'
import IconBanknote01 from '@icons/IconBanknote01'
import { ParcelLockerMethod } from '@src/app/domain/parcel-locker/constants'
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
    gap: 1.5rem;
  }
`

const SelectParcelActionScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const actions = {
    onBack: () => navigate(-1)
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          middle={t('welcome')}
          bottom={() => (
            <>
              {t(
                "Please select the desired option. If you have a QR code, select the 'Scan Code' option."
              )}
            </>
          )}
        />

        <div className="cards">
          <div className="row">
            <ImageCardV1
              fullWidth={true}
              text={t('Send Parcel')}
              image={() => <ImageSendParcel />}
              onClick={() => navigate(`/parcel-locker/select-courier/${ParcelLockerMethod.SEND}`)}
            />
            <ImageCardV1
              fullWidth={true}
              text={t('Pick up Parcel')}
              image={() => <ImagePickupParcel />}
              onClick={() => navigate(`/parcel-locker/select-courier/${ParcelLockerMethod.PICKUP}`)}
            />
          </div>

          <div className="row">
            <SimpleCardV1
              fullWidth={true}
              icon={(props) => <IconBanknote01 {...props} />}
              text={t('Cash pickup')}
              onClick={() => alert('Option not active!')}
            />
            <SimpleCardV1
              fullWidth={true}
              icon={(props) => <IconQrCode {...props} />}
              text={t('Scan code')}
              onClick={() => alert('Option not active!')}
            />
          </div>
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default SelectParcelActionScreen
