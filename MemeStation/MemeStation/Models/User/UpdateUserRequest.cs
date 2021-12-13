namespace MemeStation.Models.User
{
  public class UpdateUserRequest
  {
    public string UserName { get; set; }
    public string Picture { get; set; }
    public string WalletDescription { get; set; }
    public bool? Authenticated { get; set; }
    }
}
