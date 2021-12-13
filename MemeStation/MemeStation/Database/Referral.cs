using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemeStation.Database
{
    [Table("referral")]
    public class Referral
    {
        [Key]
        [Column("code", TypeName = "varchar(255)")]
        public string Code { get; set; }

        [Column("signup_email", TypeName = "varchar(255) UNIQUE")]
        [Required]
        public string SignupEmail { get; set; }

        [Column("referral_code", TypeName = "varchar(255)")]
        [Required]
        public string ReferralCode { get; set; }

        public Invite Invite { get; set; }
    }
}