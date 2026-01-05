/* eslint-disable prettier/prettier */

import { MessageType } from '@src/app/infrastructure/web-socket/index'
import {
  creditsStore,
  addPayoutMessage,
  addDetailedPayoutMessage
} from '@domain/transactions/store'
import Notifications from '@src/app/services/notifications'
import { addOperationMessage } from '@domain/device/stores'
import { translate } from '@domain/administration/stores'

const receiveCreditsAmount = (_payload) => {
  const payload = JSON.parse(_payload)

  creditsStore
    .getState()
    .actions.setCreditsAmount(
      payload.amount,
      payload.currency,
      payload.amountPrecision,
      payload.currencySymbol
    )
  creditsStore.getState().actions.toggleLoading(false)
}

const receiveTicketPrintingCompleted = (_payload) => {
  const payload = JSON.parse(_payload)

  addDetailedPayoutMessage(translate(`Ticket printing completed`))
  addDetailedPayoutMessage(
    `${translate('Requested ticket amount')}: ${payload.amountRequested} - ${translate('payed out')}: ${payload.amountPayedOut}`
  )

  if (payload.amountRequested !== payload.amountPayedOut) {
    addPayoutMessage({
      isCash: false,
      isTicket: true,
      failed: true,
      amount: payload.amountRequested - payload.amountPayedOut
    })
  }

  addPayoutMessage({
    isCash: false,
    isTicket: true,
    failed: false,
    amount: payload.amountPayedOut
  })
}

const receiveTicketPrintingFailed = (_payload) => {
  const payload = JSON.parse(_payload)

  addDetailedPayoutMessage(`${translate('Ticket dispensing failed')}: ${translate(payload.reason)}`)
  addDetailedPayoutMessage(
    `${translate('Requested ticket amount')}: ${payload.amountRequested} - ${translate('payed out')}: ${payload.amountPayedOut}`
  )

  addPayoutMessage({
    isCash: false,
    isTicket: true,
    failed: true,
    amount: payload.amountRequested,
    reason: payload.reason
  })
}

const receiveBillDispensingCompleted = (_payload) => {
  const payload = JSON.parse(_payload)

  addDetailedPayoutMessage(`${translate('Bill dispensing completed')}`)

  if (payload.amountRequested !== payload.amountPayedOut)
    addDetailedPayoutMessage(`${translate('Bill dispensing is partial')}`)

  addDetailedPayoutMessage(
    `${translate('Requested bill amount')}: ${payload.amountRequested} - ${translate('payed out')}: ${payload.amountPayedOut}`
  )

  if (payload.amountRequested !== payload.amountPayedOut) {
    addPayoutMessage({
      isCash: true,
      isTicket: false,
      failed: true,
      amount: payload.amountRequested - payload.amountPayedOut
    })
  }

  addPayoutMessage({
    isCash: true,
    isTicket: false,
    failed: false,
    amount: payload.amountPayedOut
  })
}

const receiveBillDispensingFailed = (_payload) => {
  const payload = JSON.parse(_payload)

  addDetailedPayoutMessage(`${translate('Bill dispensing failed')}: ${translate(payload.reason)}`)
  addDetailedPayoutMessage(
    `${translate('Requested bill amount')}: ${payload.amountRequested} - ${translate('payed out')}: ${payload.amountPayedOut}`
  )

  addPayoutMessage({
    isCash: true,
    isTicket: false,
    failed: true,
    amount: payload.amountRequested
  })
}

const receivePayoutCompleted = (_payload) => {
  const payload = JSON.parse(_payload)

  addDetailedPayoutMessage(translate(`Payout/printing process completed`))

  creditsStore
    .getState()
    .actions.payoutCredits(payload.ticketAmountPayedOut, payload.cashAmountPayedOut)

  creditsStore.getState().actions.setPayoutProcessingCompleted(true)
}

const receivePayoutFailed = (_payload) => {
  const payload = JSON.parse(_payload)

  addDetailedPayoutMessage(translate(`Payout/printing process failed`))

  creditsStore
    .getState()
    .actions.payoutCredits(payload.ticketAmountPayedOut, payload.cashAmountPayedOut)

  creditsStore.getState().actions.setPayoutProcessingCompleted(true)
}

//========================================================================================================================

const receiveBillAcceptingStarted = (timeoutId) => {
  const actions = creditsStore.getState().actions
  actions.toggleAcceptingInProgress(true)

  if (timeoutId !== null) {
    // if timeout is already set, do not set it again!
    return
  }

  setTimeout(() => {
    if (
      creditsStore.getState().acceptingMessages?.length === 0 &&
      creditsStore.getState().acceptingInProgress === true
    ) {
      actions.toggleAcceptingInProgress(false)
      Notifications.error(translate('Error during bill/ticket accepting process'))
      timeoutId = null
    }
  }, 20000)
}

const receiveTicketAccepted = (_payload) => {
  clearTimeoutId()
  const payload = JSON.parse(_payload)
  const actions = creditsStore.getState().actions

  actions.addAcceptingMessage({
    success: true,
    text: translate('Ticket accepted')
  })

  actions.toggleAcceptingInProgress(false)

  actions.increaseCreditsAmount(payload.amount)
}

const receiveTicketRejected = () => {
  clearTimeoutId()
  const actions = creditsStore.getState().actions

  actions.addAcceptingMessage({
    success: false,
    text: translate('Ticket rejected')
  })
  actions.toggleAcceptingInProgress(false)
}

const receiveBillRejected = () => {
  clearTimeoutId()
  const actions = creditsStore.getState().actions

  actions.addAcceptingMessage({
    success: false,
    text: translate('Bill rejected')
  })
  actions.toggleAcceptingInProgress(false)
}

const receiveBillAccepted = (_payload) => {
  clearTimeoutId()
  const payload = JSON.parse(_payload)
  const actions = creditsStore.getState().actions

  actions.addAcceptingMessage({
    success: true,
    text: translate('Bill accepted')
  })
  actions.toggleAcceptingInProgress(false)

  actions.increaseCreditsAmount(payload.amount)
}

const receiveCoinAccepted = (_payload) => {
  clearTimeoutId()
  const payload = JSON.parse(_payload)
  const actions = creditsStore.getState().actions

  actions.addAcceptingMessage({
    success: true,
    text: translate('Coin accepted')
  })
  actions.toggleAcceptingInProgress(false)

  actions.increaseCreditsAmount(payload.amount)
}

const receiveCoinRejected = () => {
  clearTimeoutId()
  const actions = creditsStore.getState().actions

  actions.addAcceptingMessage({
    success: false,
    text: translate('Coin rejected')
  })
  actions.toggleAcceptingInProgress(false)
}

// Clear timeout if the bill accepting process is finished or failed
let timeoutId = null

function clearTimeoutId() {
  if (timeoutId) {
    clearTimeout(timeoutId)
    timeoutId = null
  }
}

const receiveOperationExecuted = (_payload) => {
  const payload = JSON.parse(_payload)

  addOperationMessage(payload)
}

export const SocketService = {
  process: (type, payload) => {
    switch (type) {
      case MessageType.SetCreditsAmount:
        receiveCreditsAmount(payload)
        break

      case MessageType.BillAccepting:
        timeoutId = receiveBillAcceptingStarted(timeoutId)
        break

      case MessageType.TicketAccepted:
        receiveTicketAccepted(payload)
        break

      case MessageType.TicketRejected:
        receiveTicketRejected(payload)
        break

      case MessageType.BillAccepted:
        receiveBillAccepted(payload)
        break

      case MessageType.BillRejected:
        receiveBillRejected(payload)
        break

      case MessageType.CoinAccepted:
        receiveCoinAccepted(payload)
        break

      case MessageType.CoinRejected:
        receiveCoinRejected(payload)
        break

      case MessageType.TicketPrintingCompleted:
        receiveTicketPrintingCompleted(payload)
        break

      case MessageType.TicketPrintingFailed:
        receiveTicketPrintingFailed(payload)
        break

      case MessageType.BillDispensingCompleted:
        receiveBillDispensingCompleted(payload)
        break

      case MessageType.BillDispensingFailed:
        receiveBillDispensingFailed(payload)
        break

      case MessageType.PayoutCompleted:
        receivePayoutCompleted(payload)
        break

      case MessageType.PayoutFailed:
        receivePayoutFailed(payload)
        break

      case MessageType.OperationExecuted:
        receiveOperationExecuted(payload)
        break

      default:
        break
    }
  }
}
