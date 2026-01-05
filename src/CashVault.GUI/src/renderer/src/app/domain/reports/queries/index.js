/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//===========================================================================

const getDailyMediaData = async (props) => {
  const { data } = await api.get(`api/reports/daily-media`, {
    params: props
  })

  return data
}

export const useDailyMediaReport = (props) => {
  return useQuery({
    queryKey: ['reports-daily-media', props.date],
    queryFn: async () => getDailyMediaData(props),
    enabled: props.enabled
  })
}

//===========================================================================

const getEndOfShiftData = async () => {
  const { data } = await api.get(`api/reports/end-of-shift`)

  return data
}

export const useEndOfShiftReport = ({ enabled = true }) => {
  return useQuery({
    queryKey: ['reports-end-of-shift'],
    queryFn: async () => getEndOfShiftData(),
    enabled
  })
}

//===========================================================================

const getMoneyServiceData = async () => {
  const { data } = await api.get(`api/reports/money-service`)

  return data
}

export const useMoneyService = () => {
  return useQuery({
    queryKey: ['money-service'],
    queryFn: async () => getMoneyServiceData()
  })
}
