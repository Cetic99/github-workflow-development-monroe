using System;

namespace CashVault.Domain.Common;

[AttributeUsage(AttributeTargets.Method)]
public class CanBeCalledFromRemoteServer : Attribute { }
