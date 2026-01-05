/* eslint-disable prettier/prettier */
import { MessageType as ServerMessageType } from '@src/app/infrastructure/web-socket/index'

export const SocketService = {
  process: (type, payload) => {
    switch (type) {
      case ServerMessageType.ParcelLockerClosed:
        // todo
        console.info('Event received: ParcelLockerClosed', payload)
        break
      default:
        break
    }
  }
}
