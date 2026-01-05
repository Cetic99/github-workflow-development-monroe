/* eslint-disable prettier/prettier */

export const pageSizes = [5, 10, 15, 20, 30, 40, 50, 100]

export const getClosestPageSize = (rowCount) => {
  return pageSizes
    .filter((size) => size <= rowCount) // Keep only sizes <= rowCount
    .reduce(
      (prev, curr) => (Math.abs(curr - rowCount) < Math.abs(prev - rowCount) ? curr : prev),
      -Infinity // Default to -Infinity so the filter doesn't fail
    )
}

export const sanitizeTimestamp = (timestamp) => {
  if (!timestamp) return null
  if (typeof timestamp === 'string') {
    return timestamp?.split('T')?.join(' ')?.slice(0, 19)
  }
  return timestamp
}

export const getPreciseTimestamp = (date = new Date()) => {
  const pad = (num, size = 2) => String(num).padStart(size, '0')

  const year = date.getFullYear()
  const month = pad(date.getMonth() + 1)
  const day = pad(date.getDate())

  const hours = pad(date.getHours())
  const minutes = pad(date.getMinutes())
  const seconds = pad(date.getSeconds())
  const millis = pad(date.getMilliseconds(), 3)

  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}.${millis}`
}
