using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class SymbolFunction : SymbolBaseFunction
  {
  }

  [Function("symbol", "string")]
  public class SymbolBaseFunction : FunctionContract
  {
  }
}
