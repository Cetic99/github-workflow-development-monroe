/* eslint-disable prettier/prettier */
import { QueryClient as QC } from '@tanstack/react-query'

export const QueryClient = new QC({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      refetchOnmount: false,
      refetchOnReconnect: false,
      retry: false,
      staleTime: 5 * 1000,
      cacheTime: Infinity // do not delete stale data
    },
    mutations: {
      retry: false
    }
  }
})
