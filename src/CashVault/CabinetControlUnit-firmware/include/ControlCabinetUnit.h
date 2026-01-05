#pragma once
#include <Arduino.h>
#include <vector>
#include "Peripherals.h"
#include "TemperatureSensor.h"
#include "ErrWrnManager.h"

// Global parameters and configuration

#define FIRMWARE_VERSION    "1.0.0"
#define BAUD_RATE           (19200)

// Message parameters
#define MSG_REQUEST_HEADER  (0x11)
#define MSG_RESPONSE_HEADER (0x22)
#define MSG_HEADER_SIZE     (1)
#define MSG_LENGTH_SIZE     (1)
#define MSG_CMD_SIZE        (1)
#define MSG_CRC_SIZE        (2)
#define MSG_MAX_LENGTH      (255)

// CRC default parameters
#define CRC_POLYNOMIAL      (0x1021)
#define CRC_INITIAL_VALUE   (0x0000)
#define CRC_FINAL_XOR_VALUE (0x0000)
#define CRC_REFLECT_IN      (true)
#define CRC_REFLECT_OUT     (true)

// Door Sensor
#define DOOR_SENSOR_COUNT   (4)

// Temperature Sensor
#define TEMP_SENSOR_COUNT       (1)
#define TEMP_SENSOR_TYPE        (DHT_11)
#define TEMP_MEASURE_PERIOD_MS  (5000) // Define measurement period
#define TEMP_SENSOR_PIN         (PIN_8)
#define TEMP_HIGH_THRESHOLD     (65) // Define high temperature threshold

typedef enum
{
    PIN_0 = 0,
    PIN_1 = 1,
    PIN_2 = 2,
    PIN_3 = 3,
    PIN_4 = 4,
    PIN_5 = 5,
    PIN_6 = 6,
    PIN_7 = 7,
    PIN_8 = 8,
    PIN_9 = 9,
    PIN_10 = 10,
    PIN_11 = 11,
    PIN_12 = 12,
    PIN_13 = 13,
    PIN_14 = 14,
} DigitalPinNumber_t;

/**
 * Arduino Uno/Nano Pin Overview:
 * ------------------------------
 * 
 * External Interrupts: PIN_2, PIN_3
 * PWM Pins: PIN_3, PIN_5, PIN_6, PIN_9, PIN_10, PIN_11
 * I2C Pins: PIN_A4 (SDA), PIN_A5 (SCL)
 * SPI Pins: PIN_10 (SS), PIN_11 (MOSI), PIN_12 (MISO), PIN_13 (SCK)
 * UART Pins: PIN_0 (RX), PIN_1 (TX)
 * Analog Inputs: Uno: PIN_A0 to PIN_A5, Nano: PIN_A0 to PIN_A7
 * 
 * Notes:
 * - PWM and Analog pins can also be used as digital I/O.
 */
class ControlCabinetUnit
{
    public:
        DoorSensor doorSensors[DOOR_SENSOR_COUNT] =
        {
            DoorSensor("Door_1", 1, PIN_4),
            DoorSensor("Door_2", 2, PIN_5),
            DoorSensor("Door_3", 3, PIN_6),
            DoorSensor("Door_4", 4, PIN_7)
        };
        TemperatureSensor temperatureSensor = TemperatureSensor("Temp1", 0, TEMP_SENSOR_PIN, TEMP_SENSOR_TYPE);
        VibrationSensor vibrationSensor = VibrationSensor(PIN_2); // Pin 2 for vibration sensor
        ErrWrnManager errWrnManager; // Error manager instance

        ControlCabinetUnit();
        ~ControlCabinetUnit();
        void init();
};

extern ControlCabinetUnit ccu;
