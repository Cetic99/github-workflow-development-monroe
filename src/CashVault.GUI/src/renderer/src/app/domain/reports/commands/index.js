/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useMutation } from '@tanstack/react-query'
import Notifications from '@src/app/services/notifications'
import { post } from '@src/app/infrastructure/api'

//===========================================================================

const printDailyMediaReport = async (data) => {
  const result = await post(`api/reports/daily-media/print`, null, {
    params: data
  })

  return result
}

export const usePrintDailyMediaReport = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: printDailyMediaReport,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Printed successfully!')
    },
    onError: () => {
      onError()
    }
  })
}

//===========================================================================
const printEndOfShiftReport = async (data) => {
  const result = await post(`api/reports/end-of-shift/print`)

  return result
}

export const usePrintEndOfShiftReport = (onSuccess = () => {}, onError = () => {}) => {
  return useMutation({
    mutationFn: printEndOfShiftReport,
    onSuccess: () => {
      onSuccess()
      Notifications.success('Printed successfully!')
    },
    onError: () => {
      onError()
    }
  })
}
