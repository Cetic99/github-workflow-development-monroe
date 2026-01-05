#include "InvalidCmdResponse.h"

InvalidCmdResponse::InvalidCmdResponse(uint8_t cmd)
{
    uint8_t data = cmd; // requested command (invalid command)
    
    this->cmd = command::INVALID_CMD;
    this->msgLength = MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE + 1;
    this->payload = &data;

    this->crc = calculateCRC();
    // Write data to serial port
    this->send();
}

InvalidCmdResponse::~InvalidCmdResponse()
{}
