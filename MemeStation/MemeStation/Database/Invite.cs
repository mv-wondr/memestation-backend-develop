using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemeStation.Database
{
    [Table("invite")]
    public class Invite
    {
        [Key]
        [Column("code", TypeName = "varchar(255)")]
        public string Code { get; set; }

        [Column("email", TypeName = "varchar(255) UNIQUE")]
        [Required]
        public string Email { get; set; }

        public IEnumerable<Referral> Referrals { get; set; }
    }


}