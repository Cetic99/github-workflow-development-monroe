| Cabinet Control Unit  Comunication protocol |
| --- |

# RS232 communication protocol

Two types of messages:

* Request: HEADER 0x11
* Response: HEADER 0x22

| Request message | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 | Byte 2 | Payload | 2 bytes CRC |
| 0x11 | Full message length (header + byte 1 + byte 2 + Payload + CRC) | Command | Data | CRC |

| Response message | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 | Byte 2 | Payload | 2 bytes CRC |
| 0x22 | Full message length (header + byte 1 + byte 2 + Payload + CRC) | Command | Data | CRC |


---

## Messages

### All Door Sensor status

This message is used for Request door sensor status from Arduino.

| Status request message | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 (MSG len) | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x11 | 0x06 | 0x53 (ascii ‘**S**’) | 0x61 (ascii ‘**a**’) – **a**ll door sensors | CRC |

Reponse message to Status request message for all sensors:

| Status response message | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x22 | 3 bytes + payload len + CRC len | 0x53 (ascii ‘**S**’) | Data[0] = door\_1  Data[1] = door\_2  …  Data[N-1] = door\_n  **Value**: 0x80 (door closed) or 0x8F (door opened) | CRC |

---

### Door sensor status

This message is used to request sensor status for single specific Door sensor.

| Status request message of specific sensor ID | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 (MSG len) | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x11 | 0x06 | 0x73  Ascii ‘**s**’ *(****s****ensor)* | ID of single specific sensor | CRC |

Response message to a Status request for a specific sensor ID:

| Status response message for specific sensor ID | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x22 | Full msg len | 0x73  Ascii ‘**s**’ *(****s****ensor)* | Data[0] = sensor id (data from request message)  Data[1] = 0x80 (door closed) or 0x8F (door opened) or 0xFF (invalid SensorID) | CRC |


---

### Firwmare version

This messasge is used to get Firmware version of CabinetControlUnit.

| Get Firmware version | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 (MSG len) | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x11 | 0x06 | 0x66 – **f**irmware cmd  (ascii ‘**f**’) | 0x00 | CRC |

This message is a response for the Get Firmware version request

| Firwmare version response | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x22 | Full msg len | 0x66 – **f**irmware cmd  (ascii ‘**f**’) | Firmware version as string | CRC |


---

### Invalid command response

This response is sent when requested command is not valid, not recognized, or message with wrong CRC has been received.

| Firwmare version response | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x22 | Full msg len | 0x45 – ‘**E**’rror | CMD from request msg – Failed command | CRC |


---

### Cabinet Control Unit Status

This message is used to request status of the Cabinet Control Unit (general status).

| CCU status request | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 (MSG len) | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x11 | 0x06 | 0x63 – ascii ‘**c**’ | 0x00 | CRC |


Response to Cabinet Control Unit status msg request.

| CCU status response | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 (MSG len) | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x22 | 0x06 | 0x63 – ascii ‘**c**’ | 0x00 - Status OK, or error code | CRC |

#### Error codes
- CodeType TEMPERATURE_SENSOR_ERROR = 0x01

When temperature can't be read, or sensor can't be detected.

#### Warning codes
- CodeType HIGH_TEMPERATURE     = 0x80;
- CodeType VIBRATION_DETECTED   = 0x81;

---

### Temperature Request

This message is used to request temperature from temperature sensor.

| Temperature request | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 (MSG len) | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x11 | 0x06 | 0x74 – ascii ‘**t**’ | 0x00 - or other sensor ID (when more than one sensor is available) | CRC |


| Temperature response | | | | |
| --- | --- | --- | --- | --- |
| HEADER | Byte 1 (MSG len) | Byte 2 (CMD) | Payload | 2 bytes CRC |
| 0x22 | Full msg len | 0x74 – ascii ‘**t**’ | Payload data[0..3]  | CRC |


**Payload data**

* data[0] - Sensor ID
* data[1] - sign (0x00 positive, 0x01 for negative)
* data[2] - temperature in Celsius
* data[3] - fraction part of temperature

Example: Temperature -12.34°C for sensor with ID=5 : data[] = [0x05, 0x01, 0x0C, 0x22]

---