# Monroe - Basic Communication Protocol Specification

**Document Version**: 1.0.6
**Date**: 2025-11-15

## 1. Overview

The Monroe Basic Communication Protocol is designed for redemption and printing of casino tickets that can be accepted on machines connected to Casino Management System (CMS). It also facilitates the communication of events and machine money status between the Monroe machine and the CMS.

In this setup, the Monroe machine functions as a client, sending requests to the CMS, which serves as the server. Communication between the client and server is conducted over HTTPS using the POST method. All client requests, along with their parameters, are transmitted in JSON format.

## 2. Supported Commands

| Command                | Description                                             |
| ---------------------- | ------------------------------------------------------- |
| `ticketRedemption`     | Ask server for ticket status                            |
| `ticketRedemptionAck`  | Acknowledge successful ticket redemption                |
| `ticketRedemptionNack` | Communicate ticket redemption failure                   |
| `ticketPrintRequest`   | Request barcode for a new ticket                        |
| `ticketPrintComplete`  | Acknowledge successful ticket printing                  |
| `ticketPrintFailed`    | Notify ticket printing failure                          |
| `transaction`          | Report end of customer transaction and machine contents |
| `event`                | Report event occurrence on machine                      |
| `healthCheck`          | Check CMS available                                     |

## 3. ticketRedemption

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Customer inserts a ticket into the Bill Acceptor.

#### Request:

```json
{
  "type": "ticketRedemption",
  "date": "<timestamp>",
  "language": "<language_code>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>"
}
```

#### Response:

```json
{
  "type": "ticketRedemption",
  "date": "<timestamp>",
  "language": "<language_code>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>",
  "responseCode": <int>,
  "amount": "<amount>",
  "amountWithTaxes": "<amount>",
  "taxes": <int>,
  "reason": "<string>"
}
```

#### Response Codes:

- `0`: Ticket is valid.
- `1`: Ticket unknown.
- `2`: Ticket already paid.
- `3`: Ticket expired.
- `4`: Ticket not valid.
- `100`: General CMS error.

---

## 4. ticketRedemptionAck

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Successful response to `ticketRedemption`.
- Ticket is stacked into Bill Acceptor.

#### Request:

```json
{
  "type": "ticketRedemptionAck",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>"
}
```

#### Response:

responseCode = 0 (communication successful)

```json
{
  "type": "ticketRedemptionAck",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>",
  "responseCode": 0
}
```

---

## 5. ticketRedemptionNack

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Unsuccessful response to `ticketRedemption`.
- Ticket is not accepted by Bill Acceptor.

#### Request:

```json
{
  "type": "ticketRedemptionNack",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>"
}
```

#### Response:

responseCode = 0 (communication successful)

```json
{
  "type": "ticketRedemptionNack",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>",
  "responseCode": 0
}
```

---

## 6. ticketPrintRequest

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Customer requested ticket printing.

#### Request:

```json
{
  "type": "ticketPrintRequest",
  "date": "<timestamp>",
  "language": "<language_code>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "amount": <int>
}
```

#### Response:

```json
{
  "type": "ticketPrintRequest",
  "date": "<timestamp>",
  "language": "<language_code>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "responseCode": <int>,
  "barcode": "<18_digit_barcode>",
  "amount": "<amount>",
  "validity": <int>,
  "amountText": "<string>",
  "amountInWords": "<string>",
  "title": "<string>",
  "location": "<string>",
  "address1": "<string>",
  "address2": "<string>"
}
```

---

## 7. ticketPrintComplete

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Successful response to `ticketPrintRequest`.
- Ticket printed with received data and pulled out from printer chute before timeout exceeded.

#### Request:

```json
{
  "type": "ticketPrintComplete",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>"
}
```

#### Response:

responseCode = 0 (communication successful)

```json
{
  "type": "ticketPrintComplete",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>",
  "responseCode": 0
}
```

---

## 8. ticketPrintFailed

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Unsuccessful response to `ticketPrintRequest`.
- Ticket printing failed for any reason. Ticket is considered printed only when it's pulled out from printer chute before timeout exceeded.

#### Request:

```json
{
  "type": "ticketPrintFailed",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>"
}
```

#### Response:

responseCode = 0 (communication successful)

```json
{
  "type": "ticketPrintFailed",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "barcode": "<18_digit_barcode>",
  "responseCode": 0
}
```

---

## 9. event

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Event has happened on the machine (for example peripheral errors, attendant operations etc…)

#### Request:

```json
{
  "type": "event",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>",
  "event": {
    "uuid": "<uuid>",
    "timestamp": "<timestamp>",
    "message": "<string>",
    "type": "<event_type>",
    "deviceType": "<device_type>",
    "name": "<event_name>",
	"user": {
      "username": "<string>",
	  "fullName": "<string>",
	  "company": "<string>"
	}
  }
}
```

#### Response:

200 OK

```json
{}
```

#### Remarks:

<event_type>, <device_type>, <event_name> depends on actual device configuration. All values are string data type.

---

## 10. healthCheck

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.

#### Request:

```json
{
  "type": "healthCheck",
  "date": "<timestamp>",
  "machineName": "<string>",
  "secretKey": "<string>"
}
```

#### Response:

200 OK (CMS is available and ready for comunication)

This request is sent on every few seconds, to keep device informed about CMS health status. If there are no response from CMS or response is not 200 OK, device considers that as CMS integration is unavailable.

---

## 11. transaction

### Prerequisites:

- CMS integration is enabled on Monroe machine.
- Machine name is set.
- Secret key is set.
- Monroe machine sends transaction

#### Request:

```json
{
   "transactionCode": "<transaction_code>",
   "moneyStatus":{
      "dispenserCassettes":[
         {
            "cassetteNumber": <int>,
            "currencyIsoCode": "<string>",
            "billDenomination": <int>,
            "currentBillCount": <int>
         },
         ...
      ],
      "acceptorCassette": {
        "billCount": <int>,
        "billAmount": "<amount>",
        "ticketCount": <int>,
        "ticketAmount": "<amount>"
      }
   },
   "user": {
      "username": "<string>",
	  "fullName": "<string>",
	  "company": "<string>"
   },
   "type": "transaction",
   "date": "<timestamp>",
   "machineName": "<string>",
   "secretKey": "<string>"
}
```

#### Response:

200 OK

```json
{}
```

# Remarks:

Common properties data format and type:

- `<timestamp>` : Unix timestamp with millis
- `<amount>` : Money amount represented with integer amount, decimal point moved two places to the right. Example: 12.34 EUR is represented as 1234
- `<18_digit_barcode>` : string representation of ticket barcode numbers, 18 digits length
- `<language_code>` : Player selected language, ISO 639-1 code
