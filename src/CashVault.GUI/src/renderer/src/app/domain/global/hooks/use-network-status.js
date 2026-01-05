import { useEffect, useState } from 'react'

const useNetworkStatus = (intervalMs = 3000) => {
  const [isOnline, setIsOnline] = useState(true)

  useEffect(() => {
    const check = async () => {
      try {
        const result = await window.electronAPI.checkOnlineStatus()
        setIsOnline(result)
      } catch (error) {
        setIsOnline(false)
      }
    }

    check()
    const interval = setInterval(check, intervalMs)
    return () => clearInterval(interval)
  }, [intervalMs])

  return { isOnline }
}

export default useNetworkStatus
