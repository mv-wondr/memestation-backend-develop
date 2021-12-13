using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class TokenOfOwnerByIndexFunction : TokenOfOwnerByIndexBaseFunction
  {
  }

  [Function("tokenOfOwnerByIndex", "uint256")]
  public class TokenOfOwnerByIndexBaseFunction : FunctionContract
  {
    [Parameter("address", "owner", 1)]
    public string Owner { get; set; }

    [Parameter("uint256", "index", 2)]
    public BigInteger Index { get; set; }
  }
}
