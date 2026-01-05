/* eslint-disable prettier/prettier */
import { cloneDeep } from 'lodash'
import moment from 'moment'
import { useEffect } from 'react'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'
import { DefaultDecimalConfig } from '../constants'

export const regionalConfigurationStore = create(
  immer((set) => ({
    config: {},

    // =======================================

    actions: {
      load: (data) => {
        set((state) => {
          state.config = cloneDeep(data)
        })
      }
    }
  }))
)

export const useTimeZone = () =>
  regionalConfigurationStore((state) => {
    return state.config.localTimeZone
  })

export const useDecimalsConfig = () =>
  regionalConfigurationStore((state) => {
    return {
      decimalSeparator: state.config.decimalSeparatorSymbol,
      thousandSeparator: state.config.thousandSeparatorSymbol,
      amountPrecision: state.config.amountPrecision
    }
  })

export const useDateTimeFormat = () =>
  regionalConfigurationStore((state) => {
    return state.config.dateFormat
  })

export const getTimeZone = () => {
  const config = regionalConfigurationStore.getState().config

  return config.localTimeZone
}

export const getDecimalsConfig = () => {
  const config = regionalConfigurationStore.getState().config

  return {
    decimalSeparator: config.decimalSeparatorSymbol,
    thousandSeparator: config.thousandSeparatorSymbol,
    amountPrecision: config.amountPrecision
  }
}

export const getDateTimeFormat = () => {
  // only date format is defined - no time yet
  const config = regionalConfigurationStore.getState().config

  return config.dateFormat
}

export const useRegionalConfigurationActions = () =>
  regionalConfigurationStore((state) => state.actions)

export const loadRegionalConfiguration = (data) => {
  regionalConfigurationStore.getState().actions.load(data)
}

export const useLocalTime = (time) => {
  const timeZone = useTimeZone()
  const dateFormat = useDateTimeFormat()

  useEffect(() => {}, [timeZone, dateFormat])

  const getLocalTime = (currentTime) => {
    const locale = navigator.language || navigator.languages[0]

    const formatter = new Intl.DateTimeFormat(locale, {
      timeZone,
      timeZoneName: 'short',
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: 'numeric',
      minute: 'numeric',
      second: 'numeric'
    })

    const parts = formatter.formatToParts(currentTime)

    return Object.fromEntries(parts.map((p) => [p.type, p.value]))
  }

  const { year, month, day, hour, minute, second } = getLocalTime(time)

  let calculatedMonth = month

  if (calculatedMonth > 0) {
    calculatedMonth -= 1
  }

  const localDateTime = new Date(year, calculatedMonth, day, hour, minute, second)
  const convertedMoment = moment(localDateTime)

  return {
    date: convertedMoment.format(dateFormat || 'DD.MM.YYYY.'),
    hours: hour,
    minutes: minute
  }
}

export const useDecmialNumberFormat = (value) => {
  const decimalConfig = useDecimalsConfig()

  useEffect(() => {}, [decimalConfig])

  const parts = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: decimalConfig.amountPrecision || DefaultDecimalConfig.amountPrecision,
    maximumFractionDigits: decimalConfig.amountPrecision || DefaultDecimalConfig.amountPrecision,
    useGrouping: true
  })
    .format(value)
    .split('.')

  let [intPart, fracPart] = parts

  // replace thousand separator (from "," in en-US to your custom one)
  intPart = intPart.replace(
    /,/g,
    decimalConfig.thousandSeparator || DefaultDecimalConfig.thousandSeparator
  )

  // join using custom decimal separator
  return fracPart
    ? `${intPart}${decimalConfig.decimalSeparator || DefaultDecimalConfig.decimalSeparator}${fracPart}`
    : intPart
}

export const useLocationAddress = () =>
  regionalConfigurationStore((state) => {
    return state.config.locationAddress
  })

export const useMachineName = () =>
  regionalConfigurationStore((state) => {
    return state.config.machineName
  })
