/* eslint-disable prettier/prettier */
import * as signalR from '@microsoft/signalr'
import { isFunction } from 'lodash'
import { globalStore } from '../../domain/global/stores'

const baseUrl = 'http://localhost:5745'
const moneyStatusHub = 'moneyserviceshub'
const deviceHub = 'deviceeventshub'

let moneyStatusConnection = {}
let deviceConnection = {}

export const startConnection = async (processors = []) => {
  if (
    moneyStatusConnection &&
    (moneyStatusConnection.state === signalR.HubConnectionState.Connected ||
      moneyStatusConnection.state === signalR.HubConnectionState.Connecting)
  ) {
    console.log('Connection already established or in progress')
    return
  }

  setInterval(() => {
    if (moneyStatusConnection && moneyStatusConnection.state === 'Connected') {
      globalStore.getState().actions.connected()
    } else {
      globalStore.getState().actions.disconnected()
      if (moneyStatusConnection && moneyStatusConnection.state === 'Disconnected') {
        moneyStatusConnection.start().catch((error) => {
          console.log('Connection to WebSocket hub lost. Attempting to reconnect...', error)
        })
      }
    }
  }, 1000)

  // if (connection && connection.state === 'Connected') {
  //   connection.stop()
  //   globalStore.getState().actions.disconnected()
  // }

  moneyStatusConnection = createConnection(moneyStatusHub)

  try {
    moneyStatusConnection.on('SendMessage', (messageType, payload) => {
      processors.forEach((p) => {
        if (isFunction(p.process)) {
          p.process(messageType, payload)
        }
      })
    })

    moneyStatusConnection.on('SendMoneySatusError', (messageType, payload) => {
      processors.forEach((p) => {
        if (isFunction(p.process)) {
          p.process(messageType, payload)
        }
      })
    })

    if (moneyStatusConnection.state !== 'Connected') {
      await moneyStatusConnection.start()

      globalStore.getState().actions.connected()
    }
  } catch (err) {
    globalStore.getState().actions.disconnected()
  }
}

export const startDeviceHubConnection = async (processors = []) => {
  if (
    deviceConnection &&
    (deviceConnection.state === signalR.HubConnectionState.Connected ||
      deviceConnection.state === signalR.HubConnectionState.Connecting)
  ) {
    console.log('Connection already established or in progress')
    return
  }

  setInterval(() => {
    if (deviceConnection && deviceConnection.state === 'Connected') {
      globalStore.getState().actions.connected()
    } else {
      globalStore.getState().actions.disconnected()
      if (deviceConnection && deviceConnection.state === 'Disconnected') {
        deviceConnection.start().catch((error) => {
          console.log('Connection to WebSocket hub lost. Attempting to reconnect...', error)
        })
      }
    }
  }, 1000)

  deviceConnection = createConnection(deviceHub)

  try {
    deviceConnection.on('DeviceError', (messageType, payload) => {
      processors.forEach((p) => {
        if (isFunction(p.process)) {
          p.process(messageType, payload)
        }
      })
    })

    deviceConnection.on('DeviceStatus', (messageType, payload) => {
      processors.forEach((p) => {
        if (isFunction(p.process)) {
          p.process(messageType, payload)
        }
      })
    })

    deviceConnection.on('SendMessage', (messageType, payload) => {
      //if (messageType === 'UserWidgetsUpdated') {
      //  onHandleUserWidgetsUpdate(payload)
      //  return
      //}

      processors.forEach((p) => {
        if (isFunction(p.process)) {
          p.process(messageType, payload)
        }
      })
    })

    if (deviceConnection.state !== 'Connected') {
      await deviceConnection.start()

      globalStore.getState().actions.connected()
    }
  } catch (err) {
    globalStore.getState().actions.disconnected()
  }
}

export const sendMessage = (message) => {
  try {
    if (moneyStatusConnection.state === 'Connected') {
      moneyStatusConnection.invoke('ReceiveMessage', Area.GENERAL, JSON.stringify(message))
    }
  } catch (err) {
    console.error(err)
  }
}

export const sendDeviceMessage = (message) => {
  try {
    if (deviceConnection.state === 'Connected') {
      deviceConnection.invoke('ReceiveMessage', Area.GENERAL, JSON.stringify(message))
    }
  } catch (err) {
    console.error(err)
  }
}

export const sendError = (message) => {
  try {
    if (deviceConnection.state === 'Connected') {
      deviceConnection.invoke('ReceiveError', JSON.stringify(message))
    }
  } catch (err) {
    console.error(err)
  }
}

const Area = {
  GENERAL: 'General'
}

export const MessageType = {
  GetCreditsAmount: 'GetCreditsAmount',
  SetCreditsAmount: 'SetCreditsAmount',

  // BillDispensingRequested: 'BillDispensingRequested',
  BillDispensingCompleted: 'BillDispensingCompleted',
  BillDispensingFailed: 'BillDispensingFailed',
  //---------------------------------------------------------
  // TicketPrintingRequested: 'TicketPrintingRequested',
  TicketPrintingCompleted: 'TicketPrintingCompleted',
  TicketPrintingFailed: 'TicketPrintingFailed',
  //---------------------------------------------------------
  PayoutRequested: 'PayoutRequested',
  PayoutCompleted: 'PayoutCompleted',
  PayoutFailed: 'PayoutFailed',
  //---------------------------------------------------------
  BillAccepting: 'BillAccepting',
  BillAccepted: 'BillAccepted',
  BillRejected: 'BillRejected',
  TicketAccepted: 'TicketAccepted',
  TicketRejected: 'TicketRejected',
  CoinAccepted: 'CoinAccepted',
  CoinRejected: 'CoinRejected',
  //---------------------------------------------------------
  AuthenticationFailed: 'AuthenticationFailed',
  AuthenticationSuccessfull: 'AuthenticationSuccessfull',
  // ------------- Device events ---------------------------
  DeviceError: 'DeviceError',
  DeviceWarning: 'DeviceWarning',
  DeviceEnabled: 'DeviceEnabled',
  DeviceDisabled: 'DeviceDisabled',
  DeviceActivated: 'DeviceActivated',
  DeviceDeactivated: 'DeviceDeactivated',
  DeviceConnected: 'DeviceConnected',
  DeviceDisconnected: 'DeviceDisconnected',

  // ------------- Card events -----------------------------
  CardReaderInitialized: 'CardReaderInitialized',
  CardScanCompleted: 'CardScanCompleted',
  CardSuccessfullyAdded: 'CardSuccessfullyAdded',
  CardReaderInitialzationFailed: 'CardReaderInitialzationFailed',
  CardScanFailed: 'CardScanFailed',
  CardAddingFailed: 'CardAddingFailed',
  CardEnrolled: 'CardEnrolled',
  CardeEnrolmentFailed: 'CardeEnrolmentFailed',
  OperationExecuted: 'OperationExecuted',
  UserWidgetsUpdated: 'UserWidgetsUpdated',

  // ------------- Parcel locker events --------------------
  ParcelLockerClosed: 'ParcelLockerClosed'
}

const createConnection = (hubName) => {
  const token = globalStore.getState().accessToken

  const url = `${baseUrl}/${hubName}?token=${token}`

  return (
    new signalR.HubConnectionBuilder()
      .withUrl(url)
      // .configureLogging(signalR.LogLevel.Information)
      .build()
  )
}
