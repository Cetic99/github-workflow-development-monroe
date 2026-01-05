INSERT INTO "IdentificationCardStatus" ("Id", "Code", "Description") VALUES (3, 'inactive', 'Card is inactive');

UPDATE "IdentificationCard" 
SET "IdentificationCardStatusId" = (SELECT "Id" FROM "IdentificationCardStatus" WHERE "Code" = 'inactive')
WHERE "ValidTo" < CURRENT_DATE;