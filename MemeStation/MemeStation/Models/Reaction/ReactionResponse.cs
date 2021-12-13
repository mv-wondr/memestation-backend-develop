using System;

namespace MemeStation.Models.Reaction
{
    public class ReactionResponse
    {
        public string ReactionId { get; set; }
        public string MemeId { get; set; }
        public  string ReactionType { get; set; }
        public string OriginatorAddress { get; set; }
        public DateTime Date { get; set; }
        
        public string Platform { get; set; }
        
        public string Comment { get; set; }
    }
}