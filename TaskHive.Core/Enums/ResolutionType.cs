using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskHive.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResolutionType
    {
        Completed = 1,
        CannotReproduce,
        Duplicate,
        OnHold,
        Invalid,
        NoDefect,
        ExternalDefect,
        NoMaintenanceAgreement,
        OfferDeclined,
        OfferExpired,
        Rejected,
        Verified,
        Resolved,
        Done,
        Unresolved
    }
}
