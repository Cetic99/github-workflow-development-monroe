/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import ImagePhoneScanQR from '@icons/ImagePhoneScanQR'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .img-container {
    padding: 2rem 0 0 0;
  }
`

const SendScanQrScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const actions = {
    onBack: () => navigate(-1)
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container onClick={() => navigate('/parcel-locker/confirm-delivery')}>
        <ScreenHeading
          top={() => <>{t('SEND PARCEL')}</>}
          middle={t('Please scan the QR code')}
          bottom={() => (
            <>{t('Please scan the code provided to you in the confirmation document.')}</>
          )}
        />

        <div className="img-container">
          <ImagePhoneScanQR />
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default SendScanQrScreen
