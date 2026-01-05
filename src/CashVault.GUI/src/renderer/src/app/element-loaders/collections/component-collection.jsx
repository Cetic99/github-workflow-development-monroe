/* eslint-disable prettier/prettier */


export const Component = {
  TestComponent: 'TestComponent',
  TicketPrinterComponent: 'TicketPrinterComponent',
  MoneyWithdrawComponent: 'MoneyWithdrawComponent',
  BetboxTicketComponent: 'BetboxTicketComponent'
}

const ComponentCollection = {
  [Component.TicketPrinterComponent]: () => import('@ui/components/widgets/tito-printer'),
  [Component.MoneyWithdrawComponent]: () => import('@ui/components/widgets/money-withdraw'),
  [Component.BetboxTicketComponent]: () => import('@ui/components/widgets/betbox-ticket')
}

export default ComponentCollection
