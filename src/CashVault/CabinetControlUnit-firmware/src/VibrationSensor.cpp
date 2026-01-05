#include "Peripherals.h"

void VibrationSensorISR();
volatile bool vibrationFlag = false; // Flag to indicate vibration detection

VibrationSensor::VibrationSensor(int pin)
{
    this->pins = {pin};
    this->modes = {INPUT_PULLUP};
}

VibrationSensor::~VibrationSensor()
{}

void VibrationSensor::ConfigurePins()
{
    if(pins[0] == 2 || pins[0] == 3)
    {
        // Set pin mode for interrupt pins
        pinMode(pins[0], modes[0]);
        
        // Attach interrupt handler
        attachInterrupt(digitalPinToInterrupt(pins[0]), VibrationSensorISR, RISING);
    }
}

void VibrationSensor::SetVibrationFlag()
{
    vibrationFlag = true; // Set the vibration flag to true
}

void VibrationSensor::ClearVibrationFlag()
{
    vibrationFlag = false; // Clear the vibration flag
}

bool VibrationSensor::GetVibrationFlag()
{
    return vibrationFlag;
}

volatile unsigned long startTime = 0;
volatile unsigned long duration = 0;

void VibrationSensorISR()
{
    // Handle vibration detection
    VibrationSensor::SetVibrationFlag(); // Set the vibration flag    


    /////////////////////////////////////////////////////////////////////////
    // TODO: Sensitivity adjustment - debugging code... 
    // NOTE: Also change interrupt mode to 'CHANGE'
    // if (digitalRead(2) == HIGH) 
    // {
    //     startTime = micros();  // Capture start time when pin goes HIGH
    // } 
    // else 
    // {
    //     duration = micros() - startTime;  // Calculate duration when pin goes LOW
    // }


    // if (duration > 300000) 
    // {
    //     VibrationSensor::SetVibrationFlag(); // Set the vibration flag
    // }
    ///////////////////////////////////////////////////////////////////////////
}
