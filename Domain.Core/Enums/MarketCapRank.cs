using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.Enums;

public enum MarketCapRank
{
    Unknown = 0,
    Top10 = 1,
    Top50 = 2,
    Top100 = 3,
    Top500 = 4,
    Top1000 = 5,
    Other = 6
}