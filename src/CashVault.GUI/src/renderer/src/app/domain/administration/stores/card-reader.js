/* eslint-disable prettier/prettier */

import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'
import { isEmpty } from 'lodash'

const CardReaderSteps = ['InitializeCardReader', 'ScanUserCard', 'CreateIdentificationCard']

const init = CardReaderSteps.reduce((acc, step, index) => {
  acc[step] = {
    active: index === 0, // processing
    success: null
  }
  return acc
}, {})

const getActiveStep = (steps) => {
  const activeKey = Object.keys(steps).find((key) => steps[key].active === true)

  if (isEmpty(activeKey)) {
    return { key: null, index: -1 }
  }

  return {
    key: activeKey,
    index: CardReaderSteps.indexOf(activeKey),
    ...steps[activeKey]
  }
}

export const cardStore = create(
  immer((set) => ({
    steps: { ...init },
    operatorId: 0,
    // --- Actions ---
    actions: {
      reset: () => {
        set((state) => {
          state.steps = { ...init }
        })
      },
      fail: () => {
        set((state) => {
          const { key, index } = getActiveStep(state.steps)

          if (index < 0 || isEmpty(key)) {
            return
          }

          CardReaderSteps.slice(index).forEach((step) => {
            state.steps[step].success = false
          })
        })
      },
      success: (step) => {
        set((state) => {
          const stepIdx = CardReaderSteps.indexOf(step)

          if (stepIdx < 0) {
            return
          }

          state.steps[step].active = false
          state.steps[step].success = true

          if (stepIdx + 1 < CardReaderSteps.length) {
            const nextStep = CardReaderSteps[stepIdx + 1]
            state.steps[nextStep].active = true
          }
        })
      },
      failStep: (step) => {
        set((state) => {
          const stepIdx = CardReaderSteps.indexOf(step)

          if (stepIdx < 0) {
            return
          }

          if (!state.steps[step].active) {
            return
          }

          CardReaderSteps.slice(stepIdx).forEach((step) => {
            state.steps[step].success = false
          })
        })
      },
      setOperator: (id) => {
        set((state) => {
          state.operatorId = id
        })
      }
    }
  }))
)

export const useStep = (step) => cardStore((state) => state.steps[step])
export const getStep = (step) => cardStore.getState().steps[step]

export const useCardOperatorId = () => cardStore((state) => state.operatorId)
export const getCardOperatorId = () => cardStore.getState().operatorId

export const useActiveStep = () => cardStore((state) => getActiveStep(state.steps))
export const getActiveStepState = () => getActiveStep(cardStore.getState().steps)

export const useAllStepsCompleted = () =>
  cardStore((state) => {
    return CardReaderSteps.every((step) => state.steps[step].success === true)
  })

export const useAnyFailedSteps = () =>
  cardStore((state) => {
    return CardReaderSteps.findIndex((step) => state.steps[step].success === false) >= 0
  })

export const getFirstFailedStep = () =>
  cardStore((state) => {
    return CardReaderSteps.findIndex((step) => state.steps[step].success === false)
  })

export const useCardStoreActions = () => cardStore((state) => state.actions)

export const fail = () => cardStore.getState().actions.fail()
export const failStep = (step) => cardStore.getState().actions.failStep(step)
export const completeStep = (step) => cardStore.getState().actions.success(step)
