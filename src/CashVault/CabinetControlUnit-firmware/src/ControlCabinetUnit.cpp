#include "ControlCabinetUnit.h"

ControlCabinetUnit::ControlCabinetUnit()
{}

ControlCabinetUnit::~ControlCabinetUnit()
{}

void ControlCabinetUnit::init()
{
    for (size_t i = 0; i < DOOR_SENSOR_COUNT; i++)
    {
        doorSensors[i].ConfigurePins();
    }

    temperatureSensor.SensorInit();
    temperatureSensor.measure(); // Initial measurement to set the temperature value

    vibrationSensor.ConfigurePins(); // Configure vibration sensor pins
}