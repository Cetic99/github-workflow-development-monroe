#include "CcuStatusResponse.h"

CcuStatusResponse::CcuStatusResponse(uint8_t cmd)
{
    uint8_t errCnt = ccu.errWrnManager.getErrorCodeCount();
    uint8_t wrnCnt = ccu.errWrnManager.getWarningCodeCount();
    uint8_t codeCnt = errCnt + wrnCnt;

    this->cmd = cmd;
    this->msgLength = MSG_HEADER_SIZE + MSG_LENGTH_SIZE + MSG_CMD_SIZE + MSG_CRC_SIZE + (codeCnt == 0 ? 1 : codeCnt);
    
    if (codeCnt > 0)
    {
        // Prepare payload with all error and all warning codes
        this->payload = new uint8_t[codeCnt];
        for (uint8_t i = 0; i < codeCnt; i++)
        {
            if (i < errCnt)
            {
                this->payload[i] = ccu.errWrnManager.getErrorCodeByIndex(i);
            }
            else
            {
                this->payload[i] = ccu.errWrnManager.getWarningCodeByIndex(i - errCnt);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        // TODO: NOTE: This is temporary solution because there is possibility that this response will not be received by host
        // due to timeout, connection problems or other issues. In that case, the error and warning codes will be lost.
        // In the future, we should implement a mechanism to ACK the response and resend it if not received. 
        // Other solution is to clear 'sticky' codes only if host request to clear them.
        ccu.errWrnManager.clearErrorCodes(); // Clear error codes after sending the response
        ccu.errWrnManager.clearWarningCodes(); // Clear warning codes after sending the response
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
    else
    {
        this->payload = new uint8_t[1];
        this->payload[0] = CODE_OK; // No errors or warnings
    }

    this->crc = calculateCRC();
    // Write data to serial port
    this->send();
}

CcuStatusResponse::~CcuStatusResponse()
{
    if (payload != nullptr)
    {
        delete[] payload;
        payload = nullptr;
    }
}
