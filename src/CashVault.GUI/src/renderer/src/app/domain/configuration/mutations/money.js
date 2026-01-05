/* eslint-disable prettier/prettier */
import { useMutation, useQueryClient } from '@tanstack/react-query'
import Notifications from '@src/app/services/notifications'
import { put, post } from '@src/app/infrastructure/api'

//=======================================================================

const saveMoneyBillAcceptorData = async (data) => {
  const result = await post(`api/configuration/terminal/bill-acceptor`, data)

  return result
}

export const useSaveMoneyBillAcceptorData = (onSuccess = () => {}, onError = () => {}) => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: saveMoneyBillAcceptorData,
    onError,
    onSuccess: () => {
      Notifications.success('Successfully saved!')
      qc.invalidateQueries(['configuration-money-bill-acceptor'])
      onSuccess?.()
    }
  })
}

//=======================================================================

const saveMoneyBillDispenserConfigData = async (data) => {
  const result = await post(`api/configuration/terminal/bill-dispenser`, data)

  return result
}

export const useSaveMoneyBillDispenserConfigData = () => {
  return useMutation({
    mutationFn: saveMoneyBillDispenserConfigData,
    onError: (error) => {
      try {
        const errorDetails = JSON.parse(error.response?.data?.detail)
        if (errorDetails) {
          let errorMessage = ''
          errorDetails.forEach((element) => {
            errorMessage += element.PropertyName + ': ' + element.ErrorMessage + '\n'
          })
          Notifications.error(errorMessage)
        } else {
          Notifications.error('Error saving data!')
        }
      } catch (error) {
        Notifications.error('Error saving data!')
      }
    },
    onSuccess: () => {
      Notifications.success('Successfully saved!')
    }
  })
}

//===========================================================================

const saveMoneyBillDispenserRefillData = async (data) => {
  const result = await post(`api/configuration/terminal/bill-dispenser-refill`, data)

  return result
}

export const useSaveMoneyBillDispenserRefillData = (onSuccess = () => {}) => {
  return useMutation({
    mutationFn: saveMoneyBillDispenserRefillData,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Successfully saved!')
    }
  })
}

//=======================================================================

const saveMoneyCoinDispenserData = async (data) => {
  const result = await post(`api/configuration/terminal/coin-dispenser`, data)

  return result
}

export const useSaveMoneyCoinDispenserData = (onSuccess, onError) => {
  return useMutation({
    mutationFn: saveMoneyCoinDispenserData,
    onError,
    onSuccess: () => {
      Notifications.success('Successfully saved!')
    }
  })
}

//==============================================================================
const emptyBillDispenserCassettes = async (data) => {
  const result = await put(`api/configuration/terminal/bill-dispenser/empty-cassettes`, data)

  return result
}

export const useEmptyBillDispenserCassettes = (onSuccess = () => {}) => {
  return useMutation({
    mutationFn: emptyBillDispenserCassettes,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Successfully saved!')
    }
  })
}

//=======================================================================

const saveMoneyTITOPrinterData = async (data) => {
  const result = await post(`api/configuration/terminal/tito-printer`, data)

  return result
}

export const useSaveMoneyTITOPrinterData = (onSuccess = () => {}, onError = () => {}) => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: saveMoneyTITOPrinterData,
    onError,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Successfully saved!')
      qc.invalidateQueries(['configuration-money-tito-printer'])
      onSuccess()
    }
  })
}

//=======================================================================
const emptyBillTicketAcceptor = async () => {
  const result = await put(`api/configuration/terminal/bill-acceptor/empty`)

  return result
}

export const useEmptyBillTicketAcceptor = (onSuccess = () => {}) => {
  const qc = useQueryClient()

  return useMutation({
    mutationFn: emptyBillTicketAcceptor,
    onSuccess: () => {
      qc.invalidateQueries(['configuration-money-bill-acceptor'])
      Notifications.success('Successfully saved!')
      onSuccess()
    }
  })
}

//=======================================================================

const saveMoneyCoinAcceptorData = async (data) => {
  const result = await put(`api/configuration/terminal/coin-acceptor`, data)

  return result
}

export const useSaveMoneyCoinAcceptorData = (onSuccess = () => {}, onError = () => {}) => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: saveMoneyCoinAcceptorData,
    onError,
    onSuccess: () => {
      Notifications.success('Successfully saved!')
      qc.invalidateQueries(['configuration-money-coin-acceptor'])
      onSuccess?.()
    }
  })
}

//=======================================================================
