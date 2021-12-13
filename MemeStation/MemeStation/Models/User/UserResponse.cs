using System.Collections.Generic;
using MemeStation.Database;
using MemeStation.Models;
using MemeStation.Models.Meme;

namespace MemeStation.ResponseBuilder
{
  public class UserResponse
  {
    public string Address { get; set; }
    public string Auth0Id { get; set; }

    public string UserName { get; set; }
    public string Picture { get; set; }
    public string WalletDescription { get; set; }
    public bool Authenticated { get; set; }
    public ICollection<MemeResponse> OwnedMemes { get; set; }
    public ICollection<MemeResponse> CreatedMemes { get; set; }
  }
}
