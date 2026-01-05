/* eslint-disable prettier/prettier */

export const Icon = {
  TicketPrinterComponent: 'TicketPrinterComponent',
  MoneyWithdrawComponent: 'MoneyWithdrawComponent'
}

const IconCollection = {
  [Icon.TicketPrinterComponent]: () => import('@ui/components/icons/IconReceipt'),
  [Icon.MoneyWithdrawComponent]: () => import('@ui/components/icons/IconBanknote01')
}

export default IconCollection
