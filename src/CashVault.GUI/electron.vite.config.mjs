/* eslint-disable prettier/prettier */
import { resolve } from 'path'
import { defineConfig, externalizeDepsPlugin } from 'electron-vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  main: {
    plugins: [externalizeDepsPlugin()]
  },
  preload: {
    plugins: [externalizeDepsPlugin()]
  },
  renderer: {
    resolve: {
      alias: {
        '@ui': resolve('src/renderer/src/ui'),
        '@screens': resolve('src/renderer/src/ui/screens'),
        '@routers': resolve('src/renderer/src/ui/routers'),
        '@icons': resolve('src/renderer/src/ui/components/icons'),
        '@src': resolve('src/renderer/src'),
        '@components': resolve('src/renderer/src/components'),
        '@pages': resolve('src/renderer/src/pages'),
        '@domain': resolve('src/renderer/src/app/domain')
      }
    },
    plugins: [react()]
  }
})
