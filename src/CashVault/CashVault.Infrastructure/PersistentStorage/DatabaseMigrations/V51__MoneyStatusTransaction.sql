INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('bb700b46-b475-4f52-a65a-031198181bfc', 1, 'bs', 'harvest', 'Pražnjenje');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('eae66bad-53c3-4324-8e7a-f42e1b1b43bc', 1, 'us', 'harvest', 'harvest');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('fcb1875e-5d6d-4080-bf43-7b7b75fdba54', 1, 'bs', 'refill', 'Dopuna');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('e66108f5-f3ab-436f-a9c2-56eff10459a1', 1, 'us', 'refill', 'refill');

ALTER TABLE "MoneyStatusTransaction" ADD "OldDeviceBillAmount"  DECIMAL(18, 2) DEFAULT 0;
ALTER TABLE "MoneyStatusTransaction" ADD "NewDeviceBillAmount"  DECIMAL(18, 2) DEFAULT 0;
