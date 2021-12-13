using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketEngine;
using MarketEngine.Models;
using MarketEngine.Models.Market;
using MarketEngine.Models.Orders;
using MarketEngineLib;
using MemeStation.Models.Trade;
using MongoDB.Driver;
using OrderMatcher;

namespace MemeStation.Core
{
    public interface INFTEngine
    {
        public Task<OrderResponse> Sell(string type, int price, int quantity, string sellerAddress);
        public Task<OrderResponse> Buy(string type, int price, int quantity, string sellerAddress);
        public OrderCancelResponse CancelOrder(string nftType, string orderId, string author);
        public IEnumerable<Order> GetCurrentTrades(FilterDefinition<Order> filterDefinition = null);
        public IEnumerable<TradeModel> GetTradingHistory(string author, string nftType, FilterDefinition<TradeModel> filterDefinition = null);
    }

    public class NFTEngine : INFTEngine
    {
        private readonly IOrderLib _iOrderLib;
        private readonly IMarketLib _iMarketLib;

        public NFTEngine(IOrderLib iOrderLib, IMarketLib iMarketLib)
        {
          _iMarketLib = iMarketLib;
          _iOrderLib = iOrderLib;
        }


        private async Task<OrderResponse> CreateOrder(string type, int price, int quantity, string sellerAddress, bool isBuy)
        {
            if (!_iMarketLib.IsMarketExist(type.ToLower(), ""))
            {
                var id = await _iMarketLib.CreateMarket(type, "",  new Market() {Hash = type.ToLower()});
            }

            return _iOrderLib.AddOrder(type.ToLower(), price, quantity, isBuy, sellerAddress);
        }

        public async Task<OrderResponse> Sell(string type, int price, int quantity, string sellerAddress)
        {
            return await CreateOrder(type.ToLower(), price, quantity, sellerAddress, false);
        }

        public async Task<OrderResponse> Buy(string type, int price, int quantity, string sellerAddress)
        {
            return await CreateOrder(type.ToLower(), price, quantity, sellerAddress, true);
        }

        public OrderCancelResponse CancelOrder(string nftType, string orderId, string author)
        {
            return _iOrderLib.CancelOrder(nftType.ToLower(), orderId, author);
        }

        public IEnumerable<Order> GetCurrentTrades(FilterDefinition<Order> filterDefinition = null)
        {
            return _iOrderLib.GetOrders(filterDefinition);
        }

        public IEnumerable<TradeModel> GetTradingHistory(string author, string nftType, FilterDefinition<TradeModel> filterDefinition = null)
        {
          var lower = nftType.ToLower();
            return _iOrderLib.GetHistory().Select(x =>
            {
                var order = GetCurrentTrades().First(i => i.OrderId.Equals(x.RestingOrderId));
                if (author != null && order.Author.Equals(author) && x.MarketId.Equals(lower))
                {
                    return x;
                }

                if (lower != null && lower.Equals(order.MarketId))
                {
                    return x;
                }

                return null;
            }).Where(x=>x != null);
        }

        public decimal GetLatestPrice(string nfttype, decimal lastPrice)
        {
            var activeSellOrders = GetCurrentTrades(Builders<Order>.Filter.Where(x => x.MarketId == nfttype.ToLower())).Where(x => x.MarketId == nfttype.ToLower() && !x.IsFilled && !x.IsBuy).Distinct().ToList();

            if (activeSellOrders.Any())
            {
              return activeSellOrders.OrderByDescending(o => o.Timestamp).First().Price;
            }

            return lastPrice;
        }
    }
}
