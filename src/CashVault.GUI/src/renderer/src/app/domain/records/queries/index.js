/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'
import { RecordType } from '@domain/records/constants/index.js'

// ==============================================================================

const getEventLogsData = async (params) => {
  const { data } = await api.get(`api/logs/event`, { params })

  return data
}

const getFailLogsData = async (params) => {
  const { data } = await api.get(`api/logs/fail`, { params })

  return data
}

export const useLogs = (p) => {
  let queryFunc = () => {}

  if (p.type === RecordType.EVENT) queryFunc = getEventLogsData
  if (p.type === RecordType.FAIL) queryFunc = getFailLogsData

  return useQuery({
    queryKey: ['logs', ...Object.values(p)],
    queryFn: async () => queryFunc(p),
    staleTime: 2000,
    refetchInterval: 2000,
    cacheTime: 2000
  })
}

export const useEventLogs = (p) => {
  return useQuery({
    queryKey: ['event-logs', ...Object.values(p)],
    queryFn: async () => getEventLogsData(p),
    enabled: p.enabled,
    staleTime: 2000,
    refetchInterval: 2000,
    cacheTime: 2000
  })
}

export const useFailLogs = (p) => {
  return useQuery({
    queryKey: ['fail-logs', ...Object.values(p)],
    queryFn: async () => getFailLogsData(p),
    enabled: p.enabled,
    staleTime: 2000,
    refetchInterval: 2000,
    cacheTime: 2000
  })
}

//=========================================================================================================

export const useLogDetails = (p) => {
  let queryFunc = () => {}

  return useQuery({
    queryKey: ['log-details', p.id],
    queryFn: async () => queryFunc(p),
    enabled: p.enabled
  })
}

//=========================================================================================================
//===========> Tickets
//=========================================================================================================

const getTicketsLogData = async (params) => {
  const { data } = await api.get(`api/logs/tickets`, { params })

  return data
}

export const useTicketLogs = (p) => {
  return useQuery({
    queryKey: ['ticket-logs', ...Object.values(p)],
    queryFn: async () => getTicketsLogData(p)
  })
}

export const getTicketLogDetailsData = async (params) => {
  const { data } = await api.get(`api/logs/ticket-details/${params.id}`)

  return data
}

export const useTicketLogDetails = (p) => {
  return useQuery({
    queryKey: ['log-details', p.id],
    queryFn: async () => getTicketLogDetailsData(p),
    enabled: p.enabled
  })
}

//=========================================================================================================
//===========> Transactions
//=========================================================================================================

const getTransactionsLogData = async (params) => {
  const { data } = await api.get(`api/logs/transactions`, { params })

  return data
}

export const useTransactionLogs = (p) => {
  return useQuery({
    queryKey: ['transaction-logs', ...Object.values(p)],
    queryFn: async () => getTransactionsLogData(p),
    enabled: p.enabled,
    staleTime: 10000,
    refetchInterval: 10000,
    cacheTime: 10000
  })
}

export const getTransactionLogDetailsData = async (params) => {
  if (params?.isMoneyStatusTransaction === false) {
    const { data } = await api.get(`api/logs/transaction-details/${params.id}`)

    return data
  }

  const { data } = await api.get(`api/logs/money-status-transaction/${params.id}/details`)

  return data
}

export const useTransactionLogDetails = (p) => {
  return useQuery({
    queryKey: ['transaction-log-details', p.id],
    queryFn: async () => getTransactionLogDetailsData(p),
    enabled: p.enabled
  })
}

//=========================================================================================================
//===========> Money Status Transactions
//=========================================================================================================

const getMoneyStatusTransactionsLogData = async (params) => {
  const { data } = await api.get(`api/logs/money-status-transactions`, { params })

  return data
}

export const useMoneyStatusTransactionLogs = (p) => {
  return useQuery({
    queryKey: ['moeny-status-transaction-logs', ...Object.values(p)],
    queryFn: async () => getMoneyStatusTransactionsLogData(p),
    enabled: p.enabled,
    staleTime: 10000,
    refetchInterval: 10000,
    cacheTime: 10000
  })
}

export const getMoneyStatusTransactionLogDetailsData = async (params) => {
  const { data } = await api.get(`api/logs/money-status-transaction/${params.id}/details`)

  return data
}

export const useMoneyStatusTransactionLogDetails = (p) => {
  return useQuery({
    queryKey: ['moeny-status-transaction-log-details', p.id],
    queryFn: async () => getMoneyStatusTransactionLogDetailsData(p),
    enabled: p.enabled
  })
}
