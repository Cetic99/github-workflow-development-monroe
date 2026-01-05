/* eslint-disable prettier/prettier */
import { isEmpty } from 'lodash'
import { CurrencySymbolPositionEnum } from '../constants'

export const displayAmountWithCurrency = (amount, currency) => {
  if (amount == 0 || isEmpty(currency?.symbol)) {
    return ''
  }

  const currencyPosition = currency?.symbolPosition || CurrencySymbolPositionEnum.Default

  if (CurrencySymbolPositionEnum.BeforeValue === currencyPosition)
    return `${currency.symbol} ${amount}`
  if (CurrencySymbolPositionEnum.AfterValue === currencyPosition)
    return `${amount} ${currency.symbol}`

  return ''
}

export const parseDateTime = (value) => {
  const date = new Date(value)

  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()

  return `${day}.${month}.${year}.`
}

export const generateBarcode = ({
  length = 12,
  charset = 'numeric',
  prefix = '',
  allowLeadingZero = true
} = {}) => {
  const numericChars = '0123456789'
  const alphaNumChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'

  const chars = charset === 'alphanumeric' ? alphaNumChars : numericChars

  if (!allowLeadingZero && charset === 'numeric' && length > 0) {
    const first = chars.slice(1) // '1'..'9'
    let res = first.charAt(Math.floor(Math.random() * first.length))

    for (let i = 1; i < length; i++) {
      res += chars.charAt(Math.floor(Math.random() * chars.length))
    }

    return prefix + res
  }

  let result = ''
  
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length))
  }

  return prefix + result
}
