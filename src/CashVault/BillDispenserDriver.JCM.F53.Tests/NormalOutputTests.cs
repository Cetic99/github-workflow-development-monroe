using CashVault.BillDispenserDriver.JCM.F53.Config;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate;
using NUnit.Framework.Internal;
using BillDispenserDriverClass = CashVault.BillDispenserDriver.JCM.F53.BillDispenserDriver;


namespace BillDispenser.JCM.F53.Tests
{

    public class NormalOutputTests : BaseSetup
    {
        private BillDispenserDriverClass billDispenserDriver;

        private BillDispenserDriverClass _billDispenserDriver;

        [SetUp]
        public void Setup(IServiceProvider serviceProvider)
        {
            var dispenserConfig = new BillDispenserJcm53Configuration();
            var port = new Port(PortType.Serial, "COM3");

            _billDispenserDriver = new BillDispenserDriverClass(port, dispenserConfig, serviceProvider);
            var task = _billDispenserDriver.InitializeAsync();
            task.Wait();
        }

        [Test]
        public void EmptyCassettesToRejectTray()
        {
            var transaction = new DispenserBillTransaction(100, 0, "bbbb");

            transaction.AddItem(new DispenserBillTransactionItem(1, 100, 1));
            transaction.AddItem(new DispenserBillTransactionItem(2, 50, 1));

            Thread.Sleep(1000);

            _billDispenserDriver.RejectCash(transaction).Wait();

            Assert.AreEqual(transaction.Items[0].BillCountRequested, transaction.Items[0].BillCountDispensed);
        }

        [Test]
        public void DeviceStatusInformation()
        {
            var deviceStatus = _billDispenserDriver.DeviceStatus();
        }

        //During the test execution, turn off the power while bill dispensing is in progress
        [Test]
        public void PowerLossSimulation()
        {
            var transaction = new DispenserBillTransaction(100, 0, "Dispense", "bbbb");

            transaction.AddItem(new DispenserBillTransactionItem(1, 100, 5));

            Thread.Sleep(1000);

            _billDispenserDriver.DispenseCashAsync(transaction).Wait();

            Assert.AreEqual(transaction.Items[0].BillCountRequested, transaction.Items[0].BillCountDispensed);
        }

        [Test]
        public void DeviceRecoveryAfterPowerLoss()
        {
            Thread.Sleep(2000);
        }


        [Test]
        public void BillCount()
        {
            var transaction = new DispenserBillTransaction(100, 0, "bbbb");

            transaction.AddItem(new DispenserBillTransactionItem(1, 100, 1));
            transaction.AddItem(new DispenserBillTransactionItem(2, 50, 1));


            Thread.Sleep(1000);

            _billDispenserDriver.DispenseCashAsync(transaction).Wait();

            Assert.AreEqual(transaction.Items[0].BillCountRequested, transaction.Items[0].BillCountDispensed);
        }

        [Test]
        public void PeriodicallyStatusCheck()
        {
            var tmpTask = Task.Delay(60 * 1000); // 60 sec
            tmpTask.Wait();

            var transaction = new DispenserBillTransaction(100, 0, "bbbb");

            transaction.AddItem(new DispenserBillTransactionItem(1, 100, 1));
            transaction.AddItem(new DispenserBillTransactionItem(2, 50, 2));

            Thread.Sleep(1000);

            _billDispenserDriver.DispenseCashAsync(transaction).Wait();

            Assert.AreEqual(transaction.Items[0].BillCountRequested, transaction.Items[0].BillCountDispensed);

            var tmpTask1 = Task.Delay(5 * 1000); // 60 sec
            tmpTask1.Wait();


        }

        [Test]

        public void EnableDisableReset()
        {
            /*
                Enable
                Disable
                Enable
                Disable
                Reset
                Disable
                Disable
                Enable
                Enable
                Disable
                Enable
             */
            //Enable
            _billDispenserDriver.EnableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Disable
            _billDispenserDriver.DisableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Enable
            _billDispenserDriver.EnableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Disable
            _billDispenserDriver.DisableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Reset
            _billDispenserDriver.ResetAsync();
            Thread.Sleep(2000);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Disable
            _billDispenserDriver.DisableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Disable
            _billDispenserDriver.DisableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Enable
            _billDispenserDriver.EnableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Enable
            _billDispenserDriver.EnableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Enable
            _billDispenserDriver.EnableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Disable
            _billDispenserDriver.DisableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

            //Enable
            _billDispenserDriver.EnableAsync();
            Thread.Sleep(500);
            _billDispenserDriver.GetCurrentStatus();
            Thread.Sleep(1000);

        }


        [TearDown]
        public void TearDown()
        {
            _billDispenserDriver.Dispose();
        }
    }
}
