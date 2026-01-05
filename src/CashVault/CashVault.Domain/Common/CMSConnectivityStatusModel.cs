namespace CashVault.Domain.Common
{
    public class CMSConnectivityStatusModel
    {
        public bool IsConnected { get; set; }
        public bool IsCasinoManagementSystem { get; set; }

        public CMSConnectivityStatusModel(bool isConnected, bool isCasinoManagementSystem)
        {
            IsConnected = isConnected;
            IsCasinoManagementSystem = isCasinoManagementSystem;
        }
    }
}
