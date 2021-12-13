using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class TokenByIndexFunction : TokenByIndexBaseFunction
  {
  }

  [Function("tokenByIndex", "uint256")]
  public class TokenByIndexBaseFunction : FunctionContract
  {
    [Parameter("uint256", "index", 1)]
    public BigInteger Index { get; set; }
  }
}
