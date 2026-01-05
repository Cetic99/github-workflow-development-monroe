#pragma once

#include "Peripherals.h"
#include <OneWire.h>
#include <DallasTemperature.h>
#include <DHT.h>

#define ERROR_TEMP (-199.99) // Error value when reading temperature fails

/**
 * @brief Defines the resolution for DS18B20 temperature sensor.
 * 
 * Resolution options and precision:
 * - 9-bit:  ~93.75ms   (fastest, least precise, ±0.5°C)
 * - 10-bit: ~187.5ms   (±0.25°C)
 * - 11-bit: ~375ms     (±0.125°C)
 * - 12-bit: ~750ms     (slowest, most precise, ±0.0625°C)
 * 
 * Higher resolution increases precision but requires longer conversion time.
 */
#define TEMP_DS18B20_RESOLUTION (9) // 9-bit resolution for DS18B20

enum TempSensorType
{
    DHT_11 = 0,
    DHT_22,
    DS18B20,
};

/**
 * @class TemperatureSensor
 * @brief A class representing a temperature sensor peripheral.
 * 
 * The TemperatureSensor class provides an interface for managing and interacting
 * with different types of temperature sensors, including initialization, configuration,
 * and temperature measurement.
 */
class TemperatureSensor : public Peripheral
{
public:
    /**
     * @brief Constructs a TemperatureSensor object with the specified name, ID, and pin.
     * 
     * @param name The name of the temperature sensor.
     * @param id The unique identifier for the sensor.
     * @param pin The GPIO pin number associated with the sensor.
     */
    TemperatureSensor(std::string name, int id, int pin);

    /**
     * @brief Constructs a TemperatureSensor object with the specified name, ID, pin, and sensor type.
     * 
     * @param name The name of the temperature sensor.
     * @param id The unique identifier for the sensor.
     * @param pin The GPIO pin number associated with the sensor.
     * @param sensorType The type of temperature sensor (e.g., DHT or Dallas).
     */
    TemperatureSensor(std::string name, int id, int pin, TempSensorType sensorType);

    /**
     * @brief Destructor for the TemperatureSensor class.
     * 
     * Cleans up any dynamically allocated resources associated with the sensor.
     */
    ~TemperatureSensor();

    /**
     * @brief Configures the GPIO pins required for the temperature sensor.
     * 
     * This method overrides the ConfigurePins method in the Peripheral base class.
     */
    void ConfigurePins() override;

    /**
     * @brief Retrieves the last measured temperature value.
     * 
     * @return The temperature value in degrees Celsius. If an error occurs, 
     *         the value ERROR_TEMP is returned.
     */
    float getTemperature();

    /**
     * @brief Initiates a temperature measurement.
     * 
     * @return True if the measurement was successful, false otherwise.
     */
    bool measure();

    /**
     * @brief Initializes the temperature sensor based on the specified type.
     * 
     * @return True if initialization was successful, false otherwise.
     */
    bool SensorInit();

private:

    TempSensorType sensorType; ///< The type of temperature sensor being used.
    float temperature = ERROR_TEMP; ///< The last measured temperature value.

    // Sensor objects
    DHT* sensorDHT = nullptr; ///< Pointer to a DHT sensor object (if applicable).
    DallasTemperature* sensorDS = nullptr; ///< Pointer to a DallasTemperature sensor object (if applicable).
    OneWire* oneWire = nullptr; ///< Pointer to a OneWire object (if applicable).
};
