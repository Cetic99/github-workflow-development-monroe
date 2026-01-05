/* eslint-disable prettier/prettier */
export const AppMode = {
  UNKNOWN_USER: 0,
  OPERATOR: 1,
  USER: 2,
  SAFE: 3
}

export const Permission = {
  BillAcceptor: 'BillAcceptor',
  BillDispenser: 'BillDispenser',
  CoinDispenser: 'CoinDispenser',
  CardReader: 'CardReader',
  MoneyService: 'MoneyService',
  Reports: 'Reports',
  Administration: 'Administration',
  Configuration: 'Configuration',
  Shutdown: 'Shutdown',
  Maintenance: 'Maintenance',
  Logs: 'Logs'
}

export const CurrencyPosition = {
  BEFORE: 0,
  AFTER: 1
}

export const TERMINAL_TYPES = {
  GAMING_ATM: 'gamingatm',
  PARCEL_LOCKER: 'parcellocker',
  BANKING_ATM: 'bankingatm',
  ENTERTAINMENT: 'entertainment'
}

export const APP_ROUTERS = {
  GAMING_ATM: 'gaming-atm-router',
  PARCEL_LOCKER: 'parcel-locker-router',
  BANKING_ATM: 'banking-atm-router',
  ENTERTAINMENT: 'entertainment-router'
}
