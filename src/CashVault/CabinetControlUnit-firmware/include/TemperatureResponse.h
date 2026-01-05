/**
 * @file TemperatureResponse.h
 * @brief Defines the TemperatureResponse class, which represents a response message
 *        containing temperature data from the control cabinet unit.
 */
#pragma once

#include <Arduino.h>
#include "ResponseMessage.h"
#include "ControlCabinetUnit.h"

class TemperatureResponse : public ResponseMessage
{
    public:
        TemperatureResponse();
        // TemperatureResponse(uint8_t sensorID); // for future use with multiple sensors
        ~TemperatureResponse();
};
