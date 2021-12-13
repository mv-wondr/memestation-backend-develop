using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MemeStation.Contract.Function
{
  public partial class TokenURIFunction : TokenURIBaseFunction
  {
  }

  [Function("tokenURI", "string")]
  public class TokenURIBaseFunction : FunctionContract
  {
    [Parameter("uint256", "tokenId", 1)]
    public BigInteger TokenId { get; set; }
  }
}
