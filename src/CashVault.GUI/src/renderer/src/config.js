/* eslint-disable prettier/prettier */

export const getLogsPath = async () => {
  const settings = await window.settings.get()
  return settings.logsPath
}

export const getScriptsPath = async () => {
  const settings = await window.settings.get()
  return settings.scriptsPath
}

export const getMasterPassword = async () => {
  const settings = await window.settings.get()
  return settings.masterPassword
}
