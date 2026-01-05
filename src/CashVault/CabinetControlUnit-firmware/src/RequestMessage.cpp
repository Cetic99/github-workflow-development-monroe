#include "RequestMessage.h"

RequestMessage::RequestMessage(uint8_t header, uint8_t msgLength, uint8_t cmd, uint8_t* payload, uint16_t crc)
{
    this->header = header;
    this->msgLength = msgLength;
    this->cmd = cmd;
    this->payload = payload;
    this->crc = crc;
}

RequestMessage::~RequestMessage()
{
    if(payload != nullptr)
    {
        delete[] payload;
        payload = nullptr;
    }
}

// Main function to handle the request message/command
void RequestMessage::handleRequest()
{
    if(this->crc != this->calculateCRC())
    {
        // TODO: send ERROR message
        // for now, just InvalidCmdResponse, later we can add more error messages with different error codes
        InvalidCmdResponse response(cmd);
        return;
    }    

    switch (cmd)
    {
        case command::GET_FIRMWARE_VERSION:
        {
            // send firmware version response
            if(payload[0] == 0x00) // payload must be 0x00 according to the protocol
            {
                FirmwareVersionResponse response(cmd);
            }
            else
            {
                // send invalid command response
                InvalidCmdResponse response(cmd);
            }
            break;
        }
        case command::GET_ALL_DOOR_SENSORS:
        {
            if(payload[0] == 0x61) // payload must be 0x61 according to the protocol
            {
                AllSensorsResponse response(cmd, payload[0]);
            }
            else
            {
                // send invalid command response
                InvalidCmdResponse response(cmd);
            }
            break;
        }
        case command::GET_DOOR_ID_SENSOR:
        {
            SingleSensorResponse response(cmd, payload[0]);
            break;
        }
        case command::GET_CCUSTATUS:
        {
            CcuStatusResponse response(cmd);
            break;
        }
        case command::GET_TEMPERATURE:
        {
            TemperatureResponse response;
            break;
        }
        default:
        {
            InvalidCmdResponse response(cmd);
            break;
        }
    }
}