/* eslint-disable no-case-declarations */
/* eslint-disable prettier/prettier */

import { MessageType } from '@src/app/infrastructure/web-socket/index'
import { safeDeserialize } from '@domain/global/services'
import { deviceStore } from '@domain/device/stores'
import { globalStore } from '@domain/global/stores'

export const SocketService = {
  process: (type, payload) => {
    const event = safeDeserialize(payload)
    switch (type) {
      case MessageType.DeviceError:
        deviceStore.getState().handleOnError(event?.DeviceType, event?.Message)
        break
      case MessageType.DeviceWarning:
        deviceStore.getState().handleOnWarning(event?.DeviceType, event?.Message)
        break
      case MessageType.DeviceEnabled:
        deviceStore.getState().setIsEnabled(event?.DeviceType, true)
        break
      case MessageType.DeviceDisabled:
        deviceStore.getState().setIsEnabled(event?.DeviceType, false)
        break
      case MessageType.DeviceActivated:
        deviceStore.getState().setIsActive(event?.DeviceType, true)
        break
      case MessageType.DeviceDeactivated:
        deviceStore.getState().setIsActive(event?.DeviceType, false)
        break
      case MessageType.DeviceConnected:
        deviceStore.getState().setIsConnected(event?.DeviceType, true)
        break
      case MessageType.DeviceDisconnected:
        deviceStore.getState().setIsConnected(event?.DeviceType, false)
        break
      case MessageType.UserWidgetsUpdated:
        globalStore.getState().actions.setUpgrade()
        break
      default:
        break
    }
  }
}
