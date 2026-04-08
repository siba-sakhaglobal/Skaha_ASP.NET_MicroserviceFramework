namespace InventoryService.Domain.Exceptions;

public class InventoryDomainException : Exception
{
    public string Code { get; }

    public InventoryDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}
