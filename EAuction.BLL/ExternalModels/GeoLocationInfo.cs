using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAuction.BLL.ExternalModels
{
    public class GeoLocationInfo
    {
        public string ip { get; set; }
        public string country_name { get; set; }
        public string city { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}