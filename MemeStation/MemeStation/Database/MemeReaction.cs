using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MemeStation.Database.Enums;
using MemeStation.Models;

namespace MemeStation.Database
{

    [Table("reaction")]
    public class MemeReaction
    {
        [Key]
        [Column("reaction_id", TypeName = "varchar(255)")]
        public string ReactionId { get; set; }

        [Column("meme_id", TypeName = "varchar(255)"), Required]
        public string MemeId { get; set; }

        public Meme Meme { get; set; }

        [Column("reaction_type")] public ReactionType ReactionType { get; set; }

        [Column("originator_address", TypeName = "varchar(255)"), Required]
        public string OriginatorAddress { get; set; }

        public User Originator { get; set; }

        [Column(TypeName = "TIMESATMP DEFAULT CURRENT_TIMESTAMP")]
        public DateTime Date { get; set; }

        public Shared Shared { get; set; }
        public MemeComment Comment { get; set; }
    }

    [Table("Shared")]
    public class Shared
    {
        [Key]
        [Column("share_id", TypeName = "varchar(255)")]
        public string ShareId { get; set; }

        [Column("reaction_id", TypeName = "varchar(255)"), Required]
        public string MemeReactionId { get; set; }

        public MemeReaction Reaction { get; set; }

        [Column("platform")] public MediaPlatform Platform { get; set; }
    }

    [Table("meme_comment")]
    public class MemeComment
    {
        [Key]
        [Column("comment_id", TypeName = "varchar(255)")]
        public string CommentId { get; set; }

        [Column("reaction_id", TypeName = "varchar(255)"), Required]
        public string MemeReactionId { get; set; }

        public MemeReaction Reaction { get; set; }

        [Column("comment", TypeName = "varchar(255)"), Required]
        public string Comment { get; set; }

       // private ICollection<Reply> Replies { get; set; }
    }

    // [Table("reply")]
    // public class Reply
    // {
    //     [Column("comment_id", TypeName = "varchar(255)")]
    //     public string CommentId { get; set; }
    //
    //     public MemeComment Comment { get; set; }
    // }
}