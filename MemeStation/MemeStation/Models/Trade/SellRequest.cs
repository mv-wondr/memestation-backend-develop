using System.ComponentModel.DataAnnotations;

namespace MemeStation.Models.Trade
{
  public class SellRequest
  {
    [Required]
    public string SellerAddress { get; set; }

    [Required]
    public string NFTType { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public int Price { get; set; }
  }
}
