/* eslint-disable prettier/prettier */
import { enqueueSnackbar } from 'notistack'

const Notifications = {
  success: (message, options = {}) => {
    enqueueSnackbar(message, {
      variant: 'success',
      autoHideDuration: 1500,
      ...options
    })
  },
  error: (message) => {
    enqueueSnackbar(message, {
      variant: 'error',
      autoHideDuration: 4000
    })
  },
  warning: (message) => {
    enqueueSnackbar(message, {
      variant: 'warning',
      autoHideDuration: 3000
    })
  },
  info: (message) => {
    enqueueSnackbar(message, {
      variant: 'info',
      autoHideDuration: 3000
    })
  }
}

export default Notifications
