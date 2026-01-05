/* eslint-disable prettier/prettier */
import { contextBridge, ipcRenderer } from 'electron'
import { electronAPI } from '@electron-toolkit/preload'

// Custom APIs for renderer
const api = {}

// Use `contextBridge` APIs to expose Electron APIs to
// renderer only if context isolation is enabled, otherwise
// just add to the DOM global.
if (process.contextIsolated) {
  try {
    contextBridge.exposeInMainWorld('electron', electronAPI)
    contextBridge.exposeInMainWorld('api', api)
  } catch (error) {
    console.error(error)
  }

  //
  contextBridge.exposeInMainWorld('logs', {
    get: (folderPath) => ipcRenderer.invoke('fs:getFiles', folderPath),
    read: (filePath) => ipcRenderer.invoke('fs:readFile', filePath)
  })

  contextBridge.exposeInMainWorld('scripts', {
    get: (folderPath) => ipcRenderer.invoke('fs:getFiles', folderPath),
    run: (scriptPath) => ipcRenderer.invoke('bash:runScript', scriptPath)
  })

  contextBridge.exposeInMainWorld('general', {
    restart: () => ipcRenderer.invoke('restart'),
    exit: () => ipcRenderer.invoke('exit'),
    openDevtools: () => ipcRenderer.invoke('open-devtools')
  })

  contextBridge.exposeInMainWorld('settings', {
    get: () => ipcRenderer.invoke('settings:get'),
    save: (settings) => ipcRenderer.invoke('settings:save', settings)
  })

  contextBridge.exposeInMainWorld('electronAPI', {
    checkOnlineStatus: () => ipcRenderer.invoke('check-online-status')
  })

  //
} else {
  window.electron = electronAPI
  window.api = api
}
