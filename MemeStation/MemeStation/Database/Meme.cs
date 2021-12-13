using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MemeStation.Database.Enums;
using MemeStation.Models;

namespace MemeStation.Database
{
    [Table("Meme")]
    public class Meme
    {
        [Key]
        [Column("meme_id", TypeName = "varchar(255)")]
        public string Id { get; set; }

        [Column("meme_type", TypeName = "varchar(255)"), Required]
        public string MemeType { get; set; }

        [Column("creator_address", TypeName = "varchar(255)"), Required]
        public string CreatorAddress { get; set; }

        public User Creator { get; set; }

        //[Column("owner_id", TypeName = "varchar(255)")]
        //public string OwnerId { get; set; }
        [Column("contract_address", TypeName = "varchar(255)")]
        public string ContractAddress { get; set; }

        [Column("token_id", TypeName = "varchar(255) UNIQUE"), Required]
        public string TokenId { get; set; }

        [Column(TypeName = "varchar(255)"), Required]
        public string Title { get; set; }

        [Column(TypeName = "varchar(255)")] public string Subtitle { get; set; }

        [Column(TypeName = "varchar(255)")] public string Description { get; set; }

        [Column("file_json_url", TypeName = "varchar(255) UNIQUE"), Required]
        public string FileJsonUrl { get; set; }

        [Column("file_type")] public FilteType Type { get; set; }

        [Column("price")] public decimal Price { get; set; }

        [Column("date", TypeName = "TIMESTAMP DEFAULT CURRENT_TIMESTAMP")]
        public DateTime CreationTime { get; set; }

        [Column(TypeName = "varchar(55)"), Required]
        public string Origin { get; set; }

        [Column("max_instance", TypeName = "INT DEFAULT 1")]
        public int MaxInstance { get; set; }

        public IEnumerable<TransferOwnership> TransferOwnerships { get; set; }
        public ICollection<User> Owners { get; set; }
        public List<MemeReaction> Reactions { get; set; }

        public List<MemeNews> News { get; set; }
    }
}