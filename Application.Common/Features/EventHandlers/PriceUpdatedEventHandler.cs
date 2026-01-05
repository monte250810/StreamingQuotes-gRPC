using Domain.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Features.EventHandlers
{
    internal sealed class PriceUpdatedEventHandler : INotificationHandler<PriceUpdatedEvent>
    {
        private readonly ILogger<PriceUpdatedEventHandler> _logger;

        public PriceUpdatedEventHandler(ILogger<PriceUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PriceUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug(
                "Price updated for {Ticker}: ${OldPrice:N2} -> ${NewPrice:N2}",
                notification.Ticker,
                notification.OldPrice,
                notification.NewPrice);

            return Task.CompletedTask;
        }
    }
}
