import { ToastContainer, toast } from 'react-toastify'
import IconCheckmarkCircle from '@ui/components/icons/IconCheckmarkCircle'
import IconInfoCircle from '@ui/components/icons/IconInfoCircle'
import IconExclamationMarkCircle from '@ui/components/icons/IconExclamationMarkCircle'

export const NotificationsContainer = () => {
  return (
    <ToastContainer
      position="top-center"
      autoClose={3000} //autoclose after 3 seconds
      hideProgressBar={true}
      newestOnTop={false}
      closeOnClick={true}
      rtl={false}
      pauseOnFocusLoss
      draggable={false}
      pauseOnHover={true}
      theme="colored"
    />
  )
}
const Notifications = {
  success: (message, options = {}) =>
    toast.success(message, {
      icon: <IconCheckmarkCircle size="l" color="var(--toastify-text-color-success)" />,
      ...options
    }),
  error: (message, options = {}) =>
    toast.error(message, {
      icon: <IconExclamationMarkCircle size="l" color="var(--toastify-text-color-error)" />,
      ...options
    }),
  warning: (message, options = {}) =>
    toast.warning(message, {
      icon: <IconExclamationMarkCircle size="l" color="var(--toastify-text-color-warning)" />,
      ...options
    }),
  info: (message, options = {}) =>
    toast.info(message, {
      icon: <IconInfoCircle size="l" color="var(--toastify-text-color-info)" />,
      ...options
    })
}

export default Notifications
