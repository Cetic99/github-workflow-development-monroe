UPDATE "Configuration"
SET "Value" = REPLACE(
    "Value",
    '"TerminalType":"gamingatm"',
    '"TerminalTypes":["gamingatm"]'
)
WHERE "Key"  = 'MainConfiguration';


INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('36a83be3-d0a0-46b2-8753-cd4374c1b700', 1, 'bs', 'entertainment', 'Zabava');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('b1d5109b-656a-466c-a83b-5db83b846d21', 1, 'us', 'entertainment', 'Entertainment');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('353ea02a-4933-4e51-8f5d-f8a01a8a458e', 1, 'bs', 'bankingatm', 'Bankomat');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('d86ac216-1ff0-45ce-ab2c-b3ddb48b1cdd', 1, 'us', 'bankingatm', 'Banking ATM');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('20470522-8b8f-4e02-8b19-3af7950c20f5', 1, 'bs', 'gamingatm', 'Kazino ATM');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('966a15b3-6ace-4a28-bc21-baebabaa2f1c', 1, 'us', 'gamingatm', 'Gaming ATM');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('61496b70-09ca-4728-93ba-9997c657886a', 1, 'bs', 'parcellocker', 'Paketomat');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('60cb03b2-cbb3-4382-b2c2-2ef0f18d003f', 1, 'us', 'parcellocker', 'Parcel Locker');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('a997699c-b79a-44da-8b06-5ea0636af9bf', 1, 'bs', 'billacceptor', 'Bill Acceptor');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('27660e50-be35-45bb-a62a-a055b7e751f4', 1, 'us', 'billacceptor', 'Bill Acceptor');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('56a767cf-c3b8-4e5f-9573-ea726c05b741', 1, 'bs', 'billdispenser', 'Bill Dispenser');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('05262e3b-5555-48cb-9278-0298f1f515b9', 1, 'us', 'billdispenser', 'Bill Dispenser');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('6d130f99-caa4-4109-b968-43c9ce8916ff', 1, 'bs', 'thermalprinter', 'Thermal Printer');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('e61249fa-5216-4ebb-9bca-978a1f828fb2', 1, 'us', 'thermalprinter', 'Thermal Printer');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('dd1f877f-f093-496a-a8db-c8a0a8c497e8', 1, 'bs', 'titoprinter', 'TITO Printer');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('136aecf6-7e6a-46b7-92f1-499524244e54', 1, 'us', 'titoprinter', 'TITO Printer');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('fd228c2c-bb4a-4731-9d40-a53f310b68da', 1, 'bs', 'usercardreader', 'User Card Reader');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('1f0a7be4-af76-4c92-ac63-5f6144197b4d', 1, 'us', 'usercardreader', 'User Card Reader');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('e31a1313-0c1a-4669-b3cb-ba1a8c63d7c6', 1, 'bs', 'coinacceptor', 'Coin Acceptor');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('9e1e127b-47bd-4907-8ebe-8abb2c2efa45', 1, 'us', 'coinacceptor', 'Coin Acceptor');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('f886089d-eac7-446c-a0b2-3a3a62fdd134', 1, 'bs', 'coindispenser', 'Coin Dispenser');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('e2be1a8d-24d9-4415-8921-c7ecaa328e8d', 1, 'us', 'coindispenser', 'Coin Dispenser');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('6fd483c9-ddf5-4030-b9af-0e03c7b948f0', 1, 'bs', 'coinrecycler', 'Coin Recycler');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('4a224525-fcfc-40c2-8ca3-a81e8cb56762', 1, 'us', 'coinrecycler', 'Coin Recycler');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('c11671f6-87a9-4c7e-b0fb-8a2578b4e5dd', 1, 'bs', 'posterminal', 'Post Terminal');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('329a17f1-841f-4f2c-b031-7f7770e21ea8', 1, 'us', 'posterminal', 'Post Terminal');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('fb764dcf-eee5-4b98-bf9a-c611e31aaff2', 1, 'bs', 'iccardreader', 'IC Card Reader');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('aa6e8408-62a8-4b7b-8162-0e4fdc784297', 1, 'us', 'iccardreader', 'IC Card Reader');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('c177d556-22d4-4150-bb2d-679b3efe2d88', 1, 'bs', 'Add device', 'Dodaj uređaj');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('20608e76-4165-4105-a227-f954ac1486f3', 1, 'us', 'Add device', 'Add device');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('ca6a5a5e-edea-44d3-88ac-07a649b85ba7', 1, 'bs', 'Add', 'Dodaj');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('21390959-0c83-4050-8187-8800a1feb4f6', 1, 'us', 'Add', 'Add');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('c60eac7e-69d2-4a18-8baf-5663f55a3255', 1, 'bs', 'Config device', 'Konfiguriši uređaj');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('1eb95058-e52d-4393-94e9-ac38a15b13dd', 1, 'us', 'Config device', 'Config device');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('068c34ce-cf24-4e4c-a82b-81cd036c2d2a', 1, 'bs', 'Are you sure?', 'Da li ste sigurni?');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('4a4a2784-c125-44b4-b17a-6d0fe965fe65', 1, 'us', 'Are you sure?', 'Are you sure?');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('b4cde3e0-6b00-446e-80d5-2d42ad3ab903', 1, 'bs', 'Yes', 'Da');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('d2f62875-22f4-4ac1-aaa3-cd333a679f09', 1, 'us', 'Yes', 'Yes');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('fac0d079-12fa-4775-88ae-0feaea6e5c35', 1, 'bs', 'No added devices', 'Nema dodatih uređaja');
INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") VALUES ('7abfb9fd-c04c-4c62-8e14-a1b4fc61fa17', 1, 'us', 'No added devices', 'No added devices');

