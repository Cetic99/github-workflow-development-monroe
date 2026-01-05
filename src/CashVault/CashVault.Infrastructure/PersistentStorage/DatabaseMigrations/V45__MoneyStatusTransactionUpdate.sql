ALTER TABLE "MoneyStatusTransaction" ADD "MoneyStatusTransactionTypeId" INTEGER;


CREATE INDEX FK_MONEYSTATUSTRANSACTION_MSTTYPEID ON "MoneyStatusTransaction" ("MoneyStatusTransactionTypeId");

ALTER TABLE "MoneyStatusTransaction" ADD "DeviceType" VARCHAR(50);


