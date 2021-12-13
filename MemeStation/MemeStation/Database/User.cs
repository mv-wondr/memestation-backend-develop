using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemeStation.Database
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("address", TypeName = "varchar(255)")]
        public string Address { get; set; }

        [Column("user_name", TypeName = "varchar(255)")]
        public string UserName { get; set; }
        [Required] 
        [Column("auth0_id", TypeName = "varchar(255)")]
        public string Auth0Id { get; set; }

        [Column("picture", TypeName = "varchar(255)")]
        public string Picture { get; set; }

        [Column("Wallet_description", TypeName = "varchar(255)")]
        public string WalletDescription { get; set; }

        //[Column("authenticated", TypeName = "int(1)")]
        public bool Authenticated { get; set; }

        public ICollection<Meme> OwnedMemes { get; set; }
        public ICollection<Meme> CreatedMemes { get; set; }
        public ICollection<MemeReaction> Reactions { get; set; }
        public IEnumerable<TransferOwnership> TransferOwnerships { get; set; }

        [NotMapped]
        public int AmountItems
        {
            get { return 0; }
        }

        [NotMapped] public decimal Balance => 0;

        //[]OwnedMemeIDS
    }
}
