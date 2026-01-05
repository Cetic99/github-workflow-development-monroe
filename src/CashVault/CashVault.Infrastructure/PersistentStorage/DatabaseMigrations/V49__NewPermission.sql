INSERT INTO "Permission" ("Id", "Code", "Description") VALUES (11, 'Logs', 'Logs');

INSERT INTO "OperatorPermission"
("Id", "Version", "Guid", "Created", "CreatedBy", "Updated", "UpdatedBy", "OperatorId", "PermissionId")
VALUES(40, 1, '9225fac0-f18a-4069-b820-4be86bb93800', '2025-06-24 16:28:01.800', 'admin', NULL, NULL, (select "Id" FROM "Operator" WHERE "Username" = 'admin'),
(SELECT "Id" FROM "Permission" WHERE "Code" = 'Logs'));