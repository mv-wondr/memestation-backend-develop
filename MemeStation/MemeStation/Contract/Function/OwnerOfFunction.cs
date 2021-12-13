using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class OwnerOfFunction : OwnerOfBaseFunction
  {
  }

  [Function("ownerOf", "address")]
  public class OwnerOfBaseFunction : FunctionContract
  {
    [Parameter("uint256", "tokenId", 1)]
    public BigInteger tokenId { get; set; }
  }
}
