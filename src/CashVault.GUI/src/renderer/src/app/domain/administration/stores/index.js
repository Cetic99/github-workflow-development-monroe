/* eslint-disable prettier/prettier */
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'
import { Language } from '../constants'
import { cloneDeep } from 'lodash'
import { useCallback } from 'react'

export const localizatonStore = create(
  immer((set) => ({
    currentLanguage: Language.BS,
    messages: [],
    // =======================================
    actions: {
      load: (messages) => {
        set((state) => {
          state.messages = messages
        })
      },
      update: (langCode, key, value) => {
        set((state) => {
          var messagesClone = cloneDeep(state.messages)
          let message = messagesClone.find((x) => x.languageCode === langCode && x.key === key)

          if (message) {
            message.value = value
            state.messages = messagesClone
          }
        })
      },
      setCurrentLanguage: (lngCode) => {
        set((state) => {
          state.currentLanguage = lngCode
        })
      }
    }
  }))
)

export const useMessages = () => localizatonStore((state) => state.messages)

export const useCurrentLanguage = () => localizatonStore((state) => state.currentLanguage)

export const getCurrentLanguage = () => localizatonStore.getState().currentLanguage

export const useLocalizationActions = () => localizatonStore((state) => state.actions)

export const translate = (key) => {
  var currentLanguage = localizatonStore.getState().currentLanguage
  var message = localizatonStore
    .getState()
    .messages.find((x) => x.languageCode === currentLanguage && x.key === key)

  if (message) return message.value

  return key
}

export const useTranslation = () => {
  const currentLanguage = useCurrentLanguage()
  const messages = useMessages()

  const t = useCallback(
    (key) => {
      var message = messages.find((x) => x.languageCode === currentLanguage && x.key === key)
      if (message) return message.value
      return key
    },
    [currentLanguage, messages]
  )

  return {
    t
  }
}

export const loadMessages = (messages, defaultLang) => {
  localizatonStore.getState().actions.load(messages)
  localizatonStore.getState().actions.setCurrentLanguage(defaultLang || Language.BS)
}

export const updateMessage = (message) =>
  localizatonStore.getState().actions.update(message.languageCode, message.key, message.value)
