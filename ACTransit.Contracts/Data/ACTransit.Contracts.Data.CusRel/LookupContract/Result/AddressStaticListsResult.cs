using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class AddressStaticListsResult : Common.Result
    {
        public AddressStaticListsResult()
        {
            Cities = new List<string>
            {
                "Alameda",
                "Albany",
                "Alvarado",
                "Ashland",
                "Berkeley",
                "Castro Valley",
                "El Cerrito",
                "Emeryville",
                "El Sobrante",
                "Fairview",
                "Foster City",
                "Fremont",
                "Hayward",
                "Kensington",
                "Milpitas",
                "Newark",
                "Oakland",
                "Piedmont",
                "Richmond",
                "San Francisco",
                "San Leandro",
                "San Lorenzo",
                "San Pablo",
                "Union City",
            };

            States = new List<string>
            {
                "CA",
                "AK",
                "AL",
                "AR",
                "AS",
                "AZ",
                "CO",
                "CT",
                "DC",
                "DE",
                "FL",
                "FM",
                "GA",
                "GU",
                "HI",
                "IA",
                "ID",
                "IL",
                "IN",
                "KS",
                "KY",
                "LA",
                "MA",
                "MD",
                "ME",
                "MH",
                "MI",
                "MN",
                "MO",
                "MP",
                "MS",
                "MT",
                "NC",
                "ND",
                "NE",
                "NH",
                "NJ",
                "NM",
                "NV",
                "NY",
                "OH",
                "OK",
                "OR",
                "PA",
                "PR",
                "PW",
                "RI",
                "SC",
                "SD",
                "TN",
                "TX",
                "UT",
                "VA",
                "VI",
                "VT",
                "WA",
                "WI",
                "WV",
                "WY"
            };
        }

        [DataMember]
        public List<string> Cities { get; set; }

        [DataMember]
        public List<string> States { get; set; }
    }
}