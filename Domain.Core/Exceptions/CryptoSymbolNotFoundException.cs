namespace Domain.Core.Exceptions;
public sealed class CryptoSymbolNotFoundException : DomainException
{
    public CryptoSymbolNotFoundException(string symbolId)
        : base("CRYPTO_SYMBOL_NOT_FOUND", $"Crypto symbol with ID '{symbolId}' was not found")
    {
    }
}