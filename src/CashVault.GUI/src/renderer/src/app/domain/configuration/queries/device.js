/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//============================================================================

export const getTerminalTypeConfiguration = async () => {
  const response = await api.get(`api/configuration/terminal/terminal-type-config`)

  return response?.data || {}
}

const getConfigurationDeviceMainData = async () => {
  const { data } = await api.get(`api/configuration/terminal/main`)

  return data
}

export const useConfigurationDeviceMain = () => {
  return useQuery({
    queryKey: ['configuration-device-main'],
    queryFn: async () => getConfigurationDeviceMainData()
  })
}

//============================================================================

const getConfigurationDeviceUpsData = async () => {
  const { data } = await api.get(`api/configuration/terminal/ups`)

  return data
}

export const useConfigurationDeviceUps = () => {
  return useQuery({
    queryKey: ['configuration-device-ups'],
    queryFn: async () => getConfigurationDeviceUpsData()
  })
}

//============================================================================

const getConfigurationDeviceNetworkData = async () => {
  const { data } = await api.get(`api/configuration/terminal/network`)

  return data
}

export const useConfigurationDeviceNetwork = () => {
  return useQuery({
    queryKey: ['configuration-device-network'],
    queryFn: async () => getConfigurationDeviceNetworkData()
  })
}

//============================================================================

const getConfigurationDeviceServerData = async () => {
  const { data } = await api.get(`api/configuration/terminal/server`)

  return data
}

export const useConfigurationDeviceServer = () => {
  return useQuery({
    queryKey: ['configuration-device-server'],
    queryFn: async () => getConfigurationDeviceServerData()
  })
}

//============================================================================

const getConfigurationDeviceOnlineConfigData = async () => {
  const { data } = await api.get(`api/configuration/terminal/online-integrations`)

  return data
}

export const useConfigurationDeviceOnlineConfig = () => {
  return useQuery({
    queryKey: ['configuration-device-online-config'],
    queryFn: async () => getConfigurationDeviceOnlineConfigData()
  })
}

//============================================================================

const getConfigurationDeviceReginalData = async () => {
  const { data } = await api.get(`api/configuration/terminal/regional`)

  return data
}

export const useConfigurationDeviceRegional = () => {
  return useQuery({
    queryKey: ['configuration-device-regional'],
    queryFn: async () => getConfigurationDeviceReginalData()
  })
}
