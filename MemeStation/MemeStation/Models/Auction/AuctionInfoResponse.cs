using OrderMatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeStation.Models.Auction
{
    public class AuctionInfoResponse
    {
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public List<Order> LatestTrades { get; set; }
        public List<Order> UserTrades { get; set; }
    }
}
