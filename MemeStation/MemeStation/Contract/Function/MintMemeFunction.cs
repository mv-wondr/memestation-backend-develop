using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace MemeStation.Contract.Function
{
  public partial class MintMemeFunction : MintMemeBaseFunction
  {
  }

  [Function("mintMeme")]
  public class MintMemeBaseFunction : FunctionContract
  {
    [Parameter("address", "receiver", 1)]
    public string To { get; set; }

    [Parameter("string", "memeType", 2)]
    public string MemeType { get; set; }

    [Parameter("string", "tokenIPFS", 3)]
    public string TokenIPFS { get; set; }
  }
}
