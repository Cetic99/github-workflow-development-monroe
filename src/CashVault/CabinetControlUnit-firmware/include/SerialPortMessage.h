#pragma once

#include <Arduino.h>
#include <vector>
#include <cstdint>
#include "ControlCabinetUnit.h"

/**
 * @namespace command
 * @brief Contains constants for various command messages used in SerialPort communication.
 *
 * @var GET_FIRMWARE_VERSION
 * Command to get the firmware version.
 *
 * @var GET_ALL_DOOR_SENSORS
 * Command to get the status of all door sensors.
 *
 * @var GET_DOOR_ID_SENSOR
 * Command to get the status of a specific door sensor by ID.
 *
 * @var GET_PERIPHERALS
 * Command TODO: add description.
 *
 * @var INVALID_CMD
 * Command to indicate an invalid command requested by host.
 * If host request unexisting command, the message with this command
 * will be sent back to host.
 * 
 * @var GET_CCUSTATUS
 * Command to get the status of the Control Cabinet Unit.
 * This command will return 0x00 if the CCU status is OK, otherwise
 * it will return error code.
 * 
 * @var GET_TEMPERATURE
 * Command to get the temperature from the temperature sensor.
 */
namespace command
{
    constexpr uint8_t GET_FIRMWARE_VERSION  = 0x66; // ascii value of 'f' (F-irmware)
    constexpr uint8_t GET_ALL_DOOR_SENSORS  = 0x53; // ascii value of 'S' (S-ensors)
    constexpr uint8_t GET_DOOR_ID_SENSOR    = 0x73; // ascii value of 's' (s-ensor id)
    constexpr uint8_t INVALID_CMD           = 0x45; // ascii value of 'E' (E-rror command)
    constexpr uint8_t GET_CCUSTATUS         = 0x63; // ascii value of 'c' (c-cu status)
    constexpr uint8_t GET_TEMPERATURE       = 0x74; // ascii value of 't' (t-emperature)
    constexpr uint8_t CLEAR_ERR_WRN_CODE    = 0x43; // ascii value of 'C' (C-lear)
}

class SerialPortMessage
{
    public:
        SerialPortMessage() = default;
        ~SerialPortMessage() = default;

    protected:
        uint8_t header;
        uint8_t msgLength;
        uint8_t cmd;
        uint8_t* payload;
        uint16_t crc;
        uint16_t calculateCRC(uint16_t  polynomial = CRC_POLYNOMIAL,
                              uint16_t  initialValue = CRC_INITIAL_VALUE,
                              uint16_t  finalXorValue = CRC_FINAL_XOR_VALUE,
                              bool      reflectIn = CRC_REFLECT_IN,
                              bool      reflectOut = CRC_REFLECT_OUT);

    private:
        uint8_t reflectByte(uint8_t b);
        uint16_t reflectUShort(uint16_t us);
};
