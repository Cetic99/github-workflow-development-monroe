namespace CashVault.ccTalk.BillAcceptorBase;

public enum BillAcceptorDeviceStatus
{
    ReadyForAccepting,
    Initializing,
    Disabled,
    Jammed,
    StackerFull,
    Inhibiting,
    Disconected,
    // Reconnecting,
    Error,
    FatalError,
    Unknown
}