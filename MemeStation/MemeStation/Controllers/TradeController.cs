using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketEngine;
using MarketEngine.Models;
using MarketEngine.Models.Market;
using MarketEngineLib;
using MemeStation.Core;
using MemeStation.Database;
using MemeStation.Models.Trade;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OrderMatcher;
using Org.BouncyCastle.Crypto.Tls;
using SQLitePCL;
using CancelRequest = MemeStation.Models.Trade.CancelRequest;

namespace MemeStation.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class TradeController : ControllerBase
  {
    private readonly INFTEngine _nftEngine;
    private readonly DatabaseContext _databaseContext;
    private readonly IDatabaseHelper _databaseHelper;

    public TradeController(INFTEngine nftEngine, DatabaseContext databaseContext, IDatabaseHelper databaseHelper)
    {
      _nftEngine = nftEngine;
      _databaseContext = databaseContext;
      _databaseHelper = databaseHelper;
    }

    /// <summary>
    /// Create a sell option.
    /// </summary>
    /// <returns></returns>
    [HttpPost("sell")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Sell([FromBody] SellRequest req)
    {
      return Ok(_nftEngine.Sell(req.NFTType, req.Price, req.Quantity, req.SellerAddress));
    }

    /// <summary>
    /// Create a buy option.
    /// </summary>
    /// <returns></returns>
    [HttpPost("buy")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Buy([FromBody] BuyRequest req)
    {
      return Ok(_nftEngine.Buy(req.NFTType, req.Price, req.Quantity, req.BuyerAddress));
    }

    /// <summary>
    /// cancel a buy or a sell
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel([FromBody] CancelRequest req)
    {
      return Ok(_nftEngine.CancelOrder(req.NFTType, req.OrderId, req.SellerAddress));
    }

    /// <summary>
    /// GetOrders based on a filter, by NFTtype or By author.
    /// </summary>
    /// <param name="author"></param>
    /// <param name="NFTType"></param>
    /// <returns></returns>
    [HttpGet("order")]
    public async Task<IActionResult> GetOrders([FromQuery] string author, [FromQuery] string NFTType)
    {
      var matchedOrders = new List<Order>();
      var response = new List<OfferResponse>();

      //Retool test endpoint for transaction reconcile.
      if (author == "all")
      {
        matchedOrders = _nftEngine.GetCurrentTrades(Builders<Order>.Filter.Where(x => x.IsFilled == true)).ToList();
      }

      if (author != null && NFTType != null)
      {
        matchedOrders = _nftEngine.GetCurrentTrades(Builders<Order>.Filter.Where(x => x.Author.Equals(author) && x.MarketId.Equals(NFTType))).ToList();
      }

      if (NFTType != null)
      {
        var lower = NFTType.ToLower();
        matchedOrders = _nftEngine.GetCurrentTrades(Builders<Order>.Filter.Where(x => x.MarketId == lower)).Where(x => x.MarketId == lower).ToList();
      }

      if (author != null && author != "all")
      {
        matchedOrders = _nftEngine.GetCurrentTrades(Builders<Order>.Filter.Where(x => x.Author == author)).ToList();
      }

      if (author == null && NFTType == null)
      {
        matchedOrders = _nftEngine.GetCurrentTrades().ToList();
      }

      var users = _databaseHelper.GetAllUsers().ToList();
      var memes = _databaseHelper.GetAllMemes().ToList();

      //matchedOrders.RemoveAll(o => o.Author == "MINTING");

      foreach (var order in matchedOrders)
      {
        try
        {
          //the second parameter here show the seller (author) and not the Buyer for the owner.
          var matchedUser = users.FirstOrDefault(u => u.Address == order.Author);
          var matchedMeme = memes.FirstOrDefault(m => m.Id == order.MarketId);
          if (matchedMeme == null || matchedUser == null)
          {
            //TODO: if the order have no matchedUser or no matchedMeme, should the system delete it ?
            //TODO: this project should have a logger.

            Console.WriteLine("[WARN]: No matcheMeme or no matchedUser for this order-\n" + order);
            continue;
          }

          response.Add(new OfferResponse(order, matchedUser, matchedMeme.FileUrl));
        }
        catch (System.Exception e)
        {
          //Somestimes Database state is broken and an order doesn't match any memes or Author
          Console.WriteLine(e);
          return StatusCode(500);
        }
      }

      response.OrderByDescending(o => o.OrderDetails.Timestamp).ToList();

      return Ok(response.Distinct().ToList());
    }

    /// <summary>
    /// GetNftPriceById and NFT ID
    /// </summary>
    /// <param name="NFTId"></param>
    /// <returns></returns>
    [HttpGet("price")]
    public async Task<IActionResult> GetNftPriceById([FromQuery] string NFTId)
    {
      var lower = NFTId.ToLower();
      var matchedOrders = _nftEngine.GetCurrentTrades(Builders<Order>.Filter.Where(x => x.MarketId == lower)).Distinct().Where(x => x.MarketId == lower).OrderByDescending(o => o.Timestamp);

      if (matchedOrders.Any())
      {
        return Ok(new {Price = matchedOrders.First().Price.ToString()});
      }

      return Ok(new {Price = 0});
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] string author, [FromQuery] string NFTType)
    {
      return Ok(_nftEngine.GetTradingHistory(author, NFTType));
    }
  }
}
