---- Transaction.PreviousCreditAmount
ALTER TABLE "Transaction" ADD "PreviousCreditAmount" DECIMAL(18,2);

---- Transaction.NewCreditAmount
ALTER TABLE "Transaction" ADD "NewCreditAmount" DECIMAL(18,2);

---- Translations
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('c1c5677b-2c27-4b4e-8526-9684ff24a53d', 1, 'bs', 'Credit balance', 'Saldo');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('9c137588-4130-407b-bff7-2ef1b1692e8a', 1, 'us', 'Credit balance', 'Balance');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('d14ea0be-5862-4d37-834f-87a5bef2cf7d', 1, 'bs', 'New balance', 'Novi');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('d106fc5b-73e1-49b1-8b42-c4184bddfdbb', 1, 'us', 'New balance', 'New');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('8ad40a3e-0842-45b3-9bb1-f6976358aa1c', 1, 'bs', 'Previous balance', 'Prethodni');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('8a918b77-b378-4015-8c99-3573fbe79a0d', 1, 'us', 'Previous balance', 'Previous');

