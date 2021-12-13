namespace MemeStation.Models.Trade
{
    public class CancelRequest
    {
       public string NFTType { get; set; }
       public string OrderId { get; set; }
       public string SellerAddress { get; set; }
    }
}