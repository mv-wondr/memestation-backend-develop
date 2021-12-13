using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MemeStation.Models;

namespace MemeStation.Database
{
    [Table("Transfer_ownership")]
    public class TransferOwnership
    {
        [Key] public Guid Id { get; set; }

        [Column("sender_id", TypeName = "varchar(255)")]
        public string SenderId { get; set; }

        [Column("recipient_id", TypeName = "varchar(255)")]
        public string RecipientId { get; set; }

        public User Recipient { get; set; }

        [Column("meme_id", TypeName = "varchar(255)")]
        public string MemeId { get; set; }

        [Column("active", TypeName = "bool")] public bool Active { get; set; }
        public Meme Meme { get; set; }
    }
}