namespace CashVault.BillAcceptorDriver.ID003;

internal enum AcceptorStatus
{
    Disabled,
    ReadyForAccepting,
    Accepting,
    Rejecting,
    StackerOpened,
    Stacked,
    Initializing,
    PowerUp,
    Error,
    Unknown
}
