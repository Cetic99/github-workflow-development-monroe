/* eslint-disable prettier/prettier */
import * as signalR from '@microsoft/signalr'
import { useEffect } from 'react'
import {
  globalStore,
  heartbeatEstablished,
  heartbeatStopped,
  setAppMode,
  setCmsStatus,
  setTerminalTypeConfig
} from '@domain/global/stores'
import { AppMode } from '@domain/global/constants'
import { deviceStore } from '@domain/device/stores'
import { updateMessage, loadMessages } from '@domain/administration/stores'
import api from '../api'
import { loadRegionalConfiguration } from '@domain/administration/stores/regional'
import { getUserWidgetsActions } from '@domain/configuration/stores'
import { fetchUserWidgets } from '@src/app/domain/configuration/mutations/user-widgets'
import { getConfigurationMoneyTITOPrinter } from '@src/app/domain/configuration/queries/money'
import { loadDeviceConfiguration } from '@src/app/domain/device/stores/device-configuration-store'
import { getTerminalTypeConfiguration } from '@src/app/domain/configuration/queries/device'
import {
  loadLocationTypes,
  loadPostalServices
} from '@src/app/domain/parcel-locker/stores/parcel-store'

const baseUrl = 'http://localhost:5745'
const hub = 'heartbeathub'

let reconnectTimeout = null
let connection = {}

const getLocalizations = async () => {
  let response = await api.get('api/administration/all-messages')

  if (response?.data?.messages?.length > 0)
    loadMessages(response?.data?.messages, response?.data?.defaultLanguageCode)
}

const getRegionalConfiguration = async () => {
  let response = await api.get(`api/configuration/terminal/regional`)

  if (response?.data) loadRegionalConfiguration(response?.data)
}

const getTerminalTypeConfig = async () => {
  let response = await getTerminalTypeConfiguration()

  if (response) setTerminalTypeConfig(response)
}

const getPostalServices = async () => {
  let response = await api.get(`api/parcel-locker/postal-services`)

  if (response?.data) loadPostalServices(response?.data)
}

const getLocationTypes = async () => {
  let response = await api.get(`api/parcel-locker/location-types`)

  if (response?.data) loadLocationTypes(response?.data)
}

const getTITOPrinterConfiguration = async () => {
  let response = await getConfigurationMoneyTITOPrinter()

  if (response?.data) loadDeviceConfiguration(response?.data.ticketTakingTimeout)
}

const completeReconnectTimeout = () => {
  clearTimeout(reconnectTimeout)
  reconnectTimeout = null
}

const startConnection = async () => {
  if (
    connection &&
    (connection.state === signalR.HubConnectionState.Connected ||
      connection.state === signalR.HubConnectionState.Connecting)
  ) {
    console.info('Heartbeat established or in progress')
    return
  }

  setInterval(async () => {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
      heartbeatEstablished()
    }

    if (connection && connection.state !== signalR.HubConnectionState.Connected) {
      if (!reconnectTimeout) {
        reconnectTimeout = setTimeout(() => {
          heartbeatStopped()
          reconnectTimeout = null
        }, 2000)
      }

      if (connection && connection.state === signalR.HubConnectionState.Disconnected) {
        connection
          .start()
          .then(() => {
            completeReconnectTimeout()
            getLocalizations()
            getRegionalConfiguration()
            getTITOPrinterConfiguration()
            fetchUserWidgets()
            getTerminalTypeConfig()
            getPostalServices()
            getLocationTypes()
          })
          .catch((error) => {
            console.error('Heartbeat lost. Attempting to reconnect...', error)
          })
      }
    }
  }, 500)

  const token = globalStore.getState().accessToken

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${baseUrl}/${hub}?token=${token}`)
    // .configureLogging(signalR.LogLevel.Information)
    .build()

  try {
    connection.on('Heartbeat', (payload) => {
      onHandleHeartbeat(payload)
    })

    connection.on('MessageUpdated', (payload) => {
      onHandleMessageUpdated(payload)
    })

    connection.on('CMSConnectivityStatus', (payload) => {
      onHandleCMSConnectivityStatus(payload)
    })

    connection.on('UserWidgetsUpdated', (payload) => {
      onHandleUserWidgetsUpdate(payload)
    })

    if (connection.state !== signalR.HubConnectionState.Connected) {
      await connection.start().then(() => {
        getLocalizations()
        getRegionalConfiguration()
        getTITOPrinterConfiguration()
        fetchUserWidgets()
        getPostalServices()
        getLocationTypes()
        getTerminalTypeConfig()
      })
      heartbeatEstablished()
    }
  } catch (err) {
    heartbeatStopped()
  }
}

//==============> Message handlers

const onHandleHeartbeat = (payload) => {
  var data = JSON.parse(payload)

  if (data?.appVersion) {
    globalStore.getState().actions.setAppVersion(data?.appVersion)
  }

  if (data?.justMode) {
    if (data?.mode === 0) {
      setAppMode(AppMode.UNKNOWN_USER)
    }

    if (data?.mode === 1) {
      setAppMode(AppMode.OPERATOR)
    }
  } else {
    deviceStore.getState().setDevices(data?.devices ?? [])
  }
}

const onHandleMessageUpdated = (payload) => {
  var data = JSON.parse(payload)
  if (data) updateMessage(data)
}

const onHandleCMSConnectivityStatus = (payload) => {
  var data = JSON.parse(payload)
  if (data) setCmsStatus(data)
}

const onHandleUserWidgetsUpdate = (payload) => {
  const data = JSON.parse(payload)

  if (!data) return

  const storeActions = getUserWidgetsActions()
  storeActions.setWidgets(data)
}

//==============> Exports

export const useHeartbeat = () => {
  useEffect(() => {
    startConnection()
  }, [])
}
