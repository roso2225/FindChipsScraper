using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindChipsScraper.Models
{
    public class Offer
    {
        public string Distributor { get; set; }
        public string Seller { get; set; }
        public string MOQ { get; set; }
        public string SPQ { get; set; }
        public string UnitPrice { get; set; }
        public string Currency { get; set; }
        public string OfferUrl { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
