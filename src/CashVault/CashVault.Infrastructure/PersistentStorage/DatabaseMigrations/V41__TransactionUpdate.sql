
UPDATE "Transaction" SET "PreviousCreditAmount" = 0 WHERE "PreviousCreditAmount" IS NULL;
ALTER TABLE "Transaction" ALTER "PreviousCreditAmount" SET DEFAULT 0;
ALTER TABLE "Transaction" ALTER "PreviousCreditAmount" SET NOT NULL;

UPDATE "Transaction" SET "NewCreditAmount" = 0 WHERE "NewCreditAmount" IS NULL;
ALTER TABLE "Transaction" ALTER "NewCreditAmount" SET DEFAULT 0;
ALTER TABLE "Transaction" ALTER "NewCreditAmount" SET NOT NULL;
