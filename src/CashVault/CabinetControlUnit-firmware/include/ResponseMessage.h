#pragma once

#include <Arduino.h>
#include "SerialPortMessage.h"

class ResponseMessage : public SerialPortMessage
{
    public:
        ResponseMessage();
        ~ResponseMessage();
        int16_t send();
};