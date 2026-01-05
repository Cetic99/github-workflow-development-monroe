/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import { useState } from 'react'
import MapSelection from '@ui/components/map-selection'
import selectedIconUrl from '@ui/assets/images/parcel-locker/map-marker-selected.svg'
import {
  useMapFilters,
  useParcelStoreActions,
  usePostalService
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useSearchPostalServiceAddresses } from '@src/app/domain/parcel-locker/queries'
import { isNull, isUndefined } from 'lodash'
import SelectedMapMarker from '@ui/components/selected-map-marker'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4rem;
  padding: 4rem 2rem 6rem 2rem;

  & .tabs {
    display: flex;
    gap: 1.5rem;

    & button {
      flex-grow: 100;
    }
  }

  .map-container {
    position: absolute;
    top: 0;
    left: 0;
    padding: 0;
    z-index: 10;
    height: 70vh;
    width: 100%;
    & > * {
      height: 70vh;
      width: 100%;
    }
  }

  & .map-outer-container {
    position: relative;
    display: flex;
    justify-content: center;
    align-items: center;

    & .search-field {
      position: absolute;
      top: 5rem;
      z-index: 15;
      width: 70%;

      & .search-input > * {
        background-color: white !important;

        & input {
          background-color: white !important;
        }
      }
    }

    & .search-results {
      margin-top: 1rem;
    }
  }
`

const EnterParcelAddressScreen = () => {
  const navigate = useNavigate()

  const postalService = usePostalService()
  const mapFilters = useMapFilters()

  const { setRecipientData, setMapFilters } = useParcelStoreActions()

  //const [searchParams] = useSearchParams()

  const [queryText, setQueryText] = useState(null)

  const { data } = useSearchPostalServiceAddresses({
    postalServices: [...(mapFilters?.providers || []), postalService],
    query: queryText,
    locationTypes: mapFilters?.locationTypes || []
  })

  // default je 0
  //const [active, setActive] = useState(0)
  const [selectedMapMarker, setSelectedMapMarker] = useState(null)

  // kad se mounta komponenta, pogledaj query param
  //useEffect(() => {
  //  const activeParam = searchParams.get('active')
  //  if (activeParam === 'true' || activeParam === '1') {
  //    setActive(1)
  //  } else if (activeParam === 'false' || activeParam === '0') {
  //    setActive(0)
  //  }
  //}, [searchParams])

  const handleNextStep = () => {
    setRecipientData({
      address: `${selectedMapMarker?.streetName} ${selectedMapMarker?.streetNumber}`,
      postalCode: selectedMapMarker?.postalCode,
      city: selectedMapMarker?.city,
      locationType: selectedMapMarker?.locationType
    })
    setMapFilters({
      providers: [],
      locationTypes: []
    })

    navigate('/parcel-locker/enter-recipient-details')
  }

  const actions = {
    onBack: () => navigate(-1),
    onFilter: () => navigate('/parcel-locker/map-filters')
    //onProceed: () => navigate('/parcel-locker/enter-recipient-details')
  }

  return (
    <>
      <ScreenContainerTop actions={actions}>
        <Container /*active={active}*/>
          {/* <div className="tabs">
            <Button size="l" color={active === 0 ? 'dark' : 'gray'} onClick={() => setActive(0)}>
              {t('Find address')}
            </Button>
            <Button size="l" color={active === 1 ? 'dark' : 'gray'} onClick={() => setActive(1)}>
              {t('Find on map')}
            </Button>
          </div> */}

          {/* {active === 0 ? (
            <div className="search-container">
              <div className="search-field">
                <TextInput size="m" placeholder="Search address..." />
              </div>
              <div className="search-results">
                <BarLoader loading={isLoading} />
                {!isLoading && (!data || data?.length == 0) && (
                  <div className="empty-search-results">{t('No results')}</div>
                )}
                {!isLoading && data?.length > 0 && (
                  <>
                    {data?.map((x, i) => (
                      <div key={`map-search-result-${i}`}>{x.city}</div>
                    ))}
                  </>
                )}
              </div>
            </div>
          ) : (
            <MapSelection selected={selectedMapMarker} setSelected={setSelectedMapMarker} />
          )} */}

          <MapSelection
            selected={selectedMapMarker}
            setSelected={setSelectedMapMarker}
            data={data}
            searchText={queryText}
            onChangeSearchText={setQueryText}
          />
        </Container>
      </ScreenContainerTop>

      {!isUndefined(selectedMapMarker) && !isNull(selectedMapMarker) && (
        <SelectedMapMarker
          selectedMapMarker={selectedMapMarker}
          onClose={() => setSelectedMapMarker(null)}
          onAccept={handleNextStep}
          iconUrl={selectedIconUrl}
        />
      )}
    </>
  )
}

export default EnterParcelAddressScreen
