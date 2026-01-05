#include <Arduino.h>
#include "SingleSensorResponse.h"

SingleSensorResponse::SingleSensorResponse(uint8_t cmd, uint8_t sensorID)
{
    constexpr uint8_t payloadSize = 2; // sensorID and sensorStatus
    constexpr uint8_t INVALID_SENSOR_ID = 0xFF;

    this->cmd = cmd;
    this->msgLength = MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE + payloadSize;
    this->payload = new uint8_t[2];
    this->payload[0] = sensorID;

    // handle invalid sensorID
    if(sensorID > DOOR_SENSOR_COUNT)
    {
        this->payload[1] = INVALID_SENSOR_ID;
    }
    else
    {
        for(int i = 0; i < DOOR_SENSOR_COUNT; i++)
        {
            if(sensorID == ccu.doorSensors[i].getId())
            {
                this->payload[1] = ccu.doorSensors[i].getStatus();
                break;
            }
        }
    }

    this->crc = calculateCRC();
    // Write data to serial port
    this->send();
};

SingleSensorResponse::~SingleSensorResponse()
{
    if(payload != nullptr)
    {
        delete[] payload;
        payload = nullptr;
    }
}