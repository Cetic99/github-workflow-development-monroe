/* eslint-disable prettier/prettier */

export const getDeviceByType = (devices, deviceType) => {
    if(!devices || devices.length === 0)
        return null;

    return devices?.find(device => device?.type?.toLowerCase() === deviceType?.toLowerCase())
}