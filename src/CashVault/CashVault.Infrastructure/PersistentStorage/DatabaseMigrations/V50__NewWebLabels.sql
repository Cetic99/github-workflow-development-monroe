UPDATE "Message"
SET "Value" = 'Štampaj i obriši zbirne vrijednosti'
WHERE "Key" = 'Print and clear totals' AND "LanguageCode" = 'bs';

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") 
VALUES ('5741a179-5b6d-431f-9e33-e32b4f210dd4', 1, 'bs', 
        'You can choose to print the report as-is or print and reset the totals after printing.', 
        'Možete izabrati da odštampate izvještaj u trenutnom obliku ili da ga odštampate i zatim obrišete ukupne vrijednosti.');

INSERT INTO "Message" ("Guid", "Version", "LanguageCode", "Key", "Value") 
VALUES ('3741a179-5b6d-4310-9e33-e32b4f210dd4', 1, 'us', 
        'You can choose to print the report as-is or print and reset the totals after printing.', 
        'You can choose to print the report as-is or print and reset the totals after printing.');
