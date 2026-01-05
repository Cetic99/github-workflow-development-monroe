import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export const deviceConfigurationStore = create(
  immer((set) => ({
    titoPrinterTimeout: 2000000,
    //=============================
    actions: {
      setPrinterTimeout: (interval) => {
        set((state) => {
          state.titoPrinterTimeout = interval
        })
      }
    }
  }))
)

export const loadDeviceConfiguration = (interval) => {
  deviceConfigurationStore.getState().actions.setPrinterTimeout(interval)
}

// ========================================== //
export const useGetTitoPrinterTimeout = () =>
  deviceConfigurationStore((state) => state.titoPrinterTimeout)
