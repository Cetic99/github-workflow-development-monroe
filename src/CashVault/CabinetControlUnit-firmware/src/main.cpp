#include <Arduino.h>
#include "SerialPortMessage.h"
#include "RequestMessage.h"
#include "ControlCabinetUnit.h"

#define READ_DELAY_MS (2) // Delay for reading serial data

ControlCabinetUnit ccu;

unsigned long lastTemperatureMeasureTime = 0; // Track last measurement time

void setup()
{
    // put your setup code here, to run once:
    Serial.begin(BAUD_RATE);
    ccu.init();

    pinMode(LED_BUILTIN, OUTPUT); // Set LED pin as output
    digitalWrite(LED_BUILTIN, LOW); // Turn off the LED
}

void loop()
{
    unsigned long currentMillis = millis();
    unsigned long lastLedONTime = 0; // Track last LED ON time (for vibration indication, debugging, and sensitivity adjustment)
    #define VIBRATION_LED_PERIOD_TO_BE_ON_MS (2000) // LED on period, for debugging only

    if(Serial.available())
    {
        delay(10);
        uint8_t header = Serial.read();
        if (header == MSG_REQUEST_HEADER) // request msg header received
        {
            uint8_t fullMsgLen = Serial.read();
            delay(READ_DELAY_MS);

            uint8_t cmd = Serial.read();
            delay(READ_DELAY_MS);

            uint8_t payloadSize = fullMsgLen - (MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE);

            uint8_t *payload = new uint8_t[payloadSize];

            for (uint8_t i = 0; i < payloadSize; i++)
            {
                payload[i] = Serial.read();
                delay(READ_DELAY_MS);
            }

            uint8_t crcH = Serial.read();
            delay(READ_DELAY_MS);
            uint8_t crcL = Serial.read();
            delay(READ_DELAY_MS);

            uint16_t crc = (crcH << 8) | crcL;

            RequestMessage request(header, fullMsgLen, cmd, payload, crc);
            request.handleRequest();
        }
    }

    if (currentMillis - lastTemperatureMeasureTime >= TEMP_MEASURE_PERIOD_MS)
    {
        lastTemperatureMeasureTime = currentMillis;
        ccu.temperatureSensor.measure(); // Measure temperature periodically
        if(ccu.temperatureSensor.getTemperature() > TEMP_HIGH_THRESHOLD)
        {
            ccu.errWrnManager.addWarningCode(WarningCodes::HIGH_TEMPERATURE); // Add high temperature wrn code
        }
        else
        {
            ccu.errWrnManager.removeWarningCode(WarningCodes::HIGH_TEMPERATURE); // Clear high temperature wrn code if temperature is normal
        }
    }

    if(ccu.vibrationSensor.GetVibrationFlag())
    {
        ccu.errWrnManager.addWarningCode(WarningCodes::VIBRATION_DETECTED); // Add vibration warning code

        // vibration flag is just used to add warning code to the error manager
        // and now it can be cleared
        ccu.vibrationSensor.ClearVibrationFlag(); // Clear the vibration flag after handling

        digitalWrite(LED_BUILTIN, HIGH); // Turn on the LED to indicate vibration detected
        lastLedONTime = currentMillis;
    }

    if(currentMillis - lastLedONTime >= VIBRATION_LED_PERIOD_TO_BE_ON_MS)
    {
        digitalWrite(LED_BUILTIN, LOW); // Turn off the LED
    }

    delay(10);
}
