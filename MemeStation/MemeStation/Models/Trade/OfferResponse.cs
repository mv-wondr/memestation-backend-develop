using MemeStation.ResponseBuilder;
using OrderMatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeStation.Models.Trade
{
    public class OfferResponse
    {
        public Order OrderDetails { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerPictureUrl { get; set; }
        public string NftId { get; set; }
        public string NftUrl { get; set; }


        public OfferResponse(Order orderDetails, UserResponse author, string nftUrl)
        {
            OrderDetails = orderDetails;
            OwnerName = author.UserName;
            OwnerAddress = author.Address;
            OwnerPictureUrl = author.Picture;
            NftId = orderDetails.MarketId;
            NftUrl = nftUrl;
        }
    }
}
