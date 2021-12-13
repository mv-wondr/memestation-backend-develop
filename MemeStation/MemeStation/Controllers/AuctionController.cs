using MemeStation.Core;
using MemeStation.Database;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
  public class AuctionController : ControllerBase
  {
    private readonly INFTEngine _nftEngine;
    private readonly IDatabaseHelper _databaseHelper;
    private readonly IAuction _auction;

    public AuctionController(INFTEngine nftEngine, IDatabaseHelper databaseHelper, IAuction auction)
    {
      _nftEngine = nftEngine;
      _databaseHelper = databaseHelper;
      _auction = auction;
    }

    [HttpGet("/auction/last")]
    public async Task Last10()
    {
      if (HttpContext.WebSockets.IsWebSocketRequest)
      {
        await _auction.Last10Bids(HttpContext);
      }
      else
      {
        HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
      }
    }

    [HttpGet("/auction/all")]
    public async Task AllData()
    {
      if (HttpContext.WebSockets.IsWebSocketRequest)
      {
        await _auction.AllInfo(HttpContext);
      }
      else
      {
        HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
      }
    }
  }
}
