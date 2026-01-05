ALTER TABLE "MoneyStatusTransaction"
ADD CONSTRAINT FK_MONEYSTATUSTRANSACTION_MSTTYPEIDFK
FOREIGN KEY ("MoneyStatusTransactionTypeId") REFERENCES "MoneyStatusTransactionType"("Id");


UPDATE "MoneyStatusTransaction"
SET "MoneyStatusTransactionTypeId" = "TransactionTypeId";

UPDATE "MoneyStatusTransaction"
SET "DeviceType" = 'BillDispenser'
WHERE "TransactionKind" = 'DispenserMoneyStatusTransaction';

UPDATE "MoneyStatusTransaction"
SET "DeviceType" = 'BillAcceptor'
WHERE "TransactionKind" = 'AcceptorMoneyStatusTransaction';
