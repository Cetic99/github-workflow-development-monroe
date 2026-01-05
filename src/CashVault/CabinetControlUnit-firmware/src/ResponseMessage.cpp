#include <Arduino.h>
#include "ResponseMessage.h"

ResponseMessage::ResponseMessage()
{
    header = MSG_RESPONSE_HEADER;
}

ResponseMessage::~ResponseMessage()
{}

int16_t ResponseMessage::send()
{
    size_t bytesSent = 0;
    
    // send header
    bytesSent = Serial.write(this->header);

    // send msg length
    bytesSent += Serial.write(this->msgLength);

    // send cmd
    bytesSent += Serial.write(this->cmd);

    // send payload
    for (uint8_t i = 0; i < this->msgLength - (MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE); i++)
    {
        bytesSent += Serial.write(this->payload[i]);
    }

    // send crc
    bytesSent += Serial.write((uint8_t)((this->crc >> 8) & 0xFF));  // High byte
    bytesSent += Serial.write((uint8_t)(this->crc & 0xFF));         // Low byte

    if(bytesSent == this->msgLength)
    {
        return bytesSent;
    }
    else
    {
        return -1;
    }
}