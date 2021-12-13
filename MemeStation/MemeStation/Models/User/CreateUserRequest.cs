using System.ComponentModel.DataAnnotations;

namespace MemeStation.Models.User
{
  public class CreateUserRequest
  {
    public string Address { get; set; }

    [Required]
    public string UserName { get; set; }
    [Required]
    public string Auth0Id { get; set; }

    public string Picture { get; set; }
    public string WalletDescription { get; set; }
  }
}
