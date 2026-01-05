/* eslint-disable prettier/prettier */
import {
  attachRequestInterceptor,
  attachRequestForLanguageInterceptor
} from '@src/app/infrastructure/api'
import { useEffect } from 'react'
import {
  useIsAuthenticated,
  useGlobalActions,
  useHeartbeat,
  useIsInitialized
} from '@domain/global/stores'
import { useNavigate } from 'react-router-dom'
import { useLogout } from '@domain/global/commands'
import { useCurrentLanguage } from '@domain/administration/stores'

const useAuthentication = () => {
  const navigate = useNavigate()
  const authenticated = useIsAuthenticated()
  const connected = useHeartbeat()
  const { logout } = useGlobalActions()
  const language = useCurrentLanguage()
  const isInitialized = useIsInitialized()

  const logoutCommand = useLogout()

  useEffect(() => {
    if (authenticated) attachRequestInterceptor()
    else attachRequestForLanguageInterceptor()
  }, [authenticated, language])

  useEffect(() => {
    if (isInitialized === true) {
      if (!connected && authenticated) {
        logout()
        navigate('/')
        logoutCommand.mutate()
      }

      if (!authenticated) navigate('/')
    }
  }, [connected, isInitialized])
}

export default useAuthentication
