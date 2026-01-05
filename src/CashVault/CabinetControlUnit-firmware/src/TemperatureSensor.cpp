#include "TemperatureSensor.h"

TemperatureSensor::TemperatureSensor(std::string name, int id, int pin, TempSensorType sensorType)
{
    this->name = name;
    this->id = id;
    this->pins = {pin};
    this->modes = {INPUT};
    this->sensorType = sensorType;
}

TemperatureSensor::TemperatureSensor(std::string name, int id, int pin) : TemperatureSensor(name, id, pin, DS18B20)
{}

TemperatureSensor::~TemperatureSensor()
{
    if (oneWire != nullptr)
    {
        delete oneWire;
        oneWire = nullptr;
    }

    if (sensorDS != nullptr)
    {
        delete sensorDS;
        sensorDS = nullptr;
    }

    if (sensorDHT != nullptr)
    {
        delete sensorDHT;
        sensorDHT = nullptr;
    }
}

void TemperatureSensor::ConfigurePins()
{
    // Configuration is done by the libraries, so we don't need to set pin modes here
    return;
}


/**
 * @brief Measures the temperature using the configured sensor type.
 * 
 * This function reads the temperature from the sensor based on its type.
 * It supports DHT sensors (DHT11, DHT22) and DS18B20 sensors. If the sensor
 * is not properly initialized or the reading fails, the temperature is set
 * to an error value (ERROR_TEMP), and the function returns false.
 * 
 * @return true  If the temperature was successfully read from the sensor.
 * @return false If the temperature reading failed or the sensor type is invalid.
 */
bool TemperatureSensor::measure()
{
    // Read temperature from the sensor based on its type
    if(sensorType == DHT_11 || sensorType == DHT_22)
    {
        // Read temperature from DHT sensor
        if (sensorDHT != nullptr)
        {
            temperature = sensorDHT->readTemperature();
            if (isnan(temperature))
            {
                // Failed to read from DHT sensor
                temperature = ERROR_TEMP;
                return false;
            }
            else
            {
                return true; // Successfully read temperature
            }
        }
    }
    else if (sensorType == DS18B20)
    {
        // Read temperature from DS18B20 sensor
        if (sensorDS != nullptr)
        {
            sensorDS->requestTemperatures();
            temperature = sensorDS->getTempCByIndex(0 /* TODO: check! this->id */);
            if (temperature == DEVICE_DISCONNECTED_C)
            {
                // Failed to read from DS18B20 sensor
                temperature = ERROR_TEMP;
                return false;
            }
            else
            {
                return true; // Successfully read temperature
            }
            
        }
    }

    // Invalid sensor type or uninitialized sensor
    return false;
}

float TemperatureSensor::getTemperature()
{
    return temperature;
}

bool TemperatureSensor::SensorInit()
{
    if (sensorType == DHT_11 || sensorType == DHT_22)
    {
        // initialize DHT sensor
        if (sensorDHT != nullptr)
        {
            delete sensorDHT;
            sensorDHT = nullptr;
        }
        sensorDHT = new DHT(pins[0], (sensorType == DHT_11) ? DHT11 : DHT22);
        if (sensorDHT != nullptr)
        {
            sensorDHT->begin();
            return true;
        }
        else
        {
            // failed to initialize DHT sensor
            return false;
        }
    }
    else if (sensorType == DS18B20)
    {
        if(oneWire != nullptr)
        {
            delete oneWire;
            oneWire = nullptr;
        }

        if(sensorDS != nullptr)
        {
            delete sensorDS;
            sensorDS = nullptr;
        }
        
        // initialize DS18B20 sensor
        oneWire = new OneWire(pins[0]);
        sensorDS = new DallasTemperature(oneWire);
        if(oneWire != nullptr && sensorDS != nullptr)
        {
            sensorDS->begin();
            sensorDS->setResolution(TEMP_DS18B20_RESOLUTION); // Set resolution for DS18B20
            return true;
        }
        else
        {
            // failed to initialize DS18B20 sensor
            return false;
        }
    }
    else
    {
        // invalid sensor type
        return false; // Indicate failure to initialize
    }
}
