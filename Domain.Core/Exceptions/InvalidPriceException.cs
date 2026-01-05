using Domain.Core.Exceptions;
namespace Domain.Core.Exceptions;
public sealed class InvalidPriceException : DomainException
{
    public InvalidPriceException(decimal price)
        : base("INVALID_PRICE", $"Price '{price}' is not valid. Price must be greater than or equal to zero.")
    {
    }
}