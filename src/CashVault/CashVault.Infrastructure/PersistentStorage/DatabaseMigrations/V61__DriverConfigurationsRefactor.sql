UPDATE "Configuration" SET "Key" = 'ITITOPrinterConfiguration' WHERE "Key"  = 'TicketPrinterConfiguration';
UPDATE "Configuration" SET "Key" = 'IBillDispenserConfiguration' WHERE "Key"  = 'BillDispenserConfiguration';
UPDATE "Configuration" SET "Key" = 'IBillAcceptorConfiguration' WHERE "Key"  = 'BillAcceptorConfiguration';
UPDATE "Configuration" SET "Key" = 'IUserCardReaderConfiguration' WHERE "Key"  = 'UserCardReaderConfiguration';
UPDATE "Configuration" SET "Key" = 'ICabinetConfiguration' WHERE "Key"  = 'CabinetConfiguration';

UPDATE "Configuration"
SET "Value" = REPLACE("Value", 'TicketPrinterType', 'TITOPrinterType')
WHERE "Value" CONTAINING 'TicketPrinterType' AND "Key"  = 'MainConfiguration';


UPDATE "Configuration"
SET "Value" = REPLACE("Value", 'TicketPrinterInterface', 'TITOPrinterInterface')
WHERE "Value" CONTAINING 'TicketPrinterInterface' AND "Key"  = 'MainConfiguration';