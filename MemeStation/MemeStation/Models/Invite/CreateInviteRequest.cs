using System.ComponentModel.DataAnnotations;

namespace MemeStation.Models.Invite
{
    public class CreateInviteRequest
    {
        [Required]
        public string Email { get; set; }
    }
}