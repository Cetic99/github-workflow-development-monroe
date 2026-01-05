/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useMutation, useQueryClient } from '@tanstack/react-query'
import Notifications from '@src/app/services/notifications'
import { put } from '@src/app/infrastructure/api'
import apiDevice from '@src/app/infrastructure/api/api-device'

//===========================================================================

const savePayoutRulesData = async (data) => {
  const result = await put(`api/administration/payout-rules`, data)

  return result
}

export const useSavePayoutRules = (onSuccess, onError) => {
  return useMutation({
    mutationFn: savePayoutRulesData,
    onError,
    onSuccess: () => {
      Notifications.success('Successfully saved!')
    }
  })
}

//===========================================================================

const saveMessage = async (data) => {
  const result = await put(`api/administration/message`, data)

  return result
}

export const useSaveMessage = (onSuccess, onError) => {
  return useMutation({
    mutationFn: saveMessage,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Message value updated!')
    },
    onError
  })
}

//===========================================================================

const resetDevice = async (data) => {
  const result = await apiDevice.put(`api/device/${data.deviceType}/reset`, data)

  return result
}

export const useResetDevice = (onSuccess = () => {}, onError = () => {}) => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: resetDevice,
    onError: () => {
      Notifications.error('Error while reseting device')
      onError()
      queryClient.invalidateQueries(['administartion-active-devices'])
    },
    onSuccess: () => {
      Notifications.success('The device has been successfully reset')
      onSuccess()
      queryClient.invalidateQueries(['administartion-active-devices'])
    }
  })
}

//===========================================================================

const resetAllDevices = async () => {
  const result = await apiDevice.put(`api/devices/reset_all`)

  return result
}

export const useResetAllDevices = (onSuccess = () => {}, onError = () => {}) => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: resetAllDevices,
    onError: () => {
      Notifications.error('Error while reset all devices.')
      onError()
      queryClient.invalidateQueries(['administartion-active-devices'])
    },
    onSuccess: () => {
      Notifications.success('All devices have been successfully reset.')
      onSuccess()
      queryClient.invalidateQueries(['administartion-active-devices'])
    }
  })
}

//===========================================================================

const enableDevice = async (data) => {
  const result = await apiDevice.put(`api/device/${data.deviceType}/enable`)

  return result
}

export const useEnableDevice = (onSuccess = () => {}, onError = () => {}) => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: enableDevice,
    onError: () => {
      Notifications.error('Error while enabling device.')
      onError()
      queryClient.invalidateQueries(['administartion-active-devices'])
    },
    onSuccess: () => {
      Notifications.success('Device have been successfully enabled.')
      onSuccess()
      queryClient.invalidateQueries(['administartion-active-devices'])
    }
  })
}

//===========================================================================

const disableDevice = async (data) => {
  const result = await apiDevice.put(`api/device/${data.deviceType}/disable`)

  return result
}

export const useDisableDevice = (onSuccess = () => {}, onError = () => {}) => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: disableDevice,
    onError: () => {
      Notifications.error('Error while disabling device.')
      onError()
      queryClient.invalidateQueries(['administartion-active-devices'])
    },
    onSuccess: () => {
      Notifications.success('Device have been successfully disabled.')
      onSuccess()
      queryClient.invalidateQueries(['administartion-active-devices'])
    }
  })
}

//===========================================================================
const executeDeviceDiagnosticCommand = async (data) => {
  const result = await apiDevice.post(`api/device/${data.deviceType}/diagnostic_command`, data)

  return result
}

export const useRunDeviceDiagnosticCommand = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: executeDeviceDiagnosticCommand,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      onSuccess()
    }
  })
}
