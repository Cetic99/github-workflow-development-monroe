/* eslint-disable prettier/prettier */
import { jwtDecode } from 'jwt-decode'
import { isBoolean, isDate, isEmpty, isNull, isNumber } from 'lodash'

export const isJwtTokenExpired = (token, offset = 0) => {
  try {
    if (token === null || token === undefined) return true

    const decodedToken = jwtDecode(token)
    var currentTime = Date.now() / 1000

    if (decodedToken.exp + offset < currentTime) return true
  } catch (error) {
    console.error('Failed to decode token:', error)
    return true
  }

  return false
}

export const getFormData = (formId) => {
  var data = {}
  var formData = new FormData(document.getElementById(formId))

  formData.forEach((value, key) => (data[key] = value))

  return data
}

export const safeDeserialize = (jsonString) => {
  try {
    const obj = JSON.parse(jsonString)

    if (typeof obj === 'object' && obj !== null) {
      return obj
    } else {
      throw new Error('Deserialized value is not a valid object')
    }
  } catch (error) {
    console.error('Failed to deserialize JSON:', error.message)
    return null
  }
}

export const isNullOrEmptyOrSpaces = (str) => {
  return str === null || str === undefined || str.trim() === ''
}

export const includesAll = (arr, values) => values.every((v) => arr.includes(v))

export const includesAny = (arr, values) => values.some((v) => arr.includes(v))

export const hasAnyValue = (obj) => {
  return Object.values(obj).some((value) => {
    if (isDate(value)) {
      return !isNull(value)
    }

    if (isNumber(value)) {
      return value >= 0
    }

    if (isBoolean(value)) {
      return value === true
    }

    return !isEmpty(value)
  })
}
