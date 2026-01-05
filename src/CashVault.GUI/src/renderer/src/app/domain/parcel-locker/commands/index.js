/* eslint-disable prettier/prettier */
import { post, put } from '@src/app/infrastructure/api'
import { useMutation } from '@tanstack/react-query'

//===========================================================================

const sendParcelLockerRequest = async (data) => {
  const result = await post(`api/parcel-locker/pickup`, data)

  return result
}

export const useSendParcelLockerRequest = (onSuccess, onError) => {
  return useMutation({
    mutationFn: sendParcelLockerRequest,
    onError,
    onSuccess
  })
}

//===========================================================================

const openParcelLocker = async (data) => {
  const result = await put(`api/parcel-locker/open`, data)

  return result
}

export const useOpenParcelLocker = (onSuccess, onError) => {
  return useMutation({
    mutationFn: openParcelLocker,
    onSuccess,
    onError
  })
}

//===========================================================================

const createShipment = async (data) => {
  const result = await post(`api/parcel-locker/shipment`, data)

  return result
}

export const useCreateShipment = (onSuccess, onError) => {
  return useMutation({
    mutationFn: createShipment,
    onError,
    onSuccess
  })
}

//===========================================================================

const changeLockerSize = async (data) => {
  const result = await put(`api/parcel-locker/shipment/${data?.trackingNumber}/change-locker`, data)

  return result
}

export const useChangeLockerSize = (onSuccess, onError) => {
  return useMutation({
    mutationFn: changeLockerSize,
    onError,
    onSuccess
  })
}
//===========================================================================
// COURIER
//===========================================================================

const verifyCourier = async (data) => {
  const result = await post(`api/parcel-locker/verify-courier`, data)

  return result
}

export const useVerifyCourier = (onSuccess, onError) => {
  return useMutation({
    mutationFn: verifyCourier,
    onError,
    onSuccess
  })
}

//===========================================================================

const assignShipment = async (data) => {
  const result = await put(`api/parcel-locker/assign-shipment`, data)

  return result
}

export const useAssignShipment = (onSuccess, onError) => {
  return useMutation({
    mutationFn: assignShipment,
    onError,
    onSuccess
  })
}

//===========================================================================

const pickupShipment = async (data) => {
  const result = await put(`api/parcel-locker/courier/${data?.courierId}/pickup`, data)

  return result
}

export const usePickupShipment = (onSuccess, onError) => {
  return useMutation({
    mutationFn: pickupShipment,
    onError,
    onSuccess
  })
}
