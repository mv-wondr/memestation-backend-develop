using MemeStation.Database.Enums;

namespace MemeStation.Models.Meme
{
  public class UpdateMemeRequest
  {

    public string ContractAddress { get; set; }

    public string TokenId { get; set; }

    public string Title { get; set; }

    public string Subtitle { get; set; }
    public string Description { get; set; }

    public string FileUrl { get; set; }

    public FilteType? Type { get; set; }

    public decimal? Price { get; set; }

    public string Origin { get; set; }

    public int? MaxInstance { get; set; }
  }
}
