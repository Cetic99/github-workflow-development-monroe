#pragma once
#include <vector>
#include <cstdint>

// Error and Warning Codes
using CodeType = uint8_t;

constexpr CodeType CODE_ERROR = 0xFF; // General error code
constexpr CodeType CODE_OK = 0x00; // General success code

namespace ErrorCodes
{
    constexpr CodeType TEMPERATURE_SENSOR_ERROR = 0x01;
}
namespace WarningCodes
{
    constexpr CodeType HIGH_TEMPERATURE     = 0x80;
    constexpr CodeType VIBRATION_DETECTED   = 0x81;
}


/**
 * @class ErrWrnManager
 * @brief Manages error and warning codes for a system.
 *
 * This class provides functionality to add, remove, query, and manage
 * error and warning codes. It maintains separate lists for errors and warnings.
 */
class ErrWrnManager
{
    public:
        /**
         * @brief Adds an error code to the error list.
         * @param code The error code to add.
         * @return True if the code was added successfully, false otherwise.
         */
        bool addErrorCode(CodeType code);

        /**
         * @brief Adds a warning code to the warning list.
         * @param code The warning code to add.
         * @return True if the code was added successfully, false otherwise.
         */
        bool addWarningCode(CodeType code);

        /**
         * @brief Removes an error code from the error list.
         * @param code The error code to remove.
         * @return True if the code was removed successfully, false otherwise.
         */
        bool removeErrorCode(CodeType code);

        /**
         * @brief Removes a warning code from the warning list.
         * @param code The warning code to remove.
         * @return True if the code was removed successfully, false otherwise.
         */
        bool removeWarningCode(CodeType code);

        /**
         * @brief Checks if a specific error code exists in the error list.
         * @param code The error code to check.
         * @return True if the code exists, false otherwise.
         */
        bool hasErrorCode(CodeType code) const;

        /**
         * @brief Checks if a specific warning code exists in the warning list.
         * @param code The warning code to check.
         * @return True if the code exists, false otherwise.
         */
        bool hasWarningCode(CodeType code) const;

        /**
         * @brief Clears all error codes from the error list.
         */
        void clearErrorCodes();

        /**
         * @brief Clears all warning codes from the warning list.
         */
        void clearWarningCodes();

        /**
         * @brief Gets the count of error codes in the error list.
         * @return The number of error codes.
         */
        uint8_t getErrorCodeCount() const;

        /**
         * @brief Gets the count of warning codes in the warning list.
         * @return The number of warning codes.
         */
        uint8_t getWarningCodeCount() const;

        /**
         * @brief Retrieves an error code by its index in the error list.
         * @param index The index of the error code to retrieve.
         * @return The error code at the specified index.
         */
        CodeType getErrorCodeByIndex(uint8_t index) const;

        /**
         * @brief Retrieves a warning code by its index in the warning list.
         * @param index The index of the warning code to retrieve.
         * @return The warning code at the specified index.
         */
        CodeType getWarningCodeByIndex(uint8_t index) const;

    private:
        /**
         * @brief List of error codes.
         */
        std::vector<CodeType> errorCodes;

        /**
         * @brief List of warning codes.
         */
        std::vector<CodeType> warningCodes;
};
