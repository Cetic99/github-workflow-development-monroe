/* eslint-disable no-empty */
/* eslint-disable prettier/prettier */
import { jwtDecode } from 'jwt-decode'
import { removeMessageByType } from '@domain/global/stores/socket-message-store'
import { MessageType } from '@src/app/infrastructure/web-socket/index'

export const decodeToken = (token) => {
  const decoded = jwtDecode(token)

  var permissions = []

  try {
    permissions = JSON.parse(decoded['Permissions'])
  } catch (e) {}

  return {
    username: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
    id: parseInt(decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']),
    permissions
  }
}

const clearMessages = () => {
  removeMessageByType(MessageType.AuthenticationFailed)
  removeMessageByType(MessageType.AuthenticationSuccessfull)
}

export const checkAuthenticationSocketMessages = () => {
  clearMessages()
}
