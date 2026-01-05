using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.Events
{
    public sealed record CryptoAssetCreatedEvent(
        string SymbolId,
        string Ticker,
        decimal InitialPrice) : DomainEvent;
}
