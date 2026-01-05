/* eslint-disable no-unused-vars */



import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//===========================================================================

const getPayoutRulesData = async () => {
  const { data } = await api.get(`api/administration/payout-rules`)

  return data
}

export const usePayoutRules = () => {
  return useQuery({
    queryKey: ['administartion-payout-rules'],
    queryFn: async () => getPayoutRulesData()
  })
}

//===========================================================================

const getMessagesData = async (params) => {
  const { data } = await api.get(`api/administration/messages`, { params })

  return data
}

export const useMessages = (params) => {
  return useQuery({
    queryKey: ['administartion-messages', ...Object.values(params)],
    queryFn: async () => getMessagesData(params),
    enabled: params.enabled
  })
}

//===========================================================================

const getActiveDevices = async (params) => {
  const { data } = await api.get(`api/devices/active`, {
    params
  })

  return data
}

export const useGetActiveDevices = () => {
  return useQuery({
    queryKey: ['administartion-active-devices'],
    queryFn: async () => getActiveDevices()
  })
}

//===========================================================================

const getDeviceInfo = async (type) => {
  const { data } = await api.get(`api/device/info/${type}`)

  return data
}

export const useGetDeviceInfo = ({ type, enabled }) => {
  return useQuery({
    queryKey: ['administartion-device-info', type],
    queryFn: async () => getDeviceInfo(type),
    enabled,
    staleTime: 5000,
    refetchInterval: 5000,
    cacheTime: 5000
  })
}

//===========================================================================

const getDeviceDiagnosticCommands = async (type) => {
  const { data } = await api.get(`api/device/${type}/diagnostic_commands`)

  return data
}

export const useGetDeviceDiagnosticCommands = ({ type }) => {
  return useQuery({
    queryKey: ['administartion-device-info', type],
    queryFn: async () => getDeviceDiagnosticCommands(type),
    staleTime: 5000,
    cacheTime: 5000
  })
}
