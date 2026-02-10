/* eslint-disable prettier/prettier */
import { useEffect, useState } from 'react'

const useNetworkStatus = (intervalMs = 3000) => {
  const [isOnline, setIsOnline] = useState(true)

  useEffect(() => {
    let isChecking = false

    const check = async () => {
      if (isChecking) return

      isChecking = true
      try {
        const result = await window.electronAPI.checkOnlineStatus()
        setIsOnline(result)
      } catch (error) {
        setIsOnline(false)
      } finally {
        isChecking = false
      }
    }

    check()
    const interval = setInterval(check, intervalMs)

    return () => {
      clearInterval(interval)
    }
  }, [intervalMs])

  return { isOnline }
}

export default useNetworkStatus
