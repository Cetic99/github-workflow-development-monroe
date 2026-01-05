/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
import { includesAll } from '@domain/global/services'
import { useSupportedDevices } from '@src/app/domain/global/stores'

const IsDeviceSupported = ({ children, devices = [] }) => {
  const supportedDevices = useSupportedDevices()

  if (devices && devices?.length > 0 && includesAll(supportedDevices, devices))
    return <>{children}</>

  return <></>
}

export default IsDeviceSupported
