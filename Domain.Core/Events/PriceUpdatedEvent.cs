using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.Events
{
    public sealed record PriceUpdatedEvent(
        string SymbolId,
        string Ticker,
        decimal OldPrice,
        decimal NewPrice) : DomainEvent;
}
