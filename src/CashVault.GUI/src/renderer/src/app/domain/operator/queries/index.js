/* eslint-disable no-unused-vars */
/* eslint-disable prettier/prettier */
import { useQuery } from '@tanstack/react-query'
import api from '@src/app/infrastructure/api'

//===========================================================================

const getOperatorsData = async (params) => {
  const { data } = await api.get(`api/operators`, {
    params
  })

  return data
}

export const useOperators = (params) => {
  return useQuery({
    queryKey: ['administartion-operators', ...Object.values(params)],
    queryFn: async () => getOperatorsData(params)
  })
}

//===========================================================================

const getCardsForOperatorData = async (params) => {
  const { data } = await api.get(`api/operator/${params.operatorId}/id-cards`, {
    params
  })

  return data
}

export const useCardsForOperator = (params) => {
  return useQuery({
    queryKey: ['administartion-operator-cards', params.operatorId, params.page, params.pageSize],
    queryFn: async () => getCardsForOperatorData(params),
    enabled: params.enabled
  })
}

//===========================================================================

const getPermissionsData = async () => {
  const { data } = await api.get(`api/permissions`)

  return data
}

export const usePermissions = (params) => {
  return useQuery({
    queryKey: ['administartion-permissions'],
    queryFn: async () => getPermissionsData(),
    enabled: params.enabled
  })
}
