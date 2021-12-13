using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class IsLimitReachedFunction : IsLimitReachedBaseFunction {}
  [Function("isLimitReached", "bool")]
  public class IsLimitReachedBaseFunction : FunctionContract
  {
    [Parameter("string", "memeType")]
    public string MemeType { get; set; }
  }
}
