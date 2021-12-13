using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MemeStation.Models;

namespace MemeStation.Database
{
    [Table("Meme_news")]
    public class MemeNews
    {
        [Key]
        [Column("news_id", TypeName = "varchar(255)")]
        public string NewsId { get; set; }

        [Column("meme_id", TypeName = "varchar(255)"), Required]
        public string MemeId { get; set; }
        public Meme Meme { get; set; }

        [Column(TypeName = "TIMESTAMP DEFAULT CURRENT_TIMESTAMP")]
        public DateTime Date { get; set; }
        
        [Column(TypeName = "varchar(255)"), Required] 
        public string Title { get; set; }

        [Column(TypeName = "varchar(255)")] 
        public string SubTitle { get; set; }
        
        [Column(TypeName = "varchar(255)"), Required]
        public string Url { get; set; }
        
        public ICollection<MemeReaction> Reactions { get; set; }
    }
}