import { useState, useEffect, useRef } from 'react'
import { useResetDevice } from '@domain/administration/commands'
import { useResetAllDevices } from '@domain/administration/commands'
import { useDisableDevice, useEnableDevice } from '@domain/administration/commands'
import { useAllDevices } from '@domain/device/stores'

const useDevices = () => {
  const devices = useAllDevices()

  const { mutate: resetDevice, isPending: resetingDevice } = useResetDevice()
  const { mutate: resetAllDevices, isPending: resetingDevices } = useResetAllDevices()
  const { mutate: enableDevice, isPending: enablingDevice } = useEnableDevice()
  const { mutate: disableDevice, isPending: disablingDevice } = useDisableDevice()

  const [isLoading, setIsLoading] = useState(false)
  const timeoutRef = useRef(null)

  useEffect(() => {
    const pending = resetingDevice || resetingDevices || enablingDevice || disablingDevice

    if (pending) {
      setIsLoading(true)
      if (timeoutRef.current) clearTimeout(timeoutRef.current)
      timeoutRef.current = setTimeout(() => {
        setIsLoading(false)
      }, 2000) // Reset loading state after 2 seconds
    }
  }, [resetingDevice, resetingDevices, enablingDevice, disablingDevice])

  const handleReset = (deviceType) => {
    resetDevice({ deviceType })
  }

  const handleResetAll = () => {
    resetAllDevices()
  }

  const handleEnableDisable = (isEnabled = true, deviceType) => {
    if (isEnabled === true) {
      disableDevice({ deviceType })
    } else {
      enableDevice({ deviceType })
    }
  }

  return {
    data: devices || [],
    isLoading,
    onReset: handleReset,
    onResetAll: handleResetAll,
    onEnableDisable: handleEnableDisable
  }
}

export default useDevices
