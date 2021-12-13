using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MemeStation.Database;
using MemeStation.Models.Auction;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MemeStation.Core
{
  public interface IAuction
  {
    public Task Last10Bids(HttpContext context);
    public Task AllInfo(HttpContext context);
  }

  public class Auction : IAuction
  {
    private INFTEngine _nftEngine;
    private IDatabaseHelper _databaseHelper;

    public Auction(INFTEngine nftEngine, IDatabaseHelper databaseHelper)
    {
      _nftEngine = nftEngine;
      _databaseHelper = databaseHelper;
    }

    public async Task Last10Bids(HttpContext context)
    {
      using WebSocket webSocket = await
        context.WebSockets.AcceptWebSocketAsync();

      var buffer = new byte[1024 * 4];
      WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      var nftId = System.Text.Encoding.Default.GetString(buffer).TrimEnd('\0');

      var mostRecentTradeId = "";
      while (!result.CloseStatus.HasValue)
      {
        var Last10TradesSorted = _nftEngine.GetCurrentTrades().Where(t => t.MarketId == nftId).OrderByDescending(t => t.Timestamp).Take(10);
        if (Last10TradesSorted.Any())
        {
          var lastMessageId = Last10TradesSorted.ToList()[0].OrderId;
          if (mostRecentTradeId != lastMessageId)
          {
            var tradeBuffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Last10TradesSorted));

            await webSocket.SendAsync(new ArraySegment<byte>(tradeBuffer, 0, tradeBuffer.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
            mostRecentTradeId = lastMessageId;
          }
        }

        await Task.Delay(5000);
      }

      await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    public async Task AllInfo(HttpContext context)
    {
      using WebSocket webSocket = await
        context.WebSockets.AcceptWebSocketAsync();

      var buffer = new byte[1024 * 4];
      WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      var s = System.Text.Encoding.Default.GetString(buffer).TrimEnd('\0');
      var req = JsonConvert.DeserializeObject<AuctionInfoRequest>(s);

      decimal mostRecentHigestBid = 0;
      var mostRecentTradeId = "";
      while (!result.CloseStatus.HasValue)
      {
        var allTrades = _nftEngine.GetCurrentTrades().Where(t => t.MarketId == req.NFTType && !t.IsFilled);
        var highestBid = allTrades.OrderByDescending(t => t.Price).FirstOrDefault();
        var Last10TradesSorted = allTrades.OrderByDescending(t => t.Timestamp).Take(10);
        var userTrades = allTrades.OrderByDescending(t => t.Price).Where(t => t.Author == req.LoggedUser).ToList();

        if (highestBid != null && Last10TradesSorted.Any())
        {
          var currentHighestBid = highestBid.Price;
          var lastMessageId = Last10TradesSorted.ToList()[0].OrderId;
          if (mostRecentHigestBid < currentHighestBid || mostRecentTradeId != lastMessageId)
          {
            var tradeBuffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new AuctionInfoResponse()
              {EndTime = GetEndTime(req.NFTType), Price = highestBid.Price, LatestTrades = Last10TradesSorted.ToList(), UserTrades = userTrades}));
            await webSocket.SendAsync(new ArraySegment<byte>(tradeBuffer, 0, tradeBuffer.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
            mostRecentHigestBid = currentHighestBid;
            mostRecentTradeId = lastMessageId;
          }
        }
        else
        {
          var tradeBuffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new AuctionInfoResponse()
            {EndTime = GetEndTime(req.NFTType), Price = 1, LatestTrades = new List<OrderMatcher.Order>(), UserTrades = new List<OrderMatcher.Order>()}));
          await webSocket.SendAsync(new ArraySegment<byte>(tradeBuffer, 0, tradeBuffer.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
        }

        await Task.Delay(5000);
      }

      await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    private DateTime GetEndTime(string nftId)
    {
      var e = _databaseHelper.GetAllMemes(x => x.Where(m => m.Id.Equals(nftId)));
      if (!e.Any())
      {
        return DateTime.Now;
      }

      return DateTimeOffset.FromUnixTimeSeconds((long.Parse(e.First().Subtitle))).DateTime;
    }
  }
}
