/* eslint-disable prettier/prettier */

import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export const parcelStore = create(
  immer((set) => ({
    data: {
      method: null,
      barcode: null,
      accessCode: null,
      paymentInfo: {}
    },
    sendData: {
      lockerSize: null,
      size: null,
      deliveryOption: null,
      recipientData: null,
      senderData: null,
      barcode: null,
      accessCode: null
    },
    mapFilters: {
      providers: [],
      locationTypes: []
    },
    postalService: null,
    postalServices: [],
    locationTypes: [],
    courier: null,
    // --- Actions ---
    actions: {
      setPostalService: (code) => {
        set((state) => {
          state.postalService = code
        })
      },
      setMethod: (method) => {
        set((state) => {
          state.data.method = method
        })
      },
      setShipmentBarcode: (code) => {
        set((state) => {
          state.data.barcode = code
        })
      },
      setShipmentAccessCode: (code) => {
        set((state) => {
          state.data.accessCode = code
        })
      },
      resetShipmentData: () => {
        set((state) => {
          state.data.accessCode = null
          state.data.barcode = null
        })
      },
      setPaymentInfo: (data) => {
        set((state) => {
          state.data.paymentInfo = data
        })
      },
      setPostalServices: (data) => {
        set((state) => {
          state.postalServices = data
        })
      },
      setLocationTypes: (data) => {
        set((state) => {
          state.locationTypes = data
        })
      },
      resetData: () => {
        set((state) => {
          state.data = {
            method: null,
            barcode: null,
            accessCode: null,
            paymentInfo: {}
          }
          state.mapFilters = { providers: [], locationTypes: [] }
          state.postalService = null
          state.sendData = {
            lockerSize: null,
            size: null,
            deliveryOption: null,
            recipientData: null,
            barcode: null,
            accessCode: null
          }
        })
      },
      setLockerSize: (size) => {
        set((state) => {
          state.sendData.size = size
        })
      },
      setDeliveryOption: (data) => {
        set((state) => {
          state.sendData.deliveryOption = data
        })
      },
      setRecipientData: (data) => {
        set((state) => {
          state.sendData.recipientData = data
        })
      },
      setSenderData: (data) => {
        set((state) => {
          state.sendData.senderData = data
        })
      },
      setShipmentDetails: (data) => {
        set((state) => {
          state.sendData.barcode = data?.barcode
          state.sendData.accessCode = data?.lockerAccessCode
          state.sendData.registrationNumber = data?.registrationNumber
        })
      },
      setMapFilters: (data) => {
        set((state) => {
          state.mapFilters = data
        })
      },
      setCourier: (data) => {
        set((state) => {
          state.courier = data
        })
      }
    }
  }))
)

export const usePostalService = () => parcelStore((state) => state.postalService)
export const useMethod = () => parcelStore((state) => state.data.method)
export const usePaymentInfo = () => parcelStore((state) => state.data.paymentInfo)
export const usePostalServices = () => parcelStore((state) => state.postalServices)
export const useLocationTypes = () => parcelStore((state) => state.locationTypes)
export const useLockerSize = () => parcelStore((state) => state.sendData.size)
export const useDeliveryOption = () => parcelStore((state) => state.sendData.deliveryOption)
export const useRecipientData = () => parcelStore((state) => state.sendData.recipientData)
export const useSenderData = () => parcelStore((state) => state.sendData.senderData)
export const useShipmentDetails = () =>
  parcelStore((state) => {
    return {
      barcode: state.sendData?.barcode,
      accessCode: state.sendData?.accessCode
    }
  })
export const usePickupData = () =>
  parcelStore((state) => {
    return {
      postalService: state.postalService,
      barcode: state.data.barcode,
      accessCode: state.data.accessCode
    }
  })

export const usePostalServiceData = () =>
  parcelStore((state) => {
    const x = state.postalServices.find((y) => y.code === state.postalService)
    return {
      code: x?.code || state.postalService,
      name: x?.name || state.postalService
    }
  })

export const useMapFilters = () => parcelStore((state) => state.mapFilters)
export const useCourier = () => parcelStore((state) => state.courier)

export const getPostalService = () => parcelStore.getState().data.postalService
export const getMethod = () => parcelStore.getState().data.method

/* Actions */
export const useParcelStoreActions = () => parcelStore((state) => state.actions)

export const loadPostalServices = (data) => {
  parcelStore.getState().actions.setPostalServices(data)
}

export const loadLocationTypes = (data) => {
  parcelStore.getState().actions.setLocationTypes(data)
}
