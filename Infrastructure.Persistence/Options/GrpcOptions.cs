using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Options
{
    public sealed class GrpcOptions
    {
        public const string SectionName = "Grpc";

        public bool EnableDetailedErrors { get; init; } = false;

        [Range(1, 100)]
        public int MaxReceiveMessageSizeMb { get; init; } = 4;

        [Range(1, 100)]
        public int MaxSendMessageSizeMb { get; init; } = 4;

        public bool EnableMessageCompression { get; init; } = true;
    }
}
