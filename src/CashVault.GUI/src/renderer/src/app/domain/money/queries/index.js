/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//===========================================================================

const getBillDenominationsDispenseOption = async () => {
  const { data } = await api.get(`api/Money/dispense_options`)

  return data
}

export const useGetBillDispenseOptions = (onError = () => {}) => {
  const query = useQuery({
    queryKey: ['bill-dispense-options'],
    queryFn: async () => getBillDenominationsDispenseOption(),
    staleTime: 0,
    cacheTime: 0
  })

  if (query.isError) {
    onError()
  }

  return query
}

//===========================================================================
export const useRedeemBetboxTicket = (barcode, transactionKey) => {
  const query = useQuery({
    queryKey: ['betbox-ticket-redeem', barcode, transactionKey],
    queryFn: async () => {
      await new Promise((resolve) => setTimeout(resolve, 2000)) // Debounce for 2 seconds

      const { data } = await api.get(`api/betbox/ticket/${barcode}/id/${transactionKey}/redeem`)
      return data
    },
    staleTime: 0,
    cacheTime: 0,
    enabled: !!barcode
  })

  return query
}
