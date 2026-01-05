/* eslint-disable prettier/prettier */
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export const userWidgetsStore = create(
  immer((set) => ({
    widgets: [],

    // =======================================

    actions: {
      setWidgets: (widgets = []) => {
        set((state) => {
          state.widgets = widgets.map((w) => {
            return {
              uuid: w.Uuid,
              code: w.Code,
              displaySequence: w.DisplaySequence,
              size: w.Size.Code
            }
          })
        })
      },
      initialize: (widgets) => {
        set((state) => {
          state.widgets = widgets
        })
      }
    }
  }))
)

export const useUserWidgets = () =>
  userWidgetsStore((state) => {
    return state.widgets
  })

export const getUserWidgets = () => {
  return userWidgetsStore.getState().widgets
}

export const useUserWidgetsActions = () => userWidgetsStore((state) => state.actions)

export const getUserWidgetsActions = () => {
  const actions = userWidgetsStore.getState().actions
  return {
    setWidgets: actions.setWidgets,
    initialize: actions.initialize
  }
}

export const initializeUserWidgets = (widgets) => {
  userWidgetsStore.getState().actions.initialize(widgets)
}
