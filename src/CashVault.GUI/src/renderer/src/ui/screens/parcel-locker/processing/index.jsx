/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainerBottom from '@ui/layouts/screen-container-bottom'
import ScreenHeadingV2 from '@ui/components/screen-heading-v2'
import IconSettings from '@icons/IconSettings'
import { useLocation, useNavigate, useParams } from 'react-router-dom'
import { useTranslation } from '@src/app/domain/administration/stores'
import { ProcessingReason } from '@src/app/domain/parcel-locker/constants'
import {
  useCreateShipment,
  useOpenParcelLocker,
  useSendParcelLockerRequest
} from '@src/app/domain/parcel-locker/commands'
import {
  useCourier,
  useDeliveryOption,
  useLockerSize,
  useParcelStoreActions,
  usePickupData,
  usePostalService,
  useRecipientData,
  useSenderData,
  useShipmentDetails
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useEffect, useRef } from 'react'

const Container = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  flex-grow: 100;

  gap: 2rem;
  padding: 6rem;
  position: relative;
`

const ProcessingScreen = () => {
  const { state, text } = useParams()

  const navigate = useNavigate()
  const { t } = useTranslation()

  const pageLoadRef = useRef(false)

  const { state: locationState } = useLocation()

  const postalService = usePostalService()

  const { setPaymentInfo, setShipmentDetails } = useParcelStoreActions()

  /* Pickup */
  const pickupOrderData = usePickupData()
  const courier = useCourier()
  const admin = courier?.id !== null && courier?.id !== undefined && courier?.id?.trim() !== ''

  const { mutate: openLocker } = useOpenParcelLocker(
    (data) => {
      if (data?.isOpen !== true) {
        const searchParams = {
          heading: t('Parcel locker cannot open'),
          description: t(
            'We are sorry, but we can’t open your parcel at the moment because the parcel locker is not available. Please try again later.'
          ),
          btn1Text: t('Back to start'),
          btn1Path: '/#'
        }
        navigate(`/parcel-locker/error?${new URLSearchParams(searchParams).toString()}`)
        return
      }

      if (state == ProcessingReason.PICKUP_ORDER) navigate('/parcel-locker/take-package')
      if (state == ProcessingReason.SEND_OPEN_LOCKER) navigate('/parcel-locker/insert-package')
    },
    () => {
      const searchParams = {
        heading: t('Shipment is not available'),
        description: t('We are sorry, error occurred. Please, try again.'),
        btn1Text: t('Back to start'),
        btn1Path: '/#'
      }
      navigate(`/parcel-locker/error?${new URLSearchParams(searchParams).toString()}`)
    }
  )

  const handleOpenParcelLocker = (data) => {
    openLocker(data)
  }

  const sendPickupRequest = useSendParcelLockerRequest(
    (data) => {
      setPaymentInfo(data?.data)

      if (data?.data?.paymentRequired === false) {
        handleOpenParcelLocker({
          barcode: pickupOrderData?.barcode,
          accessCode: +pickupOrderData?.accessCode
        })
      } else {
        navigate('/parcel-locker/pickup-payment-options')
      }
    },
    ({ details }) => {
      const searchParams = {
        heading: t('Shipment is not available'),
        description: t('We are sorry, shipment is not available. Please try again later.'),
        btn1Text: t('Back to start'),
        btn1Path: '/#'
      }
      navigate(`/parcel-locker/error?${new URLSearchParams(searchParams).toString()}`)
    }
  )

  /* Create shipment */
  const recipientData = useRecipientData()
  const senderData = useSenderData()
  const deliveryOption = useDeliveryOption()
  const parcelLockerSize = useLockerSize()
  const shipmentDetails = useShipmentDetails()

  const createShipmentRequest = useCreateShipment((data) => {
    const shipmentDetails = data?.data

    setShipmentDetails(shipmentDetails)
    handleOpenParcelLocker({
      barcode: shipmentDetails?.barcode,
      accessCode: +shipmentDetails?.lockerAccessCode
    })
  })

  const onPageLoad = () => {
    if (state == ProcessingReason.PICKUP_ORDER) {
      if (locationState?.retry === true) {
        handleOpenParcelLocker({
          barcode: pickupOrderData?.barcode,
          accessCode: +pickupOrderData?.accessCode
        })
      } else {
        sendPickupRequest.mutate({
          postalService: pickupOrderData?.postalService,
          barcode: pickupOrderData?.barcode || '',
          accessCode: +pickupOrderData?.accessCode
        })
      }
    }

    if (state == ProcessingReason.SEND_OPEN_LOCKER) {
      if (locationState?.retry === true) {
        handleOpenParcelLocker(shipmentDetails)
      } else {
        const shipmentData = {
          postalService: postalService,
          deliveryOption: deliveryOption?.code,
          parcelLockerSize: parcelLockerSize?.code,
          location: {
            city: recipientData?.city,
            address: recipientData?.address,
            postalCode: recipientData?.postalCode,
            locationType: recipientData?.locationType
          },
          recipient: {
            firstName: recipientData?.firstName,
            lastName: recipientData?.lastName,
            phoneNumber: recipientData?.phoneNumber
          },
          sender: {
            phoneNumber: senderData?.phoneNumber
          }
        }

        createShipmentRequest.mutate(shipmentData)
      }
    }

    if (state == ProcessingReason.PICKUP_PAYMENT) {
      // TODO
    }
  }

  useEffect(() => {
    if (pageLoadRef?.current === true) return

    pageLoadRef.current = true
    onPageLoad()
  }, [])

  return (
    <ScreenContainerBottom>
      <Container
        onClick={() => {
          if (state == ProcessingReason.PICKUP_ORDER)
            navigate('/parcel-locker/pickup-payment-options')
          if (state == ProcessingReason.PICKUP_PAYMENT) navigate('/parcel-locker/take-package')
          if (state == ProcessingReason.SEND_OPEN_LOCKER) navigate('/parcel-locker/insert-package')
        }}
      >
        <ScreenHeadingV2
          top={() => <IconSettings size="xl" color="var(--primary-light)" />}
          middle={t('One moment, please')}
          bottom={() => <>{t(text) || t('Processing...')}</>}
        />
      </Container>
    </ScreenContainerBottom>
  )
}

export default ProcessingScreen
