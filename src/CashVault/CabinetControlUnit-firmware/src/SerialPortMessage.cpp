#include <Arduino.h>
#include "SerialPortMessage.h"

uint16_t SerialPortMessage::calculateCRC(uint16_t polynomial, uint16_t initialValue, uint16_t finalXorValue, bool reflectIn, bool reflectOut)
{
    uint16_t crc = initialValue; // Initial value
    uint8_t* data = new uint8_t[msgLength - MSG_CRC_SIZE];
    size_t index = 0;

    // Add header, msgLength, cmd, and payload to the data array
    data[index++] = header;
    data[index++] = msgLength;
    data[index++] = cmd;
    for (uint8_t i = 0; i < msgLength - (MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE); i++) 
    {
        data[index++] = payload[i];
    }

    // Calculate CRC
    for (uint8_t i = 0; i < msgLength - MSG_CRC_SIZE; i++)
    {
        uint8_t currentByte = reflectIn ? reflectByte(data[i]) : data[i];
        crc ^= (uint16_t)(currentByte << 8);

        for (int j = 0; j < 8; j++)
        {
            if ((crc & 0x8000) != 0)
            {
                crc = (crc << 1) ^ polynomial;
            }
            else
            {
                crc <<= 1;
            }
        }
    }

    crc ^= finalXorValue;

    delete[] data; // Clean up dynamically allocated memory

    crc = reflectOut ? reflectUShort(crc) : crc;

    // Swap bytes to correct endian (fix)
    return (crc << 8) | (crc >> 8);
}

uint8_t SerialPortMessage::reflectByte(uint8_t b)
{
    uint8_t reflection = 0;
    for (int i = 0; i < 8; i++)
    {
        reflection |= (b >> i & 1) << (7 - i);
    }
    return reflection;
}

uint16_t SerialPortMessage::reflectUShort(uint16_t us)
{
    uint16_t reflection = 0;
    for (int i = 0; i < 16; i++)
    {
        reflection |= (us >> i & 1) << (15 - i);
    }
    return reflection;
}
