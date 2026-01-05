/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//===========================================================================

const getPostalServices = async () => {
  const { data } = await api.get(`api/parcel-locker/postal-services`)

  return data
}

export const usePostalServices = () => {
  return useQuery({
    queryKey: ['parcel-locker-postal-services'],
    queryFn: async () => getPostalServices()
  })
}

//===========================================================================

const getParcelLockerSizes = async ({ postalService }) => {
  const { data } = await api.get(`api/parcel-locker/locker-sizes`, {
    params: { postalService }
  })

  return data
}

export const useParcelLockerSizes = ({ postalService }) => {
  return useQuery({
    queryKey: ['parcel-locker-sizes', postalService],
    queryFn: async () => getParcelLockerSizes({ postalService })
  })
}

//===========================================================================

const getDeliveryOptions = async ({ postalService, parcelLockerSize }) => {
  const { data } = await api.get(`api/parcel-locker/delivery-options`, {
    params: { postalService, parcelLockerSize }
  })

  return data
}

export const useDeliveryOptions = ({ postalService, lockerSize }) => {
  return useQuery({
    queryKey: ['parcel-locker-delivery-options', postalService, lockerSize],
    queryFn: async () => getDeliveryOptions({ postalService, parcelLockerSize: lockerSize })
  })
}

//===========================================================================
// params = { postalService, query, locationTypes, forSend, forReceive }
const searchPostalServiceAddresses = async ({ query, postalServices, locationTypes }) => {
  let queryParams = ``

  if (query !== null && query !== undefined && query.trim() !== '') {
    queryParams += `&query=${query.trim()}`
  }

  if (postalServices && postalServices.length > 0) {
    for (const ps of postalServices) {
      queryParams += `&postalService=${ps}`
    }
  }

  if (locationTypes && locationTypes.length > 0) {
    for (const lt of locationTypes) {
      queryParams += `&locationTypes=${lt}`
    }
  }

  const { data } = await api.get(`api/parcel-locker/search/postal-services?${queryParams}`)

  return data
}

export const useSearchPostalServiceAddresses = (params) => {
  return useQuery({
    queryKey: ['postal-service-search-addresses', params],
    queryFn: async () => searchPostalServiceAddresses(params)
  })
}

//===========================================================================

const getLocationTypes = async () => {
  const { data } = await api.get('api/parcel-locker/location-types')

  return data
}

export const useLocationTypes = () => {
  return useQuery({
    queryKey: ['parcel-locker-location-types'],
    queryFn: async () => getLocationTypes()
  })
}

//===========================================================================

const getShipmentsInLockers = async ({ courierId, postalService }) => {
  const params = {
    params: {
      postalService
    }
  }
  const { data } = await api.get(
    `api/parcel-locker/courier/${courierId}/shipments-in-lockers`,
    params
  )

  return data
}

export const useShipmentsInLockers = (courierId, postalService, enabled = true) => {
  return useQuery({
    queryKey: ['courier-shipments-in-lockers', courierId, postalService],
    queryFn: async () => getShipmentsInLockers({ courierId, postalService }),
    enabled
  })
}

//===========================================================================

const getPendingShipments = async ({ courierId, postalService }) => {
  const params = {
    params: {
      postalService
    }
  }
  const { data } = await api.get(`api/parcel-locker/courier/${courierId}/pending-shipments`, params)

  return data
}

export const usePendingShipments = (courierId, postalService, enabled = true) => {
  return useQuery({
    queryKey: ['courier-pending-shipments', courierId, postalService],
    queryFn: async () => getPendingShipments({ courierId, postalService }),
    enabled
  })
}
