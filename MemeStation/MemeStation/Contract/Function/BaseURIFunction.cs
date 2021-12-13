using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class BaseURIFunction : BaseURIBaseFunction
  {
  }

  [Function("baseURI", "string")]
  public class BaseURIBaseFunction : FunctionContract
  {
  }
}
