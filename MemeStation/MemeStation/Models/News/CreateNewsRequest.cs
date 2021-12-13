using System.ComponentModel.DataAnnotations;

namespace MemeStation.Models.News
{
  public class CreateNewsRequest
  {
    [Required]
    public string MemeId { get; set; }
    [Required]
    public string Title { get; set; }
    public string SubTitle { get; set; }

    [Required]
    public string Url { get; set; }
  }
}
