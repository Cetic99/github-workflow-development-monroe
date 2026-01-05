#include "ErrWrnManager.h"

bool ErrWrnManager::addErrorCode(CodeType code)
{
    if (!hasErrorCode(code))
    {
        errorCodes.push_back(code);
        return true;
    }
    return false;
}

bool ErrWrnManager::addWarningCode(CodeType code)
{
    if (!hasWarningCode(code))
    {
        warningCodes.push_back(code);
        return true;
    }
    return false;
}

bool ErrWrnManager::removeErrorCode(CodeType code)
{
    auto it = std::find(errorCodes.begin(), errorCodes.end(), code);
    if (it != errorCodes.end())
    {
        errorCodes.erase(it);
        return true;
    }
    return false;
}

bool ErrWrnManager::removeWarningCode(CodeType code)
{
    auto it = std::find(warningCodes.begin(), warningCodes.end(), code);
    if (it != warningCodes.end())
    {
        warningCodes.erase(it);
        return true;
    }
    return false;
}

bool ErrWrnManager::hasErrorCode(CodeType code) const
{
    return std::find(errorCodes.begin(), errorCodes.end(), code) != errorCodes.end();
}

bool ErrWrnManager::hasWarningCode(CodeType code) const
{
    return std::find(warningCodes.begin(), warningCodes.end(), code) != warningCodes.end();
}

void ErrWrnManager::clearErrorCodes()
{
    errorCodes.clear();
}

void ErrWrnManager::clearWarningCodes()
{
    warningCodes.clear();
}

uint8_t ErrWrnManager::getErrorCodeCount() const
{
    return static_cast<uint8_t>(errorCodes.size());
}

uint8_t ErrWrnManager::getWarningCodeCount() const
{
    return static_cast<uint8_t>(warningCodes.size());
}

CodeType ErrWrnManager::getErrorCodeByIndex(uint8_t index) const
{
    if (index < errorCodes.size())
    {
        return errorCodes[index];
    }
    return CODE_ERROR; // Return general error code if index is out of range
}

CodeType ErrWrnManager::getWarningCodeByIndex(uint8_t index) const
{
    if (index < warningCodes.size())
    {
        return warningCodes[index];
    }
    return CODE_ERROR; // Return general error code if index is out of range
}
