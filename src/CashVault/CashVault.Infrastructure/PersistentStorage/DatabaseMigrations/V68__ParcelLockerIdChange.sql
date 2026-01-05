DELETE FROM "Transaction" 
WHERE "ParcelLockerShipmentId" IS NOT NULL OR 
	"ParcelLockerId" IS NOT NULL;
	
DELETE FROM "ParcelLockerShipment";

ALTER TABLE "Transaction" DROP "ParcelLockerId";
ALTER TABLE "Transaction" ADD "ParcelLockerId" INTEGER;

ALTER TABLE "ParcelLockerShipment" DROP "ParcelLocker";
ALTER TABLE "ParcelLockerShipment" ADD "ParcelLockerId" INTEGER;