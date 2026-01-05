/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

const getUserWidgetsConfiguration = async () => {
  const { data } = await api.get(`api/configuration/terminal/widgets`)

  return data
}

export const useUserWidgets = () => {
  return useQuery({
    queryKey: ['configuration-user-widgets'],
    queryFn: async () => getUserWidgetsConfiguration()
  })
}

// =====================================================================

const getAvailableUserWidgetsConfiguration = async () => {
  const { data } = await api.get('api/configuration/terminal/widgets/available')

  return data
}

export const useUserWidgetsConfiguration = () => {
  return useQuery({
    queryKey: ['configuration-customization-user-widgets'],
    queryFn: async () => getAvailableUserWidgetsConfiguration()
  })
}
