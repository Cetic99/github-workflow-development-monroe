#include "AllSensorsResponse.h"

AllSensorsResponse::AllSensorsResponse(uint8_t cmd, uint8_t data)
{
    this->cmd = cmd;
    this->msgLength = MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE + DOOR_SENSOR_COUNT;
    this->payload = new uint8_t[DOOR_SENSOR_COUNT];

    for (size_t i = 0; i < DOOR_SENSOR_COUNT; i++)
    {
        this->payload[i] = ccu.doorSensors[i].getStatus();
    }

    this->crc = calculateCRC();
    // Write data to serial port
    this->send();
}

AllSensorsResponse::~AllSensorsResponse()
{
    if(payload != nullptr)
    {
        delete[] payload;
        payload = nullptr;
    }
}
