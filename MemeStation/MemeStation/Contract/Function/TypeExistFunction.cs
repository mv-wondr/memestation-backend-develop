using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class TypeExistFunction : TypeExistBaseFunction{}
    [Function("typeExist", "bool")]
    public class TypeExistBaseFunction : FunctionContract
    {
      [Parameter("string", "memeType", 1)]
      public string MemeType { get; set; }
    }
}
