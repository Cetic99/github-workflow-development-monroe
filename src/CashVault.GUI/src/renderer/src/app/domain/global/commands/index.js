/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useMutation } from '@tanstack/react-query'
import { post } from '@src/app/infrastructure/api'
import { checkAuthenticationSocketMessages } from '@domain/operator/services'
import { setLoading } from '@domain/global/stores'
import { fetchUserWidgets } from '../../configuration/mutations/user-widgets'

//===========================================================================

const authenticate = async (data) => {
  const result = await post(`api/auth/authenticate`, data)

  return result
}

export const useAuthenticate = (onSuccess, onError) => {
  return useMutation({
    mutationFn: authenticate,
    onError: () => {
      if (onError) onError()
      setLoading(false)
    },
    onSuccess: () => {
      if (onSuccess) onSuccess()
    },
    onMutate: () => {
      checkAuthenticationSocketMessages()
    }
  })
}
//===========================================================================

const logout = async () => {
  const result = await post(`api/auth/logout`)

  await fetchUserWidgets()

  return result
}

export const useLogout = (onSuccess, onError) => {
  return useMutation({
    mutationFn: logout,
    onError: () => {
      if (onError) onError()
    },
    onSuccess: () => {
      if (onSuccess) onSuccess()
    }
  })
}
