/* eslint-disable prettier/prettier */
import './assets/css/index.css'
import { HashRouter } from 'react-router-dom'
import HistoryTracker from '@ui/components/history-tracker'
import { Mediator } from '@src/app/infrastructure/command-system'
import { default as creditCommands } from '@domain/transactions/commands'
import { default as operatorCardCommands } from '@domain/operator/commands'
import { useHeartbeat } from '@src/app/infrastructure/web-socket/heartbeat'
import { startConnection, startDeviceHubConnection } from '@src/app/infrastructure/web-socket'
import { SocketService as TransactionSocketService } from '@domain/transactions/services/socket'
import { SocketService as OperatorSocketService } from '@domain/operator/services/socket'
import { SocketService as ErrorMessageSocketService } from '@domain/global/services/socket'
import { SocketService as OperatorCardReaderSocketService } from '@domain/administration/services/socket'
import { useEffect, useState } from 'react'
import { CommandType } from '@domain/transactions/commands'
import {
  globalStore,
  useActiveTerminalTypes,
  useAppMode,
  useUpgradeInProgress
} from '@domain/global/stores'
import { APP_ROUTERS, AppMode } from '@domain/global/constants'
import SafeRouter from '@ui/routers/atm-casino/safe-mode-router'
import OperatorRouter from '@ui/routers/atm-casino/operator-router'
import UserRouterCasino from '@ui/routers/atm-casino/user-mode-router'
import ParcelLockerUserRouter from '@ui/routers/parcel-locker/user-router'
import RetailRouter from '@ui/routers/retail/user-router'
import AtmUserRouter from '@ui/routers/atm/user-router'
import MainLayout from './layouts/main-layout'
import { QueryClientProvider } from '@tanstack/react-query'
import { QueryClient } from '@src/app/infrastructure/query-client'
import { ErrorBoundary } from 'react-error-boundary'
import GeneralError from '@ui/components/general-error'
import { NotificationsContainer } from '@ui/utility/notifications'
import ExperienceSelector from '@ui/components/experience-selector'
import CasinoIdleScreen from '@ui/screens/atm-casino/idle'
import RetailIdleScreen from '@ui/screens/retail/idle'
import ParcelLockerIdleScreen from '@ui/screens/parcel-locker/idle'
import AtmIdleScreen from '@ui/screens/atm/idle'
import { NavigationProvider } from '@ui/components/navigation-provider'
import FloatingLoading from '@ui/components/floating-loading'
import { useIsIdle } from '@src/app/domain/global/stores/idle'

Mediator.clear()
Mediator.registerMultiple([...creditCommands, ...operatorCardCommands])

const IDLE_SCREEN_MAP = {
  gamingatm: CasinoIdleScreen,
  parcellocker: ParcelLockerIdleScreen,
  bankingatm: AtmIdleScreen,
  entertainment: RetailIdleScreen
}

const App = () => {
  const idle = useIsIdle()
  useHeartbeat()
  const appMode = useAppMode()
  const upgrade = useUpgradeInProgress()
  const currentTerminalTypes = useActiveTerminalTypes()

  const [currentIdleIndex, setCurrentIdleIndex] = useState(0)

  const selectedRouter = globalStore((state) => state.selectedRouter)

  const handleRouterChange = (value) => {
    globalStore.getState().actions.setSelectedRouter(value)
  }

  useEffect(() => {
    startConnection([TransactionSocketService, OperatorSocketService])
      .then(() => {
        console.info('WS connected successfully')
        Mediator.dispatch(CommandType.GetCreditsAmount, {})
      })
      .catch((error) => {
        console.error('WS disconnected...', error)
      })

    startDeviceHubConnection([ErrorMessageSocketService, OperatorCardReaderSocketService])
      .then(() => {
        console.info('WS device connected successfully')
      })
      .catch((error) => {
        console.error('WS device disconnected...', error)
      })
  }, [])

  useEffect(() => {
    if (upgrade) {
      const now = Date.now()

      if (now - upgrade < 5000) {
        const timeout = setTimeout(() => {
          window.location.reload()
        }, 5000)
        return () => clearTimeout(timeout)
      }
    }
  }, [upgrade])

  useEffect(() => {
    if (idle && currentTerminalTypes && currentTerminalTypes.length > 1) {
      const interval = setInterval(() => {
        setCurrentIdleIndex((prevIndex) => (prevIndex + 1) % currentTerminalTypes.length)
      }, 5000)

      return () => clearInterval(interval)
    } else if (!idle) {
      setCurrentIdleIndex(0)
    }
  }, [idle, currentTerminalTypes])

  const showSafeMode = appMode === AppMode.SAFE && (!upgrade || Date.now() - upgrade > 60 * 1000)

  const showUpgrade = upgrade && Date.now() - upgrade < 5000

  const getUserRouter = () => {
    if (selectedRouter === APP_ROUTERS.GAMING_ATM) {
      return <UserRouterCasino />
    } else if (selectedRouter === APP_ROUTERS.PARCEL_LOCKER) {
      return <ParcelLockerUserRouter />
    } else if (selectedRouter === APP_ROUTERS.BANKING_ATM) {
      return <AtmUserRouter />
    } else if (selectedRouter === APP_ROUTERS.ENTERTAINMENT) {
      return <RetailRouter />
    }
  }

  const getIdleScreen = () => {
    if (!currentTerminalTypes || currentTerminalTypes.length === 0) return null

    // Modulo deviding to ensure we loop through available types
    const typeKey = currentTerminalTypes[currentIdleIndex % currentTerminalTypes.length]
    const IdleComponent = IDLE_SCREEN_MAP[typeKey]

    return IdleComponent ? <IdleComponent /> : null
  }

  return (
    <>
      <NotificationsContainer />
      <ErrorBoundary FallbackComponent={GeneralError} onReset={() => {}}>
        <QueryClientProvider client={QueryClient}>
          <HashRouter>
            <HistoryTracker>
              <NavigationProvider>
                <MainLayout>
                  {idle && !showUpgrade && !showSafeMode && getIdleScreen()}
                  {selectedRouter === null && !idle && (
                    <ExperienceSelector onChange={handleRouterChange} />
                  )}
                  {showUpgrade && <FloatingLoading text="Upgrade in progress..." />}
                  {!idle && appMode === AppMode.UNKNOWN_USER && getUserRouter()}
                  {appMode === AppMode.USER && getUserRouter()}
                  {showSafeMode && <SafeRouter />}
                  {appMode === AppMode.OPERATOR && <OperatorRouter />}
                </MainLayout>
              </NavigationProvider>
            </HistoryTracker>
          </HashRouter>
        </QueryClientProvider>
      </ErrorBoundary>
    </>
  )
}

export default App
