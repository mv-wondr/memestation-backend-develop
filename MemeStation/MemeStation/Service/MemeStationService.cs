using System;
using System.Numerics;
using System.Threading.Tasks;
using MemeStation.Contract.Function;
using MemeStation.Database;
using MemeStation.Models.Meme;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using Nethereum.Web3;

namespace MemeStation.Service
{
  public interface IMemeStationService
  {
    public Task<bool> TypeExist(string type);
    public Task<string> Symbol();
    public Task<bool> IsLimitReached(string type);
    public Task<Meme> CreateMeme(CreateMemeRequest req);
    public Task CreateMemeType(CreateMemeTypeRequest req);
  }

  public class MemeStationService : IMemeStationService
  {
    private readonly IContractService _contractService;
    private readonly DatabaseContext _databaseContext;

    public MemeStationService(IContractService contractService, DatabaseContext databaseContext)
    {
      _contractService = contractService;
      _databaseContext = databaseContext;
    }

    public async Task<bool> TypeExist(string type)
    {
      return await _contractService.Get<TypeExistFunction, bool>(new TypeExistFunction() {MemeType = type});
    }

    public async Task<string> Symbol()
    {
      return await _contractService.Get<SymbolFunction, string>(new SymbolFunction());
    }

    public async Task<bool> IsLimitReached(string type)
    {
      return await _contractService.Get<IsLimitReachedFunction, bool>(new IsLimitReachedFunction() {MemeType = type});
    }

    public async Task<Meme> CreateMeme(CreateMemeRequest req)
    {
      await HandleMemeCreationError(req.MemeType);

      var tokenId = await _contractService.Get<TotalSupplyFunction, BigInteger>(new TotalSupplyFunction());
      var a = await _contractService.Post(new MintMemeFunction()
      {
        MemeType = req.MemeType,
        TokenIPFS = req.IPFSHash,
        To = req.CreatorAddress
      });
      var url = await _contractService.Get<TokenURIFunction, string>(new TokenURIFunction()
      {
        TokenId = tokenId
      });

      var meme = new Meme()
      {
        Id = ObjectId.GenerateNewId().ToString(),
        CreatorAddress = req.CreatorAddress,
        MemeType = req.MemeType,
        TokenId = tokenId.ToString(),
        Title = req.Title,
        Subtitle = req.Subtitle,
        Description = req.Description,
        FileJsonUrl = url,
        Type = req.Type,
        CreationTime = DateTime.Now,
        Price = req.Price,
        Origin = req.Origin,
        //TODO:this should be fetched from the chain.
        MaxInstance = req.MaxInstance
      };

      await _databaseContext.Memes.AddAsync(meme);
      await _databaseContext.SaveChangesAsync();

      return meme;
    }

    public async Task CreateMemeType(CreateMemeTypeRequest req)
    {
      if (await TypeExist(req.Type))
      {
        throw new Exception("This type is already created");
      }

      var txReceipt = await _contractService.Post(new CreateMemeTypeFunction()
      {
        MemeType = req.Type,
        MaxCount = req.MaxCount
      });

      if (txReceipt.HasErrors() == true)
      {
        throw new Exception("The transaction could not be sent to the chain");
      }
    }

    private async Task HandleMemeCreationError(string type)
    {
      if (!(await TypeExist(type)))
      {
        throw new Exception("The type you are trying to create doesn't exist");
      }

      if (await IsLimitReached(type))
      {
        throw new Exception("Limit is reached for this type");
      }
    }
  }
}
