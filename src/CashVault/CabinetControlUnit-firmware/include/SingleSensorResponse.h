#pragma once

#include <Arduino.h>
#include "ResponseMessage.h"
#include "ControlCabinetUnit.h"

class SingleSensorResponse : public ResponseMessage
{
    public:
        SingleSensorResponse(uint8_t cmd, uint8_t sensorID);
        ~SingleSensorResponse();
};