ALTER TABLE "EventLog" ADD "CreatedByUser" VARCHAR(2000);
ALTER TABLE "EventLog" ADD "CreatedByUserFullName" VARCHAR(2000);
ALTER TABLE "EventLog" ADD "CreatedByUserCompany" VARCHAR(2000);

ALTER TABLE "Operator" ADD "Company" VARCHAR(2000);