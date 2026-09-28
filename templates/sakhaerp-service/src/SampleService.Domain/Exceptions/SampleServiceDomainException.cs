namespace SampleService.Domain.Exceptions;

public class SampleServiceDomainException : Exception
{
    public string Code { get; }

    public SampleServiceDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}
