#include "Peripherals.h"

#define DOOR_CLOSED         (0x80)
#define DOOR_OPEN           (0x8F)

DoorSensor::DoorSensor(std::string name, int id, int pin)
{
    this->name = name;
    this->id = id;
    this->pins = {pin};
    this->modes = {INPUT_PULLUP};
}

DoorSensor::~DoorSensor()
{}

void DoorSensor::ConfigurePins()
{
    pinMode(pins[0], modes[0]);
}

uint8_t DoorSensor::getStatus()
{
    uint8_t status = DOOR_CLOSED;

    if(digitalRead(pins[0]))
    {
        status = DOOR_OPEN;
    }
    else
    {
        status = DOOR_CLOSED;
    }
    return status;
}