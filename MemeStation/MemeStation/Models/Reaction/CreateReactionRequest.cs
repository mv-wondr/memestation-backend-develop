using System.ComponentModel.DataAnnotations;
using MemeStation.Database.Enums;

namespace MemeStation.Models.Reaction
{
    public class CreateReactionRequest
    {
        [Required] public ReactionType ReactionType { get; set; }
        [Required] public string OriginatorAddress { get; set; }

        public string Comment { get; set; }
        public MediaPlatform Platform { get; set; }
    }
}