using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.ValueObjects;
    public sealed record SymbolId
    {
        public string Value { get; }
        private SymbolId(string value) => Value = value;
        public static SymbolId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Symbol ID cannot be empty", nameof(value));

            return new SymbolId(value.ToLowerInvariant().Trim());
        }

        public static implicit operator string(SymbolId symbolId) => symbolId.Value;
        public override string ToString() => Value;
}
