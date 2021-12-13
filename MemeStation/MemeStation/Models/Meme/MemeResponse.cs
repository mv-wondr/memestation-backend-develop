using System;
using MemeStation.Database.Enums;
using MemeStation.ResponseBuilder;

namespace MemeStation.Models.Meme
{
  public class MemeResponse
  {
    public string Id { get; set; }
    public UserResponse Creator { get; set; }
    public string ContractAddress { get; set; }
    public string TokenId { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public string FileUrl { get; set; }
    public FilteType TypeId { get; set; }
   public string TypeString { get
   {
    return Enum.GetName(typeof(FilteType), TypeId).ToUpper();
   }}
    public decimal Price { get; set; }
    public DateTime CreationTime { get; set; }
    public string Origin { get; set; }
    public int MaxInstance { get; set; }
  }
}
