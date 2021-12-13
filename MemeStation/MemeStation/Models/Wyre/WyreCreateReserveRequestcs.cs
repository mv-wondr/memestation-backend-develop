using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeStation.Models.Wyre
{
    public class WyreCreateReserveRequestcs
    {
        public double amount { get; set; }
        public string destCurrency { get; set; }
        public string sourceCurrency { get; set; }
        public string dest { get; set; }
        public string country { get; set; }
        public string redirectUrl { get; set; }
        public string paymentMethod { get; set; }
        public string referrerAccountId { get; set; }
        public string[] lockFields { get; set; }

    }
}
