using System;
using System.Linq;
using System.Threading.Tasks;
using MarketEngine.Models;
using MarketEngine.Service;
using MemeStation.Database;
using OrderMatcher;

namespace MemeStation.Core
{
    public class NFTTradeListener : ITradeListener
    {
        private readonly IMongoHelper _mongoHelper;
        private readonly DatabaseContext _databaseContext;

        public NFTTradeListener(IMongoHelper mongoHelper, DatabaseContext databaseContext)
        {
            _mongoHelper = mongoHelper;
            _databaseContext = databaseContext;
        }

        public void OnAccept(string orderId)
        {
        }

        public async void OnTrade(Order incomingOrder, Order restingOrder, Price matchPrice, Quantity matchQuantiy,
            Quantity? askRemainingQuantity, Quantity? askFee, Quantity? bidCost, Quantity? bidFee)
        {
            // TODO: Find a way to change to LOGGER instead of console
            Console.WriteLine("****************************************************************************************************************************");
            Console.WriteLine(
                $"Order matched: IncomingOrderID : {incomingOrder.OrderId}, RestingOrderID : {restingOrder.OrderId}," +
                $" MatchQuantity : {matchQuantiy}, Price : {matchPrice}");

            // TODO: Implement TradeModel logic to execute matched trades
            var sell = !incomingOrder.IsBuy ? incomingOrder : restingOrder;
            var buy = incomingOrder.IsBuy ? incomingOrder : restingOrder;

            var transferOwnerShip = new TransferOwnership()
            {
                SenderId = sell.Author,
                RecipientId = buy.Author,
                MemeId = sell.MarketId,
                Active = true
            };
            var activeTrades = _databaseContext.TransferOwnerships.Where(x => x.MemeId.Equals(sell.MarketId) && x.Active);
            var tradedNFTMaxInstances = _databaseContext.Memes.Where(m => m.Id == sell.MarketId).First().MaxInstance;

            if(activeTrades.Count() >= tradedNFTMaxInstances)
            {
                return;
            }

            foreach (var activeTrade in activeTrades)
            {
                if (activeTrade.SenderId != "MINTING")
                {
                    activeTrade.Active = false;
                }
            }
            
            await _databaseContext.AddAsync(transferOwnerShip);
            await _databaseContext.SaveChangesAsync();

            _mongoHelper.TryInsertTradeModel(new TradeModel(incomingOrder.OrderId, restingOrder.OrderId,
                (int) matchPrice, (int) matchQuantiy, buy.Author, sell.Author, sell.MarketId));
        }

        public void OnCancel(string orderId, Quantity remainingQuantity, Quantity cost, Quantity fee,
            CancelReason cancelReason)
        {
        }

        public void OnOrderTriggered(string orderId)
        {
        }
    }
}