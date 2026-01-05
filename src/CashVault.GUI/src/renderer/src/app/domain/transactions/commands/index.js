/* eslint-disable prettier/prettier */
import { Command } from '@src/app/infrastructure/command-system'
import { MessageType, sendMessage } from '@src/app/infrastructure/web-socket'
import { creditsStore } from '@domain/transactions/store'
import { globalStore } from '@domain/global/stores'
import { v4 } from 'uuid'
// import { getMessage } from '../../global/stores/socket-message-store'
import Notifications from '@src/app/services/notifications'
import { SocketService } from '@domain/transactions/services/socket'
import { deviceConfigurationStore } from '../../device/stores/device-configuration-store'
// import { isEmpty } from 'lodash'

export const CommandType = {
  GetCreditsAmount: 'GetCreditsAmount',
  RequestPayout: 'RequestPayout'
}

export const getCreditsAmount = new Command(CommandType.GetCreditsAmount, () => {
  creditsStore?.getState().actions.toggleLoading()

  sendMessage({
    messageType: MessageType.GetCreditsAmount
  })
})

export const requestPayout = new Command(CommandType.RequestPayout, (data) => {
  if (globalStore?.getState().webSocketConnected == false) {
    Notifications.error('No connection to backbone service.')
    return
  }

  creditsStore
    ?.getState()
    .actions.setPayoutRequestedTotal(data.billSpecification, data.ticketAmount)
  creditsStore?.getState().actions.togglePayoutProcessing(true)

  const timeoutInterval = deviceConfigurationStore?.getState().titoPrinterTimeout
  const bufferTime = 8000 // Additional buffer time to ensure processing completes

  sendMessage({
    messageType: MessageType.PayoutRequested,
    requestId: v4(),
    ...data
  })

  setTimeout(() => {
    if (creditsStore?.getState().payoutProcessingCompleted == false) {
      creditsStore?.getState().actions.addDetailedPayoutMessage(`Request Canceled!`)

      creditsStore?.getState().actions.addPayoutMessage({
        isCash: false,
        isTicket: true,
        failed: true,
        amount: data?.billSpecification?.reduce(
          (total, bill) => total + bill.denomination * bill.count,
          0
        )
      })

      creditsStore?.getState().actions.addPayoutMessage({
        isCash: true,
        isTicket: false,
        failed: true,
        amount: data.ticketAmount
      })

      creditsStore.getState().actions.setPayoutProcessingCompleted(true)

      SocketService.process(
        MessageType.PayoutFailed,
        JSON.stringify({ reason: 'Canceled', ticketAmountPayedOut: 0, cashAmountPayedOut: 0 })
      )
    }
  }, timeoutInterval + bufferTime)
})

const commands = [getCreditsAmount, requestPayout]

export default commands
