using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MemeStation.Contract.Function
{
  public partial class CreateMemeTypeFunction : CreateMemeTypeBaseFunction
  {
  }

  [Function("createMemeType")]
  public class CreateMemeTypeBaseFunction : FunctionContract
  {
    [Parameter("string", "memeType", 1)]
    public string MemeType { get; set; }

    [Parameter("uint256", "maxCount", 2)]
    public BigInteger MaxCount { get; set; }
  }
}
