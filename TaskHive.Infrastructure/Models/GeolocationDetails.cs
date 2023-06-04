using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Entities.Enums;

namespace TaskHive.Infrastructure.Models
{
    public class GeolocationDetails
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public GeolocationStatus Status { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("regionName")]
        public string RegionName { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("isp")]
        public string Isp { get; set; }

        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("as")]
        public string As { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
