using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class ApproveFunction : ApproveBaseFunction
  {
  }

  [Function("approve")]
  public class ApproveBaseFunction : FunctionContract
  {
    [Parameter("address", "to", 1)]
    public string To { get; set; }

    [Parameter("uint256", "tokenId", 2)]
    public BigInteger TokenId { get; set; }
  }
}
