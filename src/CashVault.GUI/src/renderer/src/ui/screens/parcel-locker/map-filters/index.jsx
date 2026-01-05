/* eslint-disable prettier/prettier */
import { useNavigate } from 'react-router-dom'
import styled from '@emotion/styled'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import Button from '@ui/components/button'
import IconCheckCircle from '@ui/components/icons/IconCheckCircle'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import { Fragment, useState } from 'react'
import {
  useLocationTypes,
  useMapFilters,
  useParcelStoreActions,
  usePostalServices
} from '@src/app/domain/parcel-locker/stores/parcel-store'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4rem;
  padding: 4rem 2rem 6rem 2rem;

  h1 {
    font-size: 4.375rem;
    font-weight: bold;
  }

  h2 {
    font-size: 1.75rem;
    font-weight: 600;
  }

  & .providers {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  & .checks {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 2.5rem;
    justify-content: space-between;
  }
`

const MapFiltersScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const providers = usePostalServices()
  const locationTypes = useLocationTypes()
  const mapFilters = useMapFilters()

  const [selectedFilters, setSelectedFilters] = useState(mapFilters)

  const { setMapFilters } = useParcelStoreActions()

  const actions = {
    onBack: () => navigate(-1),
    onProceed: () => {
      setMapFilters(selectedFilters)
      navigate(-1)
    }
  }

  const isSelected = (accesor, codeValue) => {
    const idx = selectedFilters?.[accesor]?.findIndex((x) => x === codeValue)

    return idx > -1
  }

  const allSelected = (accessor) => {
    if (accessor === 'providers') {
      return selectedFilters[accessor]?.length === providers.length
    } else if (accessor === 'locationTypes') {
      return selectedFilters[accessor]?.length === locationTypes.length
    }
  }

  const onFilterChange = (accessor, code, add = true) => {
    const existing = selectedFilters[accessor] || []

    let updated

    if (add) {
      updated = [...existing, code]
    } else {
      updated = existing.filter((x) => x !== code)
    }

    setSelectedFilters((prev) => ({
      ...prev,
      [accessor]: updated
    }))
  }

  const onSelectAll = (accessor) => {
    if (allSelected(accessor)) {
      setSelectedFilters((prev) => ({
        ...prev,
        [accessor]: []
      }))
    } else {
      let items = []

      if (accessor === 'providers') items = providers.map((x) => x.code)
      else if (accessor === 'locationTypes') items = locationTypes.map((x) => x.code)

      setSelectedFilters((prev) => ({
        ...prev,
        [accessor]: items
      }))
    }
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <h1>{t('Filters')}</h1>
        <div className="providers">
          <h2>{t('Providers')}</h2>
          <Button
            icon={(props) => <IconCheckCircle {...props} />}
            onClick={() => onSelectAll('providers')}
          >
            {allSelected('providers') ? t('Deselect all') : t('Select all')}
          </Button>
        </div>
        <div className="checks">
          {/* [MonroeParcelLocker, ...providers] */}
          {providers.map((provider, index) => (
            <Fragment key={provider.code}>
              <CheckboxInput
                label={provider.name}
                value={isSelected('providers', provider.code)}
                onChange={({ target }) => {
                  onFilterChange('providers', provider.code, target.checked)
                }}
              />
              {(index + 1) % 2 === 0 && (
                <div
                  style={{
                    gridColumn: '1 / -1',
                    height: '1px',
                    backgroundColor: '#0E7465',
                    margin: '1rem 0'
                  }}
                />
              )}
            </Fragment>
          ))}
        </div>

        <div className="providers">
          <h2>{t('Location type')}</h2>
          <Button
            icon={(props) => <IconCheckCircle {...props} />}
            onClick={() => onSelectAll('locationTypes')}
          >
            {allSelected('locationTypes') ? t('Deselect all') : t('Select all')}
          </Button>
        </div>
        <div className="checks">
          {locationTypes?.map((location, index) => (
            <Fragment key={location.code}>
              <CheckboxInput
                label={t(location?.name)}
                value={isSelected('locationTypes', location.code)}
                onChange={({ target }) => {
                  onFilterChange('locationTypes', location.code, target.checked)
                }}
              />
              {(index + 1) % 2 === 0 && (
                <div
                  style={{
                    gridColumn: '1 / -1',
                    height: '1px',
                    backgroundColor: '#0E7465',
                    margin: '1rem 0'
                  }}
                />
              )}
            </Fragment>
          ))}
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default MapFiltersScreen
