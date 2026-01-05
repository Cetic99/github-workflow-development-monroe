/* eslint-disable prettier/prettier */
/* eslint-disable dot-notation */
import axios from 'axios'
import { getAccessToken, getRefreshToken, logout, login } from '@domain/global/stores'
import { isJwtTokenExpired } from '@domain/global/services'
import { getSettings } from '../process'
import { getCurrentLanguage } from '@domain/administration/stores'
import Notifications from '@ui/utility/notifications'
import apiDevice from '@src/app/infrastructure/api/api-device'
import { baseConfig } from './config'

export const baseURL = baseConfig.baseURL
const defaultRequestTimeout = baseConfig.defaultRequestTimeout

const api = axios.create({
  headers: {
    Accept: 'application/json, text/plain',
    'Content-Type': 'application/json;charset=UTF-8'
  },
  baseURL,
  maxContentLength: baseConfig.maxContentLength,
  maxBodyLength: baseConfig.maxBodyLength
})

export const put = async (url, data = {}, params = {}) => {
  const settings = await getSettings()
  const requestTimeout =
    settings?.requestTimeout != null ? parseInt(settings?.requestTimeout) : defaultRequestTimeout

  const controller = new AbortController()
  const timeout = setTimeout(() => controller.abort(), requestTimeout)

  try {
    const response = await api.put(url, data, {
      ...params,
      signal: controller.signal
    })
    return response.data
  } finally {
    clearTimeout(timeout)
  }
}

export const post = async (url, data = {}, params = {}) => {
  const settings = await getSettings()
  const requestTimeout =
    settings?.requestTimeout != null ? parseInt(settings?.requestTimeout) : defaultRequestTimeout

  const controller = new AbortController()
  const timeout = setTimeout(() => controller.abort(), requestTimeout)

  try {
    const response = await api.post(url, data, {
      ...params,
      signal: controller.signal
    })
    return response
  } finally {
    clearTimeout(timeout)
  }
}

export const remove = async (url, params = {}) => {
  const settings = await getSettings()
  const requestTimeout =
    settings?.requestTimeout != null ? parseInt(settings?.requestTimeout) : defaultRequestTimeout

  const controller = new AbortController()
  const timeout = setTimeout(() => controller.abort(), requestTimeout)

  try {
    const response = await api.delete(url, {
      ...params,
      signal: controller.signal
    })
    return response
  } finally {
    clearTimeout(timeout)
  }
}

export const refresh = async (token) => {
  const refreshApi = axios.create({
    baseURL,
    headers: {
      Accept: 'application/json, text/plain',
      'Content-Type': 'application/json;charset=UTF-8',
      Authorization: `Bearer ${token}`
    },
    maxContentLength: Infinity,
    maxBodyLength: Infinity
  })

  var result = await refreshApi.post(`api/auth/refresh/${token}`)

  return result
}

export const serverLogout = async (token) => {
  const logoutApi = axios.create({
    baseURL,
    headers: {
      Accept: 'application/json, text/plain',
      'Content-Type': 'application/json;charset=UTF-8',
      Authorization: `Bearer ${token}`
    },
    maxContentLength: Infinity,
    maxBodyLength: Infinity
  })

  var result = await logoutApi.post(`api/auth/logout`)

  return result
}

export const attachRequestInterceptor = () => {
  const interceptorFunc = async (config) => {
    var accessToken = getAccessToken()
    var refreshToken = getRefreshToken()

    var accessExpired = isJwtTokenExpired(accessToken, 0)
    var refreshExpired = isJwtTokenExpired(refreshToken, 0)

    if (accessExpired && refreshExpired) {
      serverLogout()
      logout()
    }

    if (accessExpired && !refreshExpired) {
      try {
        var { data } = await refresh(refreshToken)

        if (data?.accessToken && data?.refreshToken) {
          login(data.accessToken, data.refreshToken)
          config.headers.Authorization = `Bearer ${data.accessToken}`
        } else {
          serverLogout()
          logout()
        }
      } catch (e) {
        serverLogout()
        logout()
      }
    }

    if (!accessExpired) {
      config.headers.Authorization = `Bearer ${accessToken}`
    }

    config.headers['Accept-Language'] = getCurrentLanguage()

    return config
  }

  ;(error) => {
    Promise.reject(error)
  }

  api.interceptors.request.use(interceptorFunc, (error) => {
    Promise.reject(error)
  })

  apiDevice.interceptors.request.use(interceptorFunc, (error) => {
    Promise.reject(error)
  })
}

export const attachRequestForLanguageInterceptor = () => {
  const interceptorFunc = async (config) => {
    config.headers['Accept-Language'] = getCurrentLanguage()
    return config
  }
  api.interceptors.request.use(interceptorFunc, (error) => {
    Promise.reject(error)
  })
  apiDevice.interceptors.request.use(interceptorFunc, (error) => {
    Promise.reject(error)
  })
}

// Response interceptor
api.interceptors.response.use(
  function (response) {
    const method = response?.config?.method?.toUpperCase()

    if ((method === 'POST' || method === 'PUT') && response.status === 200) {
      Notifications.success('Success!')
    }

    return response
  },
  function (error) {
    if (error?.response?.status === 500) {
      Notifications.error('Server error!')
      return Promise.reject(error)
    }

    if (error?.response?.status === 401) {
      serverLogout()
      logout()
      return Promise.reject(error)
    }

    if (error?.config?.method?.toUpperCase() === 'GET') {
      Notifications.error('Error loading data!')
      return Promise.reject(error)
    }

    var errorDetails = []

    try {
      errorDetails = JSON.parse(error.response?.data?.detail)
    } catch (error) {
      Notifications.error('Error saving data!')
      return Promise.reject(error)
    }

    if (errorDetails?.length > 0) {
      let errorMessage = ''

      errorDetails.forEach((element) => {
        errorMessage += `${element.PropertyName}: ${element.ErrorMessage}\n`
      })

      Notifications.error(errorMessage)
    } else {
      Notifications.error('General Error')
    }

    return Promise.reject(error)
  }
)

export default api
