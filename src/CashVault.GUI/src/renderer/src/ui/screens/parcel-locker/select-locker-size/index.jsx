/* eslint-disable prettier/prettier */
/* eslint-disable react/jsx-key */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import SimpleCardV2 from '@ui/components/cards/simple-card-v2'
import { useTranslation } from '@src/app/domain/administration/stores'
import {
  useParcelStoreActions,
  usePostalService
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useParcelLockerSizes } from '@src/app/domain/parcel-locker/queries'

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

const SelectLockerSizeScreen = () => {
  const navigate = useNavigate()
  const postalService = usePostalService()

  const { setLockerSize } = useParcelStoreActions()

  const { t } = useTranslation()

  const { data: lockerSizes, isLoading } = useParcelLockerSizes({ postalService })

  const actions = {
    onBack: () => navigate(-1)
  }

  const handleSelectSize = (size) => {
    setLockerSize(size)
    navigate('/parcel-locker/select-delivery-option')
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <>{t('SEND PARCEL')}</>}
          middle={t('Select locker size')}
          bottom={() => (
            <>
              {t(
                'Please select a locker of suitable size. If your parcel is equal dimensions as the locker, pick a bigger locker. Parcels must not be folded or pushed into a locker by force.'
              )}
            </>
          )}
        />

        {lockerSizes && lockerSizes?.length > 0 && (
          <div className="cards">
            {lockerSizes.map((x) => (
              <SimpleCardV2
                primaryText={x.name}
                secondaryText={x.description}
                onClick={() => handleSelectSize(x)}
              />
            ))}
          </div>
        )}
      </Container>
    </ScreenContainerTop>
  )
}

export default SelectLockerSizeScreen
