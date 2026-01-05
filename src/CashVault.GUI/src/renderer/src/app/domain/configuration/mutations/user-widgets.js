/* eslint-disable prettier/prettier */
import { useMutation, useQueryClient } from '@tanstack/react-query'
import Notifications from '@src/app/services/notifications'
import api, { put } from '@src/app/infrastructure/api'
import { initializeUserWidgets } from '../stores'

const updateUserWidgetsConfiguration = async (data) => {
  const result = await put(`api/configuration/terminal/widgets`, data)

  return result
}

export const useUpdateUserWidgetsConfiguration = (onSuccess = () => {}, onError = () => {}) => {
  const qc = useQueryClient()

  return useMutation({
    mutationFn: updateUserWidgetsConfiguration,
    onError,
    onSuccess: () => {
      Notifications.success('Successfully saved!')

      qc.invalidateQueries([
        'configuration-customization-user-widgets',
        'configuration-user-widgets'
      ])

      onSuccess?.()
    }
  })
}

export const fetchUserWidgets = async () => {
  let response = await api.get(`api/configuration/terminal/widgets`)

  if (response?.data) initializeUserWidgets(response?.data)
}
