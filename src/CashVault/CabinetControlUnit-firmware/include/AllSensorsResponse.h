#pragma once

#include <Arduino.h>
#include "ResponseMessage.h"
#include "ControlCabinetUnit.h"

class AllSensorsResponse : public ResponseMessage
{
    public:
        AllSensorsResponse(uint8_t cmd, uint8_t data);
        ~AllSensorsResponse();
};
