#pragma once

#include <Arduino.h>
#include "ResponseMessage.h"

class InvalidCmdResponse : public ResponseMessage
{
    public:
        InvalidCmdResponse(uint8_t cmd);
        ~InvalidCmdResponse();
};
