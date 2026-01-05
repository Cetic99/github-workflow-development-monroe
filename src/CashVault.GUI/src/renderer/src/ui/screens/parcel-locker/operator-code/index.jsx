/* eslint-disable prettier/prettier */
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import ImagePhoneScanQR from '@icons/ImagePhoneScanQR'
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@src/app/domain/administration/stores'
import { useEffect, useRef } from 'react'
import {
  useParcelStoreActions,
  usePostalService
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useVerifyCourier } from '@src/app/domain/parcel-locker/commands'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .img-container {
    padding: 2rem 0 0 0;
  }
`

const OperatorCodeScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  /* for test */
  const formRef = useRef()
  const textInputRef = useRef()

  const postalService = usePostalService()
  const { setCourier } = useParcelStoreActions()

  const { mutate: verifyMutate } = useVerifyCourier(
    (data) => {
      setCourier(data?.data)
      navigate('/parcel-locker/shipments')
    },
    () => {}
  )

  const actions = {
    onBack: () => navigate(-1),
    onLoginAdmin: () => navigate('/parcel-locker/admin-login')
  }

  useEffect(() => {
    if (textInputRef.current) {
      textInputRef.current.focus()
    }
  }, [])

  const scanCodeMock = () => {
    formRef?.current?.requestSubmit()
  }

  const handleSubmit = (e) => {
    e.preventDefault()

    const formData = new FormData(e.currentTarget)
    const code = formData.get('barcode') || '123456789'

    verifyMutate({ barcode: code, postalService })

    e.currentTarget.reset()
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container onClick={scanCodeMock}>
        <ScreenHeading
          middle={t('Please scan your code to log in')}
          bottom={() => <>{t('Please scan your operator code.')}</>}
        />

        <div className="pts-content">
          <form onSubmit={handleSubmit} ref={formRef}>
            <input
              type="text"
              name="barcode"
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

export default OperatorCodeScreen
