/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate, useParams } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import ImageCardV2 from '@ui/components/cards/image-card-v2'
import IconDeliveryTruck from '@icons/IconDeliveryTruck'
import FullPageLoader from '@ui/components/full-page-loader'
import {
  useParcelStoreActions,
  usePostalService,
  usePostalServices
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { ParcelLockerMethod } from '@src/app/domain/parcel-locker/constants'
import { useTranslation } from '@src/app/domain/administration/stores'
import PostalServiceImage from '@ui/components/images/postal-service-image'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .cards {
    padding: 2rem 0 0 0;
    display: flex;
    gap: 1.5rem;
    flex-wrap: nowrap;
    overflow-x: hidden;
  }
`

const SelectCourierScreen = () => {
  const navigate = useNavigate()
  const { method } = useParams()

  const { t } = useTranslation()

  const data = usePostalServices()

  //const selectedMethod = useMethod()
  const postalService = usePostalService()

  const { setPostalService } = useParcelStoreActions()

  const actions = {
    onBack: () => navigate(-1),
    onProceed: () => {
      if (method == ParcelLockerMethod.SEND) navigate('/parcel-locker/select-locker-size')
      if (method == ParcelLockerMethod.PICKUP) navigate('/parcel-locker/select-pickup-method')
      if (method == 'operator-code') navigate('/parcel-locker/operator-code')
    }
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <IconDeliveryTruck size="xl" />}
          middle={t('Please select the courier company')}
        />

        {/* Style for this loader should be fixed or we should use different loader here */}
        {/* <FullPageLoader loading={isLoading} /> */}

        {data?.length > 0 && (
          <div className="cards">
            {data.map((x, i) => (
              <ImageCardV2
                key={`parcel-locker-courier-${i}`}
                active={postalService === x.code}
                onClick={() => setPostalService(x.code)}
                text={x.name}
                image={() => <PostalServiceImage code={x.code} />}
              />
            ))}
          </div>
        )}
      </Container>
    </ScreenContainerTop>
  )
}

export default SelectCourierScreen
