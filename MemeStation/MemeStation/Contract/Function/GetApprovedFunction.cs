using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class GetApprovedFunction : GetApprovedBaseFunction
  {
  }

  [Function("getApproved", "address")]
  public class GetApprovedBaseFunction : FunctionContract
  {
    [Parameter("uint256", "tokenId", 1)]
    public BigInteger TokenId { get; set; }
  }
}
