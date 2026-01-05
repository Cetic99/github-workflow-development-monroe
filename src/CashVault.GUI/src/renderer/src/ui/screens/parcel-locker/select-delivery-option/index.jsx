/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import SimpleCardV1 from '@ui/components/cards/simple-card-v1'
import IconPerspective from '@icons/IconPerspective'
import IconHome from '@icons/IconHome'
import IconLocation from '@icons/IconLocation'
import { useTranslation } from '@src/app/domain/administration/stores'
import {
  useLockerSize,
  useParcelStoreActions,
  usePostalService
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useDeliveryOptions } from '@src/app/domain/parcel-locker/queries'
import { displayAmountWithCurrency } from '@src/app/domain/parcel-locker/services'
import { MaunalAddressDeliveryOption } from '@src/app/domain/parcel-locker/constants'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .cards {
    padding: 2rem 0 0 0;
    display: grid;
    grid-template-columns: 320px 320px;
    row-gap: 1.5rem;
    column-gap: 1.5rem;
  }
`

const Icons = [IconPerspective, IconHome, IconLocation]

const SelectDeliveryOptionScreen = () => {
  const navigate = useNavigate()

  const postalService = usePostalService()
  const lockerSize = useLockerSize()

  const { setDeliveryOption } = useParcelStoreActions()

  const { t } = useTranslation()

  const { data: deliveryOptions, isLoading } = useDeliveryOptions({
    postalService,
    lockerSize: lockerSize?.code
  })

  const actions = {
    onBack: () => navigate(-1)
  }

  const handleSelectOption = (option) => {
    setDeliveryOption(option)

    if (MaunalAddressDeliveryOption === option?.code)
      navigate('/parcel-locker/enter-recipient-details')
    else navigate('/parcel-locker/enter-parcel-address')
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <>{t('SEND PARCEL')}</>}
          middle={t('Delivery options')}
          bottom={() => (
            <>
              {/* TODO */}
              <span>{t('You selected the ')}</span>
              <span>
                <strong>{`${lockerSize?.code} ${t('size')}`}</strong>
                {` ${t('parcel')} ${lockerSize?.description}.`}
              </span>
              <span>{t('Where do you want to deliver the parcel?')}</span>
            </>
          )}
        />

        {deliveryOptions && deliveryOptions?.length > 0 && (
          <div className="cards">
            {deliveryOptions?.map((x, i) => (
              <SimpleCardV1
                key={`delivery-option-${i}`}
                text={t(x.code)}
                icon={Icons?.at(i)}
                tag={
                  x?.paymentRequired === false
                    ? null
                    : displayAmountWithCurrency(x?.amount, x?.currency)
                }
                onClick={() => handleSelectOption(x)}
              />
            ))}
          </div>
        )}
      </Container>
    </ScreenContainerTop>
  )
}

export default SelectDeliveryOptionScreen
