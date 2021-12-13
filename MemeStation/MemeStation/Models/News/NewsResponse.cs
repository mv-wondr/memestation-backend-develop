namespace MemeStation.Models.News
{
  public class NewsResponse
  {
    public string NewsId { get; set; }
    public string MemeId { get; set; }
    public Database.Meme Meme { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Url { get; set; }

//        public ICollection<MemeReaction> Reactions { get; set; }
  }
}
