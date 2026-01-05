using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.TicketPrinterDriver.FutureLogic;
using CashVault.TicketPrinterDriver.FutureLogic.Config;

namespace TicketPrinterTests
{
    public class Tests
    {
        private readonly IServiceProvider serviceProvider;
        private TicketPrinterDriver _ticketPrinterDriverClass;
        private TITOPrinterFutureLogicConfiguration ticketPrinterConfiguration = new();



        [SetUp]
        public void Setup()
        {
            ticketPrinterConfiguration.BaudRate = 38400;
            ticketPrinterConfiguration.HasTemplate0 = true;

            var port = new Port(PortType.Serial, "COM12");

            _ticketPrinterDriverClass = new TicketPrinterDriver(port, ticketPrinterConfiguration, serviceProvider);

            var task = _ticketPrinterDriverClass.InitializeAsync();
            task.Wait();
        }

        [Test]
        public void Test1_PrintCashoutTicket()
        {
            Ticket ticket = new Ticket(TicketType.MonroeCashConfirmation, "014702580369111123", true, "321", 54.5m, 7, CashVault.Domain.ValueObjects.Currency.BAM);

            _ticketPrinterDriverClass.PrintTicketAsync(ticket, "caption", "location demo", "location address", "001");
        }

        [Test]
        public void Test2_PrintCashoutTicket()
        {
            Ticket ticket = new Ticket(TicketType.MonroeCashConfirmation, "659123048250971312", true, "456", 145.78m, 7, CashVault.Domain.ValueObjects.Currency.BAM);

            //_ticketPrinterDriverClass.PrintTicketAsync(ticket);
        }

        [Test]
        public void Test3_PrintText()
        {
            string[] lines = { "text1 line1", "text1231231 line114324151", "textew1ersadf1 linasfdasfe1", "asfdasdftext1 line1asdfas" };

            _ticketPrinterDriverClass.PrintTextAsync(lines);
        }

        [Test]
        public void Test4_PeriodicalStatusCheckLoop()
        {
            var tmpTask = Task.Delay(60 * 1000); // 60 sec
            tmpTask.Wait();
        }

        [Test]
        public void Test5_DisableEnableResetScenario()
        {
            /*
                1. Enable
                2. Disable
                3. Enable
                4. Disable
                5. Reset
                6. Disable
                7. Disable
                8. Enable
                9. Enable
                10. Disable
                11. Enable
             */
            //Enable

            int cnt = 1;

            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.EnableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Disable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.DisableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Enable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.EnableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Disable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.DisableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Reset
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.ResetAsync();
            Thread.Sleep(2000);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Disable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.DisableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Disable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.DisableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Enable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.EnableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Enable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.EnableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Enable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.EnableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Disable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.DisableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

            //Enable
            Console.WriteLine((cnt++) + ". ");
            _ticketPrinterDriverClass.EnableAsync();
            Thread.Sleep(500);
            _ticketPrinterDriverClass.GetCurrentStatus();
            Thread.Sleep(1000);
            Console.WriteLine("---------");

        }

        [TearDown]
        public void TearDown()
        {
            _ticketPrinterDriverClass.Dispose();
        }
    }
}