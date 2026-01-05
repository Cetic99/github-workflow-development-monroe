namespace CashVault.Domain.Common;

public class OperationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public OperationResult()
    {
        IsSuccess = false;
    }

    public OperationResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
}
