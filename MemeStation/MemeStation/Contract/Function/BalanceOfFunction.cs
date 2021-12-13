using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class BalanceOfFunction : BalanceOfBaseFunction
  {
  }

  [Function("balanceOf", "uint256")]
  public class BalanceOfBaseFunction : FunctionContract
  {
    [Parameter("address", "owner", 1)]
    public string Owner { get; set; }
  }
}
