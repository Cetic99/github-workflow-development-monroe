/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import 'leaflet/dist/leaflet.css'
import L from 'leaflet'
import { MapContainer, Marker, TileLayer } from 'react-leaflet'
import TextInput from '@ui/components/inputs/text-input'
import markerIconUrl from '@ui/assets/images/parcel-locker/map-marker.svg'
import selectedIconUrl from '@ui/assets/images/parcel-locker/map-marker-selected.svg'
import { useTranslation } from '@src/app/domain/administration/stores'

const position = [51.505, -0.09]

const unSelectedIcon = L.icon({
  iconUrl: markerIconUrl,
  iconSize: [32, 32],
  iconAnchor: [16, 32],
  popupAnchor: [0, -32]
})

const selectedIcon = L.icon({
  iconUrl: selectedIconUrl,
  iconSize: [32, 32],
  iconAnchor: [16, 32],
  popupAnchor: [0, -32]
})

const MapSelection = ({ setSelected, selected, data, searchText, onChangeSearchText }) => {
  const { t } = useTranslation()

  return (
    <div className="map-outer-container">
      <div className="search-field">
        <TextInput
          size="m"
          placeholder={t('Tap here to search...')}
          value={searchText}
          className="search-input"
          onChange={({ target }) => onChangeSearchText(target?.value)}
        />
      </div>
      <div className="map-container">
        <MapContainer center={position} zoom={75} scrollWheelZoom={false}>
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <>
            {data?.map((location, index) => (
              <Marker
                key={`address-marker-${index}`}
                position={[location?.latitude, location?.longitude]}
                icon={selected?.uuid === location?.uuid ? selectedIcon : unSelectedIcon}
                eventHandlers={{
                  click: () => setSelected(location)
                }}
              />
            ))}
          </>
        </MapContainer>
      </div>
    </div>
  )
}

export default MapSelection
