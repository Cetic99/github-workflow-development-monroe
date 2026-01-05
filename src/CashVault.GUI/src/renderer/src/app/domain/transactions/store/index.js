/* eslint-disable prettier/prettier */
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export const creditsStore = create(
  immer((set) => ({
    amount: 0,
    amountPrecision: 2,
    currency: 'BAM',
    currencySymbol: 'KM',
    payoutProcessing: false,
    loading: false,
    // ---------------------------------------
    payoutProcessingCompleted: false,
    payoutRequestedTotal: {
      billSpecification: [],
      ticketAmount: 0
    },
    payoutMessages: [],
    detailedPayoutMessages: [],
    // ---------------------------------------
    acceptingInProgress: false,
    acceptingMessages: [],
    // =======================================
    actions: {
      setPayoutRequestedTotal: (billSpecification = [], ticket = 0) => {
        set((state) => {
          state.payoutRequestedTotal.billSpecification = billSpecification
          state.payoutRequestedTotal.ticketAmount = ticket
        })
      },
      addDetailedPayoutMessage: (message) => {
        set((state) => {
          state.detailedPayoutMessages.push(message)
        })
      },
      clearDetailedPayoutMessages: () => {
        set((state) => {
          state.detailedPayoutMessages = []
        })
      },
      addPayoutMessage: (message) => {
        set((state) => {
          state.payoutMessages.push(message)
        })
      },
      clearPayoutMessages: () => {
        set((state) => {
          state.payoutMessages = []
        })
      },
      // ---------------------------------------
      addAcceptingMessage: (message) => {
        set((state) => {
          state.acceptingMessages.push(message)
        })
      },
      clearAcceptingMessages: () => {
        set((state) => {
          state.acceptingMessages = []
        })
      },
      toggleAcceptingInProgress: (value = null) => {
        set((state) => {
          if (value === state.acceptingInProgress) return

          if (value) {
            state.acceptingInProgress = value
          } else {
            state.acceptingInProgress = !state.acceptingInProgress
          }
        })
      },
      // ---------------------------------------
      setPayoutProcessingCompleted: (value) => {
        set((state) => {
          state.payoutProcessingCompleted = value
        })
      },
      toggleLoading: (value) => {
        set((state) => {
          if (value) {
            state.loading = value
          } else {
            state.loading = !state.loading
          }
        })
      },
      togglePayoutProcessing: (value = null) => {
        set((state) => {
          if (value) {
            if (value === true) {
              //if processing is started, then clear messages
              state.payoutMessages = []
              state.detailedPayoutMessages = []
            }
            state.payoutProcessing = value
          } else {
            if (!state.payoutProcessing === true) {
              //if processing is started, then clear messages
              state.payoutMessages = []
              state.detailedPayoutMessages = []
            }
            state.payoutProcessing = !state.payoutProcessing
          }
        })
      },

      setCreditsAmount: (amount, currency, amountPrecision, currencySymbol) => {
        set((state) => {
          state.amount = amount
        })
        set((state) => {
          state.amountPrecision = amountPrecision || 2
        })
        set((state) => {
          state.currency = currency || 'BAM'
        })
        set((state) => {
          state.currencySymbol = currencySymbol || 'KM'
        })
      },
      increaseCreditsAmount: (amount) => {
        set((state) => {
          state.amount = state.amount + amount
        })
      },
      decreaseCreditsAmount: (amount) => {
        set((state) => {
          state.amount = state.amount + amount
        })
      },
      payoutCredits: (ticketAmount = 0, cashAmount = 0) => {
        set((state) => {
          state.amount = state.amount - ticketAmount - cashAmount
        })
      }
    }
  }))
)

export const setPayoutRequestedTotal = (billSpecification, ticket) =>
  creditsStore.getState().actions.setPayoutRequestedTotal(billSpecification, ticket)

export const usePayoutRequestedTotal = () => creditsStore((state) => state.payoutRequestedTotal)

export const usePayoutMessages = () => creditsStore((state) => state.payoutMessages)

export const addPayoutMessage = (message) =>
  creditsStore.getState().actions.addPayoutMessage(message)

export const clearPayoutMessages = () => creditsStore.getState().actions.clearPayoutMessages()

export const useDetailedPayoutMessages = () => creditsStore((state) => state.detailedPayoutMessages)

export const addDetailedPayoutMessage = (message) =>
  creditsStore.getState().actions.addDetailedPayoutMessage(message)

export const clearDetailedPayoutMessages = () =>
  creditsStore.getState().actions.clearDetailedPayoutMessages()

export const useAcceptingInProgress = () => creditsStore((state) => state.acceptingInProgress)
export const useAcceptingMessages = () => creditsStore((state) => state.acceptingMessages)

export const useCreditsAmount = () => {
  return {
    amount: creditsStore((state) => state.amount),
    currency: creditsStore((state) => state.currency),
    currencySymbol: creditsStore((state) => state.currencySymbol),
    amountPrecision: creditsStore((state) => state.amountPrecision)
  }
}
export const usePayoutProcessing = () => creditsStore((state) => state.payoutProcessing)
export const usePayoutProcessingCompleted = () =>
  creditsStore((state) => state.payoutProcessingCompleted)
export const useCreditsLoading = () => creditsStore((state) => state.loading)

export const useCreditsActions = () => creditsStore((state) => state.actions)
