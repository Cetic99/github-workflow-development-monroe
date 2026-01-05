/* eslint-disable prettier/prettier */
import { app, shell, BrowserWindow, ipcMain, screen } from 'electron'
import { dirname, join } from 'path'
import { electronApp, optimizer, is } from '@electron-toolkit/utils'
import icon from '../../resources/icon.png?asset'
import marketingHtml from './marketing.html?asset'
import fs from 'fs'
import { exec } from 'child_process'
const os = require('os')
const https = require('https')

export const OS_PLATFORM = {
  WINDOWS: 'windows',
  LINUX: 'linux',
  UNKNOWN: 'unknown'
}
const getPlatform = () => {
  const platform = os.platform()
  if (platform === 'win32') {
    return OS_PLATFORM.WINDOWS
  } else if (platform === 'linux') {
    return OS_PLATFORM.LINUX
  } else {
    return OS_PLATFORM.UNKNOWN
  }
}

let mainWindow
let marketingWindow

const isDev = !app.isPackaged

//==========================================================================================
//======> Window create functions
//==========================================================================================

// function createSplashWindow() {
//   const splash = new BrowserWindow({
//     width: 400,
//     height: 350,
//     alwaysOnTop: true,
//     show: false,
//     autoHideMenuBar: true
//   })

//   splash.loadFile('./src/main/splash.html')

//   return splash
// }

function createWindow() {
  // const splash = createSplashWindow()
  const primaryDisplay = screen.getPrimaryDisplay()
  // Create the browser window.
  mainWindow = new BrowserWindow({
    show: false,
    autoHideMenuBar: true,
    fullscreen: true,
    x: primaryDisplay.bounds.x,
    y: primaryDisplay.bounds.y,
    // fullscreen: false,
    kiosk: true,
    ...(process.platform === 'linux' ? { icon } : {}),
    webPreferences: {
      preload: join(__dirname, '../preload/index.js'),
      sandbox: false,
      contextIsolation: true
    }
  })

  // splash.on('ready-to-show', () => {
  //   splash.show()
  // })

  mainWindow.webContents.setWindowOpenHandler((details) => {
    shell.openExternal(details.url)
    return { action: 'deny' }
  })

  // HMR for renderer base on electron-vite cli.
  // Load the remote URL for development or the local html file for production.
  if (is.dev && process.env['ELECTRON_RENDERER_URL']) {
    mainWindow.loadURL(process.env['ELECTRON_RENDERER_URL'])
  } else {
    mainWindow.loadFile(join(__dirname, '../renderer/index.html'))
  }

  mainWindow.webContents.on('dom-ready', () => {
    // splash.close() // Close splash screen

    mainWindow.show() // Show the main app window
  })

  mainWindow.on('closed', () => {
    // Close marketing window if it exists
    if (marketingWindow) {
      marketingWindow.close()
      marketingWindow = null
    }
    // Ensure app quits when main window is closed
    app.quit()
  })
}

function createMarketingWindow() {
  const displays = screen.getAllDisplays()
  // Try to use the second display if available, otherwise use primary
  const targetDisplay = displays.length > 1 ? displays[1] : displays[0]
  marketingWindow = new BrowserWindow({
    show: false,
    autoHideMenuBar: true,
    fullscreen: true,
    x: targetDisplay.bounds.x,
    y: targetDisplay.bounds.y,
    ...(process.platform === 'linux' ? { icon } : {}),
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
      enableRemoteModule: true
    }
  })

  // Load marketing.html using asset import
  marketingWindow.loadFile(marketingHtml)
  marketingWindow.webContents.on('dom-ready', () => {
    marketingWindow.show()
  })

  marketingWindow.on('closed', () => {
    marketingWindow = null
  })
}

//==========================================================================================
//======> IPC Hadlers
//==========================================================================================

const runPowerShellScript = (scriptPath) => {
  const platform = getPlatform()
  let interpreter = ''

  if (platform === OS_PLATFORM.WINDOWS) {
    interpreter = 'powershell'
  } else if (platform === OS_PLATFORM.LINUX) {
    interpreter = 'pwsh'
  } else {
    throw new Error('Unsupported platform')
  }

  const command = `${interpreter} -ExecutionPolicy Bypass -File "${scriptPath}"`

  return new Promise((resolve, reject) => {
    exec(`${command}`, (error, stdout, stderr) => {
      if (error) {
        console.error(`Error executing script [${scriptPath}]: ${error.message}`)
        reject(stderr || error)
      } else {
        console.log(`Script output [${scriptPath}]: ${stdout}`)
        resolve(stdout)
      }
    })
  })
}

ipcMain.handle('open-devtools', () => {
  if (mainWindow) {
    mainWindow?.webContents?.openDevTools()
  }
})

ipcMain.handle('bash:runScript', async (_, scriptPath) => {
  try {
    const result = await runPowerShellScript(scriptPath)
    return { success: true, output: result }
  } catch (error) {
    return { success: false, error: error }
  }
})

// Handle fetching the list of files from the folder
ipcMain.handle('fs:getFiles', async (_, folderPath) => {
  try {
    const files = fs.readdirSync(folderPath)
    return files
  } catch (error) {
    return { error: error.message }
  }
})

ipcMain.handle('check-online-status', async () => {
  return new Promise((resolve) => {
    const req = https.request(
      {
        method: 'HEAD',
        host: 'www.google.com',
        timeout: 3000
      },
      (res) => resolve(true)
    )

    req.on('error', () => resolve(false))
    req.end()
  })
})

// Handle reading a specific file's content
ipcMain.handle('fs:readFile', async (_, filePath) => {
  try {
    const content = fs.readFileSync(filePath, 'utf-8')
    return content
  } catch (error) {
    return { error: error.message }
  }
})

const getSettingsPath = () => {
  const exeDir = dirname(app.getPath('exe'))
  return isDev
    ? join(app.getAppPath(), 'resources', 'settings.json') // In development mode
    : join(exeDir, 'resources', 'app.asar.unpacked', 'resources', 'settings.json') // In production mode
}

const getSettings = () => {
  let settings

  const settingsPath = getSettingsPath()

  try {
    const data = fs.readFileSync(settingsPath)
    settings = JSON.parse(data)
    return settings
    //
  } catch (error) {
    console.error('Failed to load settings.json:', error)
    return {
      logsPath: '/Users/dragovicn/projects/cash-vault/src/CashVault/CashVault.WebAPI/logs',
      scriptsPath: '/Users/dragovicn/projects/cash-vault/scripts',
      masterPassword: 'admin',
      marketingWindowEnabled: false,
      marketingVideoPath: ''
    }
  }
}

ipcMain.handle('settings:get', async () => {
  try {
    return getSettings()
  } catch (error) {
    return { error: error.message }
  }
})

ipcMain.handle('settings:save', async (event, newSettings) => {
  const settingsPath = getSettingsPath()

  try {
    fs.writeFileSync(settingsPath, JSON.stringify(newSettings, null, 2))

    // Update marketing window when settings change
    toggleMarketingWindow()

    return true
  } catch (error) {
    console.error('Failed to save settings file:', error)
    return false
  }
})

ipcMain.handle('restart', async () => {
  app.relaunch()
  app.quit()
})

ipcMain.handle('exit', async () => {
  app.quit()
})

ipcMain.handle('marketing:getVideoPath', async () => {
  try {
    const settings = getSettings()
    return settings.marketingVideoPath || ''
  } catch (error) {
    console.error('Error getting marketing video path:', error)
    return ''
  }
})

function shouldShowMarketingWindow() {
  try {
    const settings = getSettings()
    return settings.marketingWindowEnabled === true
  } catch (error) {
    console.error('Error checking marketing window settings:', error)
    return false
  }
}

function initializeMarketingWindow() {
  if (shouldShowMarketingWindow()) {
    createMarketingWindow()
  }
}

function toggleMarketingWindow() {
  if (shouldShowMarketingWindow()) {
    if (!marketingWindow) {
      createMarketingWindow()
    }
  } else {
    if (marketingWindow) {
      marketingWindow.close()
      marketingWindow = null
    }
  }
}

//==========================================================================================
//==========================================================================================

// ====================> Check if another instance of the app is already running
// ====================> and quit if it is. This is to prevent multiple instances of the app from running.
const gotTheLock = app.requestSingleInstanceLock()
if (!gotTheLock) {
  console.log('Another instance is already running. Exiting...')
  app.quit()
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(() => {
  // Set app user model id for windows
  electronApp.setAppUserModelId('com.electron')

  // Default open or close DevTools by F12 in development
  // and ignore CommandOrControl + R in production.
  // see https://github.com/alex8088/electron-toolkit/tree/master/packages/utils
  app.on('browser-window-created', (_, window) => {
    optimizer.watchWindowShortcuts(window)
  })

  createWindow()
  initializeMarketingWindow()

  app.on('activate', function () {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow()
      initializeMarketingWindow()
    }
  })
})

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

// In this file you can include the rest of your app"s specific main process
// code. You can also put them in separate files and require them here.
