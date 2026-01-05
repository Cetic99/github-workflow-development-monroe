/* eslint-disable prettier/prettier */

import { MessageType } from '@src/app/infrastructure/web-socket/index'
import { login, setLoading, setLoginOpen, setAuthFailedStatus } from '@domain/global/stores'
import { addMessage } from '@domain/global/stores/socket-message-store'

const receiveAuthenticationFailed = () => {
  setAuthFailedStatus(false)
  addMessage({
    type: MessageType.AuthenticationFailed,
    uuid: null
  })
  setLoading(false)
}

const receiveAuthenticationSuccessfull = (_payload) => {
  addMessage({
    type: MessageType.AuthenticationSuccessfull,
    uuid: null
  })

  const payload = JSON.parse(_payload)

  login(payload.accessToken, payload.refreshToken)

  setLoginOpen(false)
  setAuthFailedStatus(undefined)
  setLoading(false)
}

export const SocketService = {
  process: (type, payload) => {
    switch (type) {
      case MessageType.AuthenticationSuccessfull:
        receiveAuthenticationSuccessfull(payload)
        break

      case MessageType.AuthenticationFailed:
        receiveAuthenticationFailed()
        break

      default:
        break
    }
  }
}
