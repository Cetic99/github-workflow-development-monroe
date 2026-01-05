/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import Image from '@ui/assets/images/parcel-locker/scan-parcel.png'
import { useTranslation } from '@src/app/domain/administration/stores'
import { useEffect, useRef, useState } from 'react'
import { usePickupShipment } from '@src/app/domain/parcel-locker/commands'
import {
  useCourier,
  useParcelStoreActions,
  usePickupData,
  usePostalService
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { generateBarcode } from '@src/app/domain/parcel-locker/services'
import NumericKeyboard from '@ui/components/numeric-keyboard'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@icons/IconLeftHalfArrow'
import IconRightHalfArrow from '@icons/IconRightHalfArrow'
import IconQrCode from '@icons/IconQrCode'
import IconKeyboardSettings from '@icons/IconKeyboardSettings'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 2rem 2rem;
  height: 100%;

  & .img-container {
    padding: 2rem 0 0 0;
  }
`

const Actions = styled.div`
  margin-top: auto;
  display: flex;
  gap: 4rem;
  padding-bottom: 2.5rem;
  padding-top: 1rem;
  justify-content: space-between;
  width: 100%;

  position: sticky;
  bottom: 0;
  z-index: 10;

  > div {
    min-width: 17rem;
  }
`

const GetParcelScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const [screen, setScreen] = useState('barcode')

  const postalService = usePostalService()
  const courier = useCourier()
  const pickupData = usePickupData()

  const { setShipmentBarcode, setShipmentAccessCode } = useParcelStoreActions()

  const { mutate: pickupMutate } = usePickupShipment(
    (data) => {
      if (data?.isOpen === true) {
        navigate(`/parcel-locker/take-parcel/admin/${data?.parcelLockerId}`)
      } else {
        const searchParams = {
          heading: t('Parcel locker cannot open'),
          description: t(
            'We are sorry, but we can’t open your locker at the moment. Please try again later.'
          ),
          btn1Text: t('Back to shipments'),
          btn1Path: '/parcel-locker/shipments'
        }
        navigate(`/parcel-locker/error?${new URLSearchParams(searchParams).toString()}`)
      }
    },
    () => {
      const searchParams = {
        heading: t('Error saving data'),
        description: t('We are sorry, error occurred. Please, try again.'),
        btn1Text: t('Back to shipments'),
        btn1Path: '/parcel-locker/shipments'
      }
      navigate(`/parcel-locker/error?${new URLSearchParams(searchParams).toString()}`)
    }
  )

  /* for test */
  const formRef = useRef()
  const textInputRef = useRef()

  const handleChangeScreen = () => {
    if (screen === 'code') {
      setScreen('barcode')
      return
    }

    setScreen('code')
  }

  const scanCodeMock = () => {
    formRef?.current?.requestSubmit()
  }

  const handleSubmit = (e) => {
    e.preventDefault()

    const formData = new FormData(e.currentTarget)
    const code = formData.get('barcode') || generateBarcode()

    setShipmentBarcode(code)
    pickupMutate({ barcode: code, postalService, courierId: courier.id })

    e.currentTarget.reset()
  }

  const handleSubmitAccessCode = (e) => {
    e.preventDefault()
    pickupMutate({ accessCode: pickupData.accessCode, postalService, courierId: courier.id })
  }

  useEffect(() => {
    if (textInputRef.current) {
      textInputRef.current.focus()
    }
  }, [])

  return (
    <ScreenContainerTop actions={{}}>
      <Container>
        <ScreenHeading
          middle={t('Please scan or enter the parcel code')}
          bottom={() => <>{t('Please scan or enter the code of the parcel.')}</>}
        />
        {(!screen || screen === 'barcode') && (
          <>
            <div className="img-container">
              <img src={Image} onClick={scanCodeMock} />
            </div>

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
          </>
        )}
        {screen === 'code' && (
          <>
            <NumericKeyboard maxLength={7} onChange={setShipmentAccessCode} />
          </>
        )}

        <Actions>
          {!screen || screen === 'barcode' ? (
            <CircleButton
              size="l"
              color="medium"
              icon={(props) => <IconLeftHalfArrow {...props} />}
              textRight={t('Back')}
              onClick={() => navigate(-1)}
              shadow={true}
            />
          ) : (
            <CircleButton
              size="l"
              color="medium"
              icon={(props) => <IconQrCode {...props} />}
              textRight={t('Scan code')}
              onClick={handleChangeScreen}
              shadow={true}
            />
          )}
          {!screen || screen === 'barcode' ? (
            <CircleButton
              size="l"
              icon={(props) => <IconKeyboardSettings {...props} />}
              textRight={t('Enter code')}
              onClick={handleChangeScreen}
              shadow={true}
            />
          ) : (
            <CircleButton
              size="l"
              icon={(props) => <IconRightHalfArrow {...props} />}
              textRight={t('Proceed')}
              onClick={handleSubmitAccessCode}
              shadow={true}
            />
          )}
        </Actions>
      </Container>
    </ScreenContainerTop>
  )
}

export default GetParcelScreen
