/**
 * Rounds a number to the specified precision if it has more decimal places than the precision.
 *
 * @param {number|string} number - The number to be rounded.
 * @param {number} [precision=2] - The number of decimal places to round to.
 * @returns {number|string} - The rounded number or the original input if it doesn't need rounding or is invalid.
 */
export const roundAmountToPrint = (number, precision = 2) => {
  if (number === null || number === undefined || isNaN(number)) {
    return number
  }

  const parsedNumber = parseFloat(number)
  if (isNaN(parsedNumber)) {
    return number
  }

  const decimalPlaces = (parsedNumber.toString().split('.')[1] || '').length
  if (decimalPlaces > precision) {
    return parsedNumber.toFixed(precision)
  }

  return parsedNumber
}

export const computeDifferenceToFixed = (total, differenceAmount, precision = 2) => {
  const toNumber = (value) => {
    if (value == null) return 0
    if (typeof value === 'number' && !isNaN(value)) return value
    const parsed = parseFloat(value)
    return isNaN(parsed) ? 0 : parsed
  }

  let prec = Math.trunc(Number(precision))
  if (isNaN(prec) || prec < 0) {
    prec = 0
  }

  const t = toNumber(total)
  const d = toNumber(differenceAmount)
  const diff = t - d

  return diff.toFixed(prec)
}
