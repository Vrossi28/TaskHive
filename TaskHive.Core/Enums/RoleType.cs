using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskHive.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoleType
    {
        Administrator = 1,
        ProjectManager,
        TeamMember,
        Observer,
        Customer
    }
}
