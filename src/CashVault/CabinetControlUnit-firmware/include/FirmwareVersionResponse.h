#pragma once

#include <Arduino.h>
#include "ResponseMessage.h"
#include "ControlCabinetUnit.h"

class FirmwareVersionResponse : public ResponseMessage
{
    public:
        FirmwareVersionResponse(uint8_t cmd);
        ~FirmwareVersionResponse() = default;
};