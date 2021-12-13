using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MemeStation.Contract.Function
{
    public partial class TotalSupplyFunction:TotalSupplyBaseFunction
    {
    }
    [Function("totalSupply", "uint256")]
    public class TotalSupplyBaseFunction:FunctionContract
    {
    }
}
