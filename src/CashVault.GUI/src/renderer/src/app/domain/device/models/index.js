/* eslint-disable prettier/prettier */

export class Device {
  constructor(
    name,
    status,
    type,
    isEnabled = false,
    isActive = false,
    isConnected = false,
    errorMessage = '',
    warningMessage = '',
    deviceDriver = '',
    commandInProgress = false
  ) {
    this.name = name
    this.status = status
    this.type = type
    this.isEnabled = isEnabled
    this.isActive = isActive
    this.isConnected = isConnected
    this.errorMessage = errorMessage
    this.warningMessage = warningMessage
    this.deviceDriver = deviceDriver,
    this.commandInProgress = commandInProgress
  }
}
