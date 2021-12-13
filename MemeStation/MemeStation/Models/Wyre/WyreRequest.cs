using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace MemeStation.Models.Wyre
{
    public class WyreRequest
    {
        public ObjectId Id { get; set; }
        [Required]
        public string AccountId { get; set; }
        public string BlockchainNetworkTx {get; set; }
        [Required]
        public string Dest { get; set; }
        [Required]
        public string Fees { get; set; }
        [Required]
        public string OrderId { get; set; }
        [Required] 
        public string TransferId { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Auth0Id { get; set; }
        [Required]
        public decimal DestAmount { get; set; }
    }
}