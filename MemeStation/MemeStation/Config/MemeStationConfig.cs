using System.ComponentModel.DataAnnotations;

namespace MemeStation.Config
{
  public class MemeStationConfig
  {
    [Required]
    public string AlphaSecret { get; set; }
    [Required]
    public string ContractOwnerPrivKey { get; set; }
    [Required]
    public string ContractAddress { get; set; }
    [Required]
    public string InfuraUrl { get; set; }
    [Required]
    public string DbPath { get; set; }
  }
}
