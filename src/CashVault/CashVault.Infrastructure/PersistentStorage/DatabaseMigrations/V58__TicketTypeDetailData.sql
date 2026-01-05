ALTER TABLE "Transaction"
ADD CONSTRAINT FK_TICKET_TRANSACTION_TICKET_TYPE_DETAIL
FOREIGN KEY ("TicketTypeDetailId") REFERENCES "TicketTypeDetail"("Id");


INSERT INTO "TicketTypeDetail" ("Id","Code","Description") VALUES (1,'cms','Cms ticket');
INSERT INTO "TicketTypeDetail" ("Id","Code","Description") VALUES (2,'local','Local ticket');