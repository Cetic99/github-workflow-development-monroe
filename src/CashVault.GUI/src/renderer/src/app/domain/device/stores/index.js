/* eslint-disable prettier/prettier */
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'
import { getDeviceByType } from '@domain/device/services'
import { Device } from '@domain/device/models'
import { DEVICE_TYPE } from '@domain/device/constants'

export const deviceStore = create(
  immer((set) => ({
    initialized: false,
    devices: [],
    operationMessages: [],
    // =======================================
    setDevices: (devices = []) => {
      set((state) => {
        state.devices = devices.map((device) => {
          return new Device(
            device.name,
            device.status,
            device.type,
            device?.isEnabled,
            device?.isActive,
            device?.isConnected,
            device?.error,
            device?.warning,
            device?.interface,
            device?.commandInProgress
          )
        })

        if (state.initialized !== true) state.initialized = true
      })
    },
    initialize: (devices) => {
      set((state) => {
        state.devices = devices.map((device) => {
          return new Device(
            device.name,
            device.status,
            device.type,
            device?.isEnabled,
            device?.isActive,
            device?.isConnected,
            device?.error,
            device?.warning,
            device?.interface,
            device?.commandInProgress
          )
        })
        state.initialized = true
      })
    }
  }))
)

export const useAllDevices = () => deviceStore((state) => state.devices)

export const useGetDeviceByType = (type) =>
  deviceStore((state) => getDeviceByType(state.devices, type))

export const useConnectedGetDevices = () => deviceStore((state) => state.devices)

// ====================== Bill Acceptor =================== //
export const useIsBillAcceptorReady = () =>
  deviceStore((state) => {
    const targetDevice = getDeviceByType(state.devices, DEVICE_TYPE.BILL_ACCEPTOR)

    if (targetDevice) {
      return targetDevice?.isActive
    }
    return false
  })

export const useGetBillAcceptor = () =>
  deviceStore((state) => getDeviceByType(state.devices, DEVICE_TYPE.BILL_ACCEPTOR))

// ==================== Bill Dispenser ==================== //

export const useIsBillDispenserReady = () =>
  deviceStore((state) => {
    const targetDevice = getDeviceByType(state.devices, DEVICE_TYPE.BILL_DISPENSER)

    if (targetDevice) {
      return targetDevice?.isActive
    }
    return false
  })

export const useGetBillDispenser = () =>
  deviceStore((state) => getDeviceByType(state.devices, DEVICE_TYPE.BILL_DISPENSER))

// ==================== Card Reader ==================== //
export const useIsCardReaderReady = () =>
  deviceStore((state) => {
    const targetDevice = getDeviceByType(state.devices, DEVICE_TYPE.CARD_READER)

    if (targetDevice) {
      return targetDevice?.isActive
    }
    return false
  })

export const useGetCardReader = () =>
  deviceStore((state) => getDeviceByType(state.devices, DEVICE_TYPE.CARD_READER))

// ==================== Ticket Printer ==================== //
export const useIsTITOPrinterReady = () =>
  deviceStore((state) => {
    const targetDevice = getDeviceByType(state.devices, DEVICE_TYPE.TITO_PRINTER)
    if (targetDevice) {
      return targetDevice?.isActive
    }
    return false
  })

export const useGetTITOPrinter = () =>
  deviceStore((state) => getDeviceByType(state.devices, DEVICE_TYPE.TITO_PRINTER))

// ==================== Device Warnings ==================== //
export const useShowTITOPrinterWarning = () =>
  deviceStore((state) => {
    const targetDevice = getDeviceByType(state.devices, DEVICE_TYPE.TITO_PRINTER)
    if (targetDevice) {
      return targetDevice?.isActive == false
    }

    return false
  })

export const useShowBillDispenserWarning = () =>
  deviceStore((state) => {
    const targetDevice = getDeviceByType(state.devices, DEVICE_TYPE.BILL_DISPENSER)
    if (targetDevice) {
      return targetDevice?.isActive == false
    }

    return false
  })

export const useShowBillAcceptorWarning = () =>
  deviceStore((state) => {
    const targetDevice = getDeviceByType(state.devices, DEVICE_TYPE.BILL_ACCEPTOR)
    if (targetDevice) {
      return targetDevice?.isActive == false
    }

    return false
  })

// ==================== Device Operations ==================== //
export const cleanOperationMessages = () => {
  deviceStore.setState((state) => {
    state.operationMessages = []
  })
}

export const addOperationMessage = (message) => {
  deviceStore.setState((state) => {
    state.operationMessages.push(message)
  })
}

export const useOperationMessages = () => {
  return {
    messages: deviceStore((s) => s.operationMessages),

    addMessage: (message) => {
      deviceStore.setState((state) => {
        state.operationMessages.push(message)
      })
    },

    dequeueLastMessage: () => {
      const { operationMessages } = deviceStore.getState()
      if (operationMessages.length === 0) return null

      const lastMessage = operationMessages[operationMessages.length - 1]

      deviceStore.setState((state) => {
        state.operationMessages.pop()
      })

      return lastMessage
    },
    clearMessages: () => {
      deviceStore.setState((state) => {
        state.operationMessages = []
      })
    }
  }
}
