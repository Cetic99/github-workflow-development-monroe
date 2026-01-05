/* eslint-disable prettier/prettier */
/* eslint-disable dot-notation */
import axios from 'axios'
import { logout } from '@domain/global/stores'
import { serverLogout } from './index'
import { baseConfig } from './config'

// ===========================

const apiDevice = axios.create({
  headers: {
    Accept: 'application/json, text/plain',
    'Content-Type': 'application/json;charset=UTF-8'
  },
  baseURL: baseConfig.baseURL,
  maxContentLength: baseConfig.maxContentLength,
  maxBodyLength: baseConfig.maxBodyLength
})

// Response interceptor
apiDevice.interceptors.response.use(function (error) {
  if (error?.response?.status === 401) {
    serverLogout()
    logout()
    return Promise.reject(error)
  }

  return Promise.reject(error)
})

export default apiDevice
