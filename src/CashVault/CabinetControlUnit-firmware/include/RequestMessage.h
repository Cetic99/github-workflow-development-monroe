#pragma once

#include <Arduino.h>
#include "SerialPortMessage.h"
#include "FirmwareVersionResponse.h"
#include "SingleSensorResponse.h"
#include "AllSensorsResponse.h"
#include "InvalidCmdResponse.h"
#include "CcuStatusResponse.h"
#include "TemperatureResponse.h"

class RequestMessage : public SerialPortMessage
{
    public:
        RequestMessage(uint8_t header, uint8_t msgLength, uint8_t cmd, uint8_t* payload, uint16_t crc);
        ~RequestMessage();

        void handleRequest();
};