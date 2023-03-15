using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class AllowedCountriesOnPostilionResponse : BaseResponse
    {
        public AllowedCountryOnPostillion[] AllowedCountries;
    }

    [JsonObject]
    public class AllowedCountryOnPostillion
    {
        public string CountryName;

        public string ISO2Code;

        public string AlphaCode;

        public string CurrencyCode;

        public string ISO3Code;
    }
}
