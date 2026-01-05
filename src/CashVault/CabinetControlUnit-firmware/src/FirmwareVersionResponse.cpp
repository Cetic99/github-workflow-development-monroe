#include <Arduino.h>
#include "FirmwareVersionResponse.h"

FirmwareVersionResponse::FirmwareVersionResponse(uint8_t cmd)
{
    this->cmd = cmd;
    this->msgLength = MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE + strlen(FIRMWARE_VERSION);
    this->payload = (uint8_t*) (FIRMWARE_VERSION);
    this->crc = calculateCRC();

    // Write data to serial port
    this->send();
};