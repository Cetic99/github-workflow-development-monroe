#pragma once

#include <Arduino.h>
#include "ResponseMessage.h"
#include "ControlCabinetUnit.h"

class CcuStatusResponse : public ResponseMessage
{
    public:
        CcuStatusResponse(uint8_t cmd);
        ~CcuStatusResponse();
};
