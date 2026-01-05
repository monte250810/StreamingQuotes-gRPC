using Domain.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Features.EventHandlers
{
    internal sealed class SignificantPriceChangeEventHandler : INotificationHandler<SignificantPriceChangeEvent>
    {
        private readonly ILogger<SignificantPriceChangeEventHandler> _logger;

        public SignificantPriceChangeEventHandler(ILogger<SignificantPriceChangeEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(SignificantPriceChangeEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning(
                "🚨 Significant price change detected for {SymbolId}: {ChangePercentage:+0.00;-0.00}% (${OldPrice:N2} -> ${NewPrice:N2})",
                notification.SymbolId,
                notification.ChangePercentage,
                notification.OldPrice,
                notification.NewPrice);

            // Here you could trigger notifications, alerts, webhooks, etc.

            return Task.CompletedTask;
        }
    }
}
