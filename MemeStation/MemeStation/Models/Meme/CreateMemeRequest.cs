using System.ComponentModel.DataAnnotations;
using MemeStation.Database.Enums;

namespace MemeStation.Models.Meme
{
  public class CreateMemeRequest
  {
    [Required]
    public string IPFSHash { get; set; }
    [Required]
    public string MemeType { get; set; }
    public string AlphaSecret { get; set; }

    [Required]
    public string CreatorAddress { get; set; }

   // [Required]
   // public string ContractAddress { get; set; }

    //[Required]
    //public string TokenId { get; set; }

    [Required]
    public string Title { get; set; }

    public string Subtitle { get; set; }
    public string Description { get; set; }

    //[Required]
    //public string FileUrl { get; set; }

    [Required]
    public FilteType Type { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Origin { get; set; }

    [Required]
    public int MaxInstance { get; set; }
  }
}
