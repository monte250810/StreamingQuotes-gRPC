using Domain.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Domain.Core.Exceptions;
public sealed class CryptoSymbolNotFoundException : DomainException
{
    public CryptoSymbolNotFoundException(string symbolId)
        : base("CRYPTO_SYMBOL_NOT_FOUND", $"Crypto symbol with ID '{symbolId}' was not found")
    {
    }
}