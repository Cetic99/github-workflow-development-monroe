/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useMutation, useQueryClient } from '@tanstack/react-query'
import Notifications from '@src/app/services/notifications'
import { put, post, remove } from '@src/app/infrastructure/api'
import { Command } from '@src/app/infrastructure/command-system'
import { sendDeviceMessage } from '@src/app/infrastructure/web-socket'
import { fail } from '@src/app/domain/administration/stores/card-reader'

//===========================================================================

const harvestShiftMoney = async (params) => {
  const result = await post(`api/operator/harvest-shift-money`)

  return result
}

export const useHarvestShiftMoney = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: harvestShiftMoney,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Shift money harvesting successfull!')
    },
    onError: () => {
      onError()
      Notifications.error('Shift money harvesting failed!')
    }
  })
}

//===========================================================================

const deactivateCard = async (params) => {
  const result = await put(
    `api/operator/${params.operatorId}/id-card/${params.id}/deactivate`,
    params
  )

  return result
}

export const useCloseCard = (operatorId, onSuccess, onError) => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: deactivateCard,
    onSuccess: () => {
      onSuccess()
      queryClient.invalidateQueries(['administartion-operator-cards', operatorId])
      Notifications.success('Card successfully closed!')
    },
    onError: () => {
      onError()
      Notifications.error('Closing card failed!')
    }
  })
}

const activateCard = async (params) => {
  const result = await put(
    `api/operator/${params.operatorId}/id-card/${params.id}/activate`,
    params
  )

  return result
}

export const useActivateCard = (operatorId, onSuccess, onError) => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: activateCard,
    onSuccess: () => {
      onSuccess()
      queryClient.invalidateQueries(['administartion-operator-cards', operatorId])
      Notifications.success('Card successfully activated!')
    },
    onError: () => {
      onError()
      Notifications.error('Card activation failed!')
    }
  })
}

//===========================================================================

const saveOperator = async (data) => {
  const result = await put(`api/operator`, data)

  return result
}

export const useSaveOperator = (onSuccess, onError) => {
  return useMutation({
    mutationFn: saveOperator,
    onError: () => {
      if (onError) onError()
      Notifications.error('Operator updated failed!')
    },
    onSuccess: (_, data) => {
      if (onSuccess) onSuccess(data)
      Notifications.success('Operator updated successfully!')
    }
  })
}

//===========================================================================

const addOperator = async (data) => {
  const result = await post(`api/operator`, data)

  return result
}

export const useAddOperator = (onSuccess, onError) => {
  return useMutation({
    mutationFn: addOperator,
    onError: () => {
      if (onError) onError()
      Notifications.error('Adding operator failed!')
    },
    onSuccess: (_, data) => {
      if (onSuccess) onSuccess(data)
      Notifications.success('Operator added successfully!')
    }
  })
}

//===========================================================================

const changeOperatorPassword = async (data) => {
  const result = await put(`api/operator/${data.operatorId}/password`, data)

  return result
}

export const useChangeOperatorPassword = (onSuccess, onError) => {
  return useMutation({
    mutationFn: changeOperatorPassword,
    onError: () => {
      onError()
      Notifications.error('Error while changing password!')
    },
    onSuccess: () => {
      onSuccess()
      Notifications.success('Password changed successfully!')
    }
  })
}

//===========================================================================

const addNewCard = async (data) => {
  const result = await post(`api/operator/${data.operatorId}/id-card`, data)

  return result
}

export const useAddNewCard = (onSuccess, onError) => {
  return useMutation({
    mutationFn: addNewCard,
    onError,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Card successfully added!')
    }
  })
}

//===========================================================================

export const CommandType = {
  DisableUserLogin: 'DisableUserLogin',
  EnableUserLogin: 'EnableUserLogin',
  InitializeCardReader: 'InitializeCardReader',
  ScanUserCard: 'ScanUserCard',
  CreateIdentificationCard: 'CreateIdentificationCard'
}

export const MessageType = {
  DisableUserLogin: 'DisableUserLogin',
  EnableUserLogin: 'EnableUserLogin',
  InitializeCardReader: 'InitializeCardReader',
  ScanUserCard: 'ScanUserCard',
  CreateIdentificationCard: 'CreateIdentificationCard'
}

export const CardReaderSteps = [
  MessageType.InitializeCardReader,
  MessageType.ScanUserCard,
  MessageType.CreateIdentificationCard
]

export const initializeCardReader = new Command(CommandType.InitializeCardReader, () => {
  sendDeviceMessage({
    messageType: MessageType.InitializeCardReader
  })

  setTimeout(() => {
    fail()
  }, 200000)
})

export const scanUserCard = new Command(CommandType.ScanUserCard, () => {
  sendDeviceMessage({
    messageType: MessageType.ScanUserCard
  })

  setTimeout(() => {
    fail()
  }, 200000)
})

export const createIdentificationCard = new Command(
  CommandType.CreateIdentificationCard,
  (data) => {
    sendDeviceMessage({
      messageType: MessageType.CreateIdentificationCard,
      ...(data || {})
    })

    setTimeout(() => {
      fail()
    }, 200000)
  }
)

export const disableUserLogin = new Command(CommandType.DisableUserLogin, () => {
  sendDeviceMessage({
    messageType: MessageType.DisableUserLogin
  })
})

export const enableUserLogin = new Command(CommandType.EnableUserLogin, () => {
  sendDeviceMessage({
    messageType: MessageType.EnableUserLogin
  })
})

const commands = [
  disableUserLogin,
  enableUserLogin,
  initializeCardReader,
  scanUserCard,
  createIdentificationCard
]

export default commands
