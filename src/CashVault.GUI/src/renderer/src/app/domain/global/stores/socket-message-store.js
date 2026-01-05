/* eslint-disable prettier/prettier */
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export const socketMessageStore = create(
  immer((set) => ({
    messages: [],
    // =======================================
    actions: {
      add: (message) => {
        set((state) => {
          state.messages.push(message)
        })
      },
      remove: (uuid) => {
        set((state) => {
          state.messages = state.messages.filter((x) => x.uuid !== uuid)
        })
      },
      removeByType: (type) => {
        set((state) => {
          state.messages = state.messages.filter((x) => x.type !== type)
        })
      }
    }
  }))
)

export const addMessage = (message) => socketMessageStore.getState().actions.add(message)
export const removeMessage = (uuid) => socketMessageStore.getState().actions.remove(uuid)
export const removeMessageByType = (type) => socketMessageStore.getState().actions.remove(type)
export const getMessages = () => socketMessageStore.getState().messages
export const getMessage = (uuid) =>
  socketMessageStore.getState().messages?.find((x) => x.uuid === uuid)
