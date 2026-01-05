/* eslint-disable prettier/prettier */
import { useEffect, useState } from 'react'
import { getSettings, saveSettings } from '@src/app/infrastructure/process'

export const useSettings = () => {
  const [settings, setSettings] = useState({})

  useEffect(() => {
    const temp = async () => {
      var result = await getSettings()
      setSettings(result)
    }

    temp()
  }, [])

  const updateSettings = async (newSettings) => {
    const updatedSettings = { ...settings, ...newSettings }
    setSettings(updatedSettings)

    await saveSettings(updatedSettings)
  }

  return [settings, updateSettings]
}
