#pragma once

#include <Arduino.h>
#include <vector>
#include <string>

enum peripheralType
{
    IN,
    OUT,
    INOUT
};

class Peripheral
{
    protected:
        std::vector<int> pins;
        std::vector<uint8_t> modes;
        std::string name;
        int id;

    public:
        Peripheral() {};
        ~Peripheral() {};
        virtual void ConfigurePins() = 0;
        int getId() { return id; }
};

class DoorSensor : public Peripheral
{
    public:
        DoorSensor(std::string name, int id, int pin);
        ~DoorSensor();
        void ConfigurePins() override;
        uint8_t getStatus();
};

class Buzzer : public Peripheral
{
    public:
        Buzzer();
        ~Buzzer();
};

class LED : public Peripheral
{
    public:
        LED();
        ~LED();
};

class VibrationSensor : public Peripheral
{
    public:
        VibrationSensor(int pin);
        ~VibrationSensor();
        void ConfigurePins() override;
        static void SetVibrationFlag();
        static void ClearVibrationFlag();
        static bool GetVibrationFlag();

    private:
        void setInterruptHandler(void (*handler)());
};
