using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MemeStation.Models.Meme
{
  public class CreateMemeTypeRequest
  {
    public string AlphaKey { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public BigInteger MaxCount { get; set; }
  }
}
