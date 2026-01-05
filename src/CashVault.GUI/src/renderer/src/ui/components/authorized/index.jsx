/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
import { usePermissions } from '@domain/global/stores'
import { includesAll, includesAny } from '@domain/global/services'

const Authorized = ({ children, all = false, permissions = [] }) => {
  const userPermissions = usePermissions()

  if (all && permissions && permissions?.length > 0 && includesAll(userPermissions, permissions))
    return <>{children}</>

  if (!all && permissions && permissions?.length > 0 && includesAny(userPermissions, permissions))
    return <>{children}</>

  return <></>
}

export default Authorized
