/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//============================================================================

const getConfigurationCustomizationSound = async () => {
  const { data } = await api.get(`api/configuration/terminal/customization/sound`)

  return data
}

export const useConfigurationCustomizationSound = () => {
  return useQuery({
    queryKey: ['configuration-customization-sound'],
    queryFn: async () => getConfigurationCustomizationSound()
  })
}

//============================================================================

const getConfigurationCustomizationSoundEvents = async () => {
  const { data } = await api.get(`api/configuration/terminal/customization/sound-events`)

  return data
}

export const useConfigurationCustomizationSoundEvents = () => {
  return useQuery({
    queryKey: ['configuration-customization-sound-events'],
    queryFn: async () => getConfigurationCustomizationSoundEvents()
  })
}

//============================================================================

const getConfigurationCustomizationVideo = async () => {
  const { data } = await api.get(`api/configuration/terminal/customization/video`)

  return data
}

export const useConfigurationCustomizationVideo = () => {
  return useQuery({
    queryKey: ['configuration-customization-video'],
    queryFn: async () => getConfigurationCustomizationVideo()
  })
}

//============================================================================

const getConfigurationCustomizationFlash = async () => {
  const { data } = await api.get(`api/configuration/terminal/customization/flash`)

  return data
}

export const useConfigurationCustomizationFlash = () => {
  return useQuery({
    queryKey: ['configuration-customization-flash'],
    queryFn: async () => getConfigurationCustomizationFlash()
  })
}
