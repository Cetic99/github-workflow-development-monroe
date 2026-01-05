INSERT INTO "TicketType" ("Code", "Description") VALUES ('TITO', 'Casino cashout ticket - ticket in ticket out - can be accepted by bill acceptor');
INSERT INTO "TicketType" ("Code", "Description") VALUES ('CashConfirmation', 'Standard cash confirmation - with QR code');

INSERT INTO "TransactionType" ("Code", "Description") VALUES ('Credit', 'Credit transaction');
INSERT INTO "TransactionType" ("Code", "Description") VALUES ('Debit', 'Debit transaction');

INSERT INTO "TransactionStatus" ("Code", "Description") VALUES ('Pending', 'Pending transaction');
INSERT INTO "TransactionStatus" ("Code", "Description") VALUES ('Completed', 'Completed transaction');
INSERT INTO "TransactionStatus" ("Code", "Description") VALUES ('PartiallyCompleted', 'Partially completed transaction');
INSERT INTO "TransactionStatus" ("Code", "Description") VALUES ('Failed', 'Failed transaction');