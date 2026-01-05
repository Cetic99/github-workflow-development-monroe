/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useMutation, useQueryClient } from '@tanstack/react-query'
import Notifications from '../../../services/notifications'
import { post } from '@src/app/infrastructure/api'

//============================================================================

const saveDeviceMainData = async (data) => {
  const result = await post(`api/configuration/terminal/main`, data)

  return result
}

export const useSaveDeviceMainData = (onSuccess = () => {}, onError = () => {}) => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: saveDeviceMainData,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      qc.invalidateQueries('configuration-device-main')
      onSuccess()
    }
  })
}

//============================================================================

const saveDeviceUpsData = async (data) => {
  const result = await post(`api/configuration/terminal/ups`, data)

  return result
}

export const useSaveDeviceUpsData = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: saveDeviceUpsData,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      Notifications.success('Successfully saved!')
      onSuccess()
    }
  })
}

//============================================================================

const saveDeviceNewtorkData = async (data) => {
  const result = await post(`api/configuration/terminal/network`, data)

  return result
}

export const useSaveDeviceNetworkData = (onSuccess = () => {}, onError = () => {}) => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: saveDeviceNewtorkData,

    onError: () => {
      qc.invalidateQueries('configuration-device-network')
      onError()
    },
    onSuccess: () => {
      qc.invalidateQueries('configuration-device-network')
      Notifications.success('Successfully saved!')
      onSuccess()
    }
  })
}

//============================================================================

const saveDeviceServerData = async (data) => {
  const result = await post(`api/configuration/terminal/server`, data)

  return result
}

export const useSaveDeviceServerData = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: saveDeviceServerData,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      Notifications.success('Successfully saved!')
      onSuccess()
    }
  })
}

//============================================================================

const saveDeviceOnlineConfigData = async (data) => {
  const result = await post(`api/configuration/terminal/online-integrations`, data)

  return result
}

export const useSaveDeviceOnlineConfigData = (onSuccess = () => {}, onError = () => {}) => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: saveDeviceOnlineConfigData,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      Notifications.success('Successfully saved!')
      queryClient.invalidateQueries('configuration-device-online-config')
      onSuccess()
    }
  })
}

//============================================================================

const saveDeviceRegionalData = async (data) => {
  const result = await post(`api/configuration/terminal/regional`, {
    Data: data
  })

  return result
}

export const useSaveDeviceRegionalData = (onSuccess = () => {}, onError = () => {}) => {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: saveDeviceRegionalData,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      onSuccess()
      queryClient.invalidateQueries('configuration-device-regional')
    }
  })
}
