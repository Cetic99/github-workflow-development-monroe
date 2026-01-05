/* eslint-disable prettier/prettier */
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'
import { useEffect } from 'react'
import { useAcceptingInProgress } from '@domain/transactions/store'
import { useCreditsAmount } from '@domain/transactions/store'
import { useLogout } from '@domain/global/commands'
import {
  globalStore,
  useActiveTerminalTypes,
  useGlobalActions,
  useIsAuthenticated
} from '@domain/global/stores/index'
import { useIdle } from '@uidotdev/usehooks'

export const idleStore = create(
  immer((set) => ({
    isIdle: false,
    // =======================================
    actions: {
      setIdleState: (_state) => {
        set((state) => {
          state.isIdle = _state
        })
      }
    }
  }))
)

export const useIsIdle = () => idleStore((state) => state.isIdle)

export const getIsIdle = () => idleStore.getState().isIdle

export const setIsIdle = (state) => idleStore.getState().actions.setIdleState(state)

export const useIdleScreen = (navigate) => {
  const idle = useIdle(150000)

  const accepting = useAcceptingInProgress()
  const creditsAmount = useCreditsAmount()
  const authenticated = useIsAuthenticated()
  const currentTerminalTypes = useActiveTerminalTypes()

  const { logout } = useGlobalActions()
  const logoutCommand = useLogout()

  useEffect(() => {
    if (idle && authenticated) {
      logoutCommand.mutate()
      navigate('/')
      logout()
    }

    if (currentTerminalTypes.length > 1) {
      globalStore.getState().actions.setSelectedRouter(null)
    }

    setIsIdle(idle)
  }, [idle])

  useEffect(() => {
    if (accepting || creditsAmount?.amount > 0) {
      setIsIdle(false)
    }
  }, [accepting, creditsAmount])
}
