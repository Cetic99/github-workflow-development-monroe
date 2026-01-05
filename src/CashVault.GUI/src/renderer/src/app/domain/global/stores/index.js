/* eslint-disable prettier/prettier */
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'
import { APP_ROUTERS, AppMode, TERMINAL_TYPES } from '@domain/global/constants'
import { decodeToken } from '@domain/operator/services'
import { createJSONStorage, persist } from 'zustand/middleware'

export const globalStore = create(
  persist(
    immer((set) => ({
      appVersion: '-.-.-',
      loading: false,
      isInitialized: false,
      loginOpen: false,
      authenticated: false,
      authFailed: undefined,
      operatorName: null,
      operatorId: null,
      accessToken: null,
      refreshToken: null,
      mode: AppMode.UNKNOWN_USER,
      permissions: [],
      webSocketConnected: false,
      screenHeader: 'Header',
      heartbeat: false,
      upgrade: null,
      selectedRouter: APP_ROUTERS.GAMING_ATM,
      isMultipleRoutersEnabled: false,
      cmsStatus: {
        isCasinoManagementSystem: false,
        isConnected: false
      },
      terminalTypeConfig: {
        selectedTerminalTypes: [],
        supportedDevices: [],
        terminalTypeToDevicesMap: {}
      },

      // =======================================
      actions: {
        setTerminalTypeConfig: (config) => {
          set((state) => {
            state.terminalTypeConfig = config
            state.isMultipleRoutersEnabled = config?.selectedTerminalTypes?.length > 1
            state.selectedRouter =
              config?.selectedTerminalTypes?.length === 1
                ? config.selectedTerminalTypes[0] === TERMINAL_TYPES.PARCEL_LOCKER
                  ? APP_ROUTERS.PARCEL_LOCKER
                  : APP_ROUTERS.GAMING_ATM
                : state.selectedRouter
          })
        },
        setInitialized: (initialized) => {
          set((state) => {
            state.isInitialized = initialized
          })
        },
        setAppVersion: (version) => {
          set((state) => {
            state.appVersion = version
          })
        },
        setAppMode: (mode) => {
          set((state) => {
            if (mode > -1 && state.mode !== mode) state.mode = mode
          })
        },
        heartbeatEstablished: () => {
          set((state) => {
            if (!state.heartbeat) state.heartbeat = true

            if (!state.heartbeat && state.mode === AppMode.SAFE) {
              state.mode = AppMode.UNKNOWN_USER
            }
          })
        },
        heartbeatStopped: () => {
          set((state) => {
            if (state.heartbeat) {
              state.heartbeat = false
            }

            if (!state.heartbeat) {
              state.mode = AppMode.SAFE
            }
          })
        },
        setLoginOpen: (open = false) => {
          set((state) => {
            state.loginOpen = open
          })
        },
        setAuthFailed: (failed) => {
          set((state) => {
            state.authFailed = failed
          })
        },
        setLoading: (loading = false) => {
          set((state) => {
            state.loading = loading
          })
        },
        login: (accessToken, refreshToken) => {
          const decoded = decodeToken(accessToken)

          set((state) => {
            state.authenticated = true
            // state.mode = AppMode.OPERATOR
            state.permissions = decoded.permissions
            state.accessToken = accessToken
            state.refreshToken = refreshToken
            state.operatorName = decoded.username
            state.operatorId = decoded.id
            state.authFailed = undefined
          })
        },
        logout: () => {
          set((state) => {
            state.authenticated = false
            state.mode = AppMode.UNKNOWN_USER
            state.permissions = []
            state.accessToken = null
            state.refreshToken = null
            state.operatorName = null
            state.authFailed = undefined
          })
        },
        connected: () => {
          set((state) => {
            state.webSocketConnected = true
          })
        },
        disconnected: () => {
          set((state) => {
            state.webSocketConnected = false
          })
        },
        setScreenHeader: (header) => {
          set((state) => {
            state.screenHeader = header
          })
        },
        setCmsStatus: (status) => {
          set((state) => {
            state.cmsStatus.isCasinoManagementSystem = status?.IsCasinoManagementSystem ?? false
            state.cmsStatus.isConnected = status?.IsConnected ?? false
          })
        },
        setUpgrade: () => {
          set((state) => {
            state.upgrade = Date.now()
          })
        },
        setSelectedRouter: (router) => {
          set((state) => {
            state.selectedRouter = router
          })
        }
      }
    })),
    {
      name: 'global-store', //localStorage key,
      storage: createJSONStorage(() => sessionStorage),
      partialize: (state) => ({
        upgrade: state.upgrade,
        accessToken: state.accessToken,
        authenticated: state.authenticated,
        permissions: state.permissions,
        refreshToken: state.refreshToken,
        operatorName: state.operatorName,
        operatorId: state.operatorId
      }),
      onRehydrateStorage: () => {
        return (state) => {
          // we delay isInitialization prop due to WS connection
          setTimeout(() => {
            state.actions.setInitialized(true)
          }, 200)
        }
      }
    }
  )
)

// ==============================================================================

export const useTerminalTypeConfig = () => globalStore((state) => state.terminalTypeConfig)

export const useActiveTerminalTypes = () =>
  globalStore((state) => state.terminalTypeConfig?.selectedTerminalTypes || [])

export const setTerminalTypeConfig = (config) => {
  globalStore.getState().actions.setTerminalTypeConfig(config)
}

export const useSupportedDevices = () =>
  globalStore.getState().terminalTypeConfig?.supportedDevices || []

export const isDeviceSupported = (device) => {
  const supportedDevices = globalStore.getState().terminalTypeConfig?.supportedDevices || []
  return supportedDevices.includes(device)
}

export const getDevicesByTerminalTypes = (terminalTypes = []) => {
  const terminalTypeConfig = globalStore.getState().terminalTypeConfig

  if (!terminalTypeConfig || !terminalTypeConfig.terminalTypeToDevicesMap) {
    console.warn('terminalTypeConfig or terminalTypeToDevicesMap is missing', terminalTypeConfig)
    return []
  }

  const map = terminalTypeConfig.terminalTypeToDevicesMap
  const devicesSet = new Set()

  terminalTypes.forEach((type) => {
    const devices = map[type] || []
    devices.forEach((d) => devicesSet.add(d))
  })

  return [...devicesSet]
}

export const useHeartbeat = () => globalStore((state) => state.heartbeat)

export const useAppVersion = () => globalStore((state) => state.appVersion)

export const getHeartbeat = () => globalStore.getState().heartbeat

export const heartbeatEstablished = () => {
  globalStore.getState().actions.heartbeatEstablished()
}

export const heartbeatStopped = () => {
  globalStore.getState().actions.heartbeatStopped()
}

export const setAppMode = (mode) => {
  globalStore.getState().actions.setAppMode(mode)
}

// ==============================================================================

export const useIsAuthenticated = () => globalStore((state) => state.authenticated)
export const useIsMultipleRoutersEnabled = () =>
  globalStore((state) => state.isMultipleRoutersEnabled)

export const usePermissions = () => globalStore((state) => state.permissions)

export const useHasPermission = (permission) =>
  globalStore((state) => {
    if (state?.permissions?.includes(permission)) return true

    return false
  })

export const useWebSocketConnected = () => globalStore((state) => state.webSocketConnected)

export const useAppMode = () => globalStore((state) => state.mode)

export const useScreenHeader = () => globalStore((state) => state.screenHeader)

export const login = (accessToken, refreshToken) => {
  globalStore.getState().actions.login(accessToken, refreshToken)
}

export const logout = () => {
  globalStore.getState().actions.logout()
}

export const setLoading = (loading) => {
  globalStore.getState().actions.setLoading(loading)
}

export const setLoginOpen = (open) => {
  globalStore.getState().actions.setLoginOpen(open)
}

export const setAuthFailedStatus = (status) => {
  globalStore.getState().actions.setAuthFailed(status)
}

export const getAuthFailedStatus = () => globalStore.getState().authFailed

export const getLoginOpen = () => globalStore.getState().loginOpen

export const useLoading = () => globalStore((state) => state.loading)

export const useOperatorName = () => globalStore((state) => state.operatorName)

export const useOperatorId = () => globalStore((state) => state.operatorId)

export const getOperatorId = () => globalStore.getState().operatorId

export const getOperatorName = () => globalStore.getState().operatorName

export const useLoginOpen = () => globalStore((state) => state.loginOpen)

export const getAccessToken = () => globalStore.getState().accessToken

export const getRefreshToken = () => globalStore.getState().refreshToken

export const useAccessToken = () => globalStore((state) => state.accessToken)
export const useRefreshToken = () => globalStore((state) => state.refreshToken)

export const useGlobalActions = () => globalStore((state) => state.actions)

export const useCmsStatus = () => globalStore((state) => state.cmsStatus)

export const useIsInitialized = () => globalStore((state) => state.isInitialized)

export const useUpgradeInProgress = () => globalStore((state) => state.upgrade)

export const setCmsStatus = (status) => {
  globalStore.getState().actions.setCmsStatus(status)
}
