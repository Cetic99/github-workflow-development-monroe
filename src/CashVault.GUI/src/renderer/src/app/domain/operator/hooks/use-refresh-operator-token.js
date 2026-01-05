/* eslint-disable prettier/prettier */
import { useOperatorId, useRefreshToken, login } from '@domain/global/stores'
import { refresh, attachRequestInterceptor } from '@src/app/infrastructure/api/index'
import { useNavigate } from 'react-router-dom'

const useRefreshOpertorToken = ({ id }) => {
  const currentOperatorId = useOperatorId()
  const token = useRefreshToken()
  const navigate = useNavigate()

  const refreshToken = async () => {
    if (id != currentOperatorId) return

    var { data } = await refresh(token)
    login(data.accessToken, data.refreshToken)
    attachRequestInterceptor()

    navigate('/')
  }

  return { refreshToken }
}

export default useRefreshOpertorToken
