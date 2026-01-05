/* eslint-disable prettier/prettier */

export const EventLogType = {
  DEVICE_EVENT_LOG: 'DeviceEvent',
  TRANSACTION_EVENT_LOG: 'TransactionEvent',
  FAIL_EVENT_LOG: 'DeviceFailEvent',
  WARNING_EVENT_LOG: 'DeviceWarningEvent'
}

export const RecordType = {
  EVENT: 'event',
  TRANSACTION: 'transaction',
  FAIL: 'fail',
  TICKET: 'ticket'
}

export const TransactionKind = {
  TicketTransaction: 'TicketTransaction',
  DispenserBillTransaction: 'DispenserBillTransaction',
  AcceptorBillTransaction: 'AcceptorBillTransaction'
}
