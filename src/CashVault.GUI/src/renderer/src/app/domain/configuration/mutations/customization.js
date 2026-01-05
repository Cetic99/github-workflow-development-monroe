/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useMutation } from '@tanstack/react-query'
import { post } from '@src/app/infrastructure/api'
import Notifications from '@src/app/services/notifications'

//============================================================================

const saveCustomizationSound = async (data) => {
  const result = await post(`api/configuration/terminal/customization/sound`, data)

  return result
}

export const useSaveCustomizationSound = (onSuccess, onError) => {
  return useMutation({
    mutationFn: saveCustomizationSound,
    onError,
    onSuccess: () => {
      if (onSuccess) onSuccess()
      Notifications.success('Successfully saved!')
    }
  })
}

//============================================================================

const saveCustomizationSoundEvents = async (data) => {
  const result = await post(`api/configuration/terminal/customization/sound-events`, data)

  return result
}

export const useSaveCustomizationSoundEvents = (onSuccess, onError) => {
  return useMutation({
    mutationFn: saveCustomizationSoundEvents,
    onError,
    onSuccess: () => {
      if (onSuccess) onSuccess()
      Notifications.success('Successfully saved!')
    }
  })
}

//============================================================================

const saveCustomizationVideo = async (data) => {
  const result = await post(`api/configuration/terminal/customization/video`, data)

  return result
}

export const useSaveCustomizationVideo = (onSuccess, onError) => {
  return useMutation({
    mutationFn: saveCustomizationVideo,
    onError,
    onSuccess: () => {
      if (onSuccess) onSuccess()
      Notifications.success('Successfully saved!')
    }
  })
}

//============================================================================

const saveCustomizationFlash = async (data) => {
  const result = await post(`api/configuration/terminal/customization/flash`, data)

  return result
}

export const useSaveCustomizationFlash = (onSuccess, onError) => {
  return useMutation({
    mutationFn: saveCustomizationFlash,
    onError,
    onSuccess: () => {
      if (onSuccess) onSuccess()
      Notifications.success('Successfully saved!')
    }
  })
}
