/* eslint-disable prettier/prettier */
import { getLogsPath, getScriptsPath } from '@src/config'

export const openDevtools = () => {
  window.general.openDevtools()
}

export const restartApplication = async () => {
  window.general.restart()
}

export const getScriptFiles = async () => {
  const path = await getScriptsPath()
  const scripts = await window.scripts.get(path)
  if (!scripts.error) {
    return scripts
  } else {
    console.error('Error fetching scripts:', scripts.error)
    return []
  }
}

export const runScript = async (script) => {
  const path = await getScriptsPath()
  const scriptPath = `${path}\\${script}`
  const result = await window.scripts.run(scriptPath)

  if (result.success) {
    return result.output
  } else {
    return result
  }
}

export const getLogFiles = async () => {
  const path = await getLogsPath()
  const files = await window.logs.get(path)
  if (!files.error) {
    return files
  } else {
    console.error('Error fetching files:', files.error)
    return []
  }
}

export const readLogFile = async (file) => {
  const path = await getLogsPath()
  const filePath = `${path}/${file}`
  const content = await window.logs.read(filePath)

  if (!content.error) {
    return content
  } else {
    console.error('Error reading file:', content.error)
    return "Can't read file content..."
  }
}

export const getSettings = async () => {
  try {
    const result = await await window.settings.get()

    return result
  } catch (e) {
    return {}
  }
}

export const saveSettings = async (updatedSettings) => {
  try {
    await window.settings.save(updatedSettings)
  } catch (e) {
    console.error(e)
  }
}
