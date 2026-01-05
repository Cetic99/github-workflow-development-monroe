/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate, useParams } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import ImagePhoneScanQR from '@icons/ImagePhoneScanQR'
import { useTranslation } from '@src/app/domain/administration/stores'
import { useRef } from 'react'
import { useParcelStoreActions } from '@src/app/domain/parcel-locker/stores/parcel-store'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .img-container {
    padding: 2rem 0 0 0;
  }

  & form {
    & button {
      display: none;
    }
  }
`

const PickupScanQrScreen = () => {
  const params = useParams()
  const { admin } = params

  const navigate = useNavigate()
  const { t } = useTranslation()

  const textInputRef = useRef()
  const { setShipmentBarcode } = useParcelStoreActions()

  /* for test */
  const formRef = useRef()

  const actions = {
    onBack: () => navigate(-1)
  }

  const scanCodeMock = () => {
    formRef?.current?.requestSubmit()
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const code = formData.get('qrCode') || 'MOCK-QR-CODE-1234'

    setShipmentBarcode(code)
    e.currentTarget.reset()

    //if (admin) {
    //  navigate(`/parcel-locker/take-parcel/${admin}`)
    //} else {
    //  navigate('/parcel-locker/processing/pickup-order/Processing your order...')
    //}
    navigate('/parcel-locker/processing/pickup-order/Processing your order...')
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container onClick={scanCodeMock}>
        <ScreenHeading
          top={() => <>{t('PICK UP PARCEL')}</>}
          middle={t('Please scan the QR code')}
          bottom={() => <>{t('Please scan the code from your mobile device.')}</>}
        />

        <div className="pts-content">
          <form onSubmit={handleSubmit} ref={formRef}>
            <input
              type="text"
              name="qrCode"
              autoComplete="off"
              inputMode="none"
              ref={textInputRef}
              style={{ opacity: 0, position: 'absolute', pointerEvents: 'none' }}
            />
            <button type="submit"></button>
          </form>
        </div>

        <div className="img-container">
          <ImagePhoneScanQR />
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default PickupScanQrScreen
