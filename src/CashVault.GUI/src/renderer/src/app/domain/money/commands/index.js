/* eslint-disable prettier/prettier */
import { Command } from '@src/app/infrastructure/command-system'
import { sendMessage } from '@src/app/infrastructure/web-socket'
import { useMutation } from '@tanstack/react-query'
import { post } from '@src/app/infrastructure/api'
import { v4 } from 'uuid'

export const CommandType = {
  GetBillDispenserData: 'GetBillDispenserData',
  GetBillAcceptorData: 'GetBillAcceptorData',
  RequestBillCountChange: 'RequestBillCountChange'
}

export const MessageType = {
  GetBillDispenserData: 'GetBillDispenserData',
  GetBillAcceptorData: 'GetBillAcceptorData',
  BillCountChangeRequested: 'BillCountChangeRequested'
}

export const getBillDispenserData = new Command(CommandType.GetBillDispenserData, () => {
  sendMessage({
    messageType: MessageType.GetBillDispenserData
  })
})

export const getBillAcceptorData = new Command(CommandType.GetBillAcceptorData, () => {
  sendMessage({
    messageType: MessageType.GetBillAcceptorData
  })
})

export const requestBillCountChange = new Command(
  CommandType.RequestBillCountChange,
  (casseteId, count) => {
    // toggle loading

    let uuid = v4()

    sendMessage({
      uuid,
      messageType: CommandType.BillCountChangeRequested,
      casseteId,
      count
    })

    // check if message success received
  }
)
//===========================================================================

const sendBetboxTicketAck = async (data) => {
  const result = await post(`api/betbox/ticket/ack`, data)

  return result
}

export const useSendBetboxTicketAck = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: sendBetboxTicketAck,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      onSuccess()
    }
  })
}

//===========================================================================

const sendBetboxTicketNack = async (data) => {
  const result = await post(`api/betbox/ticket/nack`, data)

  return result
}

export const useSendBetboxTicketNack = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: sendBetboxTicketNack,
    onError: () => {
      onError()
    },
    onSuccess: () => {
      onSuccess()
    }
  })
}
