using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webCrawler_1
{
    class FlightData
    {
        public string outbound_departure_airport { set; get; }
        public string outbound_arrival_airport { set; get; }
        public string outbound_departure_time { set; get; }
        public string outbound_arrival_time { set; get; }
        public string inbound_departure_airport { set; get; }
        public string inbound_arrival_airport { set; get; }
        public string inbound_departure_time { set; get; }
        public string inbound_arrival_time { set; get; }
        public string depart_price { set; get; }
        public string arrive_price { set; get; }
        public string total_price { set; get; }
        public double taxes { set; get; }
    }
}
