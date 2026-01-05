#include "TemperatureResponse.h"
#include <cmath>

TemperatureResponse::TemperatureResponse()
{
    constexpr uint8_t payloadSize = 4; // 4 bytes for temperature
    float temperature = ccu.temperatureSensor.getTemperature();

    this->cmd = command::GET_TEMPERATURE;
    this->msgLength = MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE + payloadSize; // 4 bytes for temperature
    this->payload = new uint8_t[payloadSize];

    // prepare payload
    this->payload[0] = 0; // Default sensor ID (0)
    this->payload[1] = (temperature < 0) ? 0x01 : 0x00; // Sign (0x00 for positive, 0x01 for negative)
    temperature = std::abs(temperature);
    this->payload[2] = static_cast<uint8_t>(temperature); // Integer part of temperature
    this->payload[3] = static_cast<uint8_t>((temperature - payload[2]) * 100); // Fraction part of temperature (scaled to 2 decimal places)

    this->crc = calculateCRC(); // Calculate CRC for the message
    this->send(); // Send the response message
}

TemperatureResponse::~TemperatureResponse()
{
    if(payload != nullptr)
    {
        delete[] payload;
        payload = nullptr;
    }
}
