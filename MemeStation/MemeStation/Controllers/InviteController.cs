using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MemeStation.Database;
using MemeStation.Models.Invite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace MemeStation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InviteController : ControllerBase
    {
        private readonly ILogger<MemesController> _logger;
        private readonly DatabaseContext _databaseContext;
        private readonly IDatabaseHelper _databaseHelper;

        public InviteController(ILogger<MemesController> logger, DatabaseContext databaseContext, IDatabaseHelper databaseHelper)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _databaseHelper = databaseHelper;
        }

        [HttpPost]
        public async Task<IActionResult> SafeCreateInviteCode([FromBody] CreateInviteRequest req)
        {
            var i = _databaseContext.Invites.FirstOrDefault(x => x.Email.Equals(req.Email));
            if (i != null)
            {
                return Ok(i);
            } 
            var inv = new Invite()
            {
                Code = ObjectId.GenerateNewId().ToString(),
                Email = req.Email
            };

            await _databaseContext.AddRangeAsync(inv);
            await _databaseContext.SaveChangesAsync();

            return Ok(inv);
        }

        [HttpGet]
        public async Task<IActionResult> ListCurrentInvites()
        {
            return Ok(_databaseContext.Invites.ToList());
        }

        [HttpGet("referral/{referrerCode}")]
        public async Task<IActionResult> ListReferral([FromRoute] string referrerCode)
        {
            var referral = _databaseContext.Invites.Include(r => r.Referrals)
                .FirstOrDefault(x => x.Code.Equals(referrerCode));

            if (referral == null)
            {
                return NotFound("Cannot found invite.");
            }

            var resp = new
            {
                Email = referral.Email,
                Code = referral.Code,
                Invites = referral.Referrals.Select(x => new
                {
                    Code = x.Code,
                    Email = x.SignupEmail
                })
            };

            return Ok(resp);
        }

        [HttpGet("stats/{referrerCode}")]
        public async Task<IActionResult> RefeererStats([FromRoute] string referrerCode)
        {
            var referralsAll = _databaseContext.Invites.Include(r => r.Referrals).OrderByDescending(t => t.Referrals.Count());
            var referral = referralsAll .FirstOrDefault(x => x.Code.Equals(referrerCode));

            if (referral == null)
            {
                return NotFound("Cannot found invite.");
            }

            var resp = new
            {
                Email = referral.Email,
                Code = referral.Code,
                CompletedReferrals = referral.Referrals.Count(),
                TopCompletedReferrals = referralsAll.First().Referrals.Count(),
                Rank = referralsAll.ToList().IndexOf(referral),
                TotalRank = referralsAll.Count(),
            };

            return Ok(resp);
        }


        [HttpPost("referral/{referrerCode}")]
        public async Task<IActionResult> CreateReferral([FromRoute] string referrerCode, [FromBody] CreateInviteRequest req)
        {
            var referral = _databaseContext.Invites.Include(x => x.Referrals).FirstOrDefault(x => x.Code.Equals(referrerCode));

            if (referral == null)
            {
                return NotFound("Cannot find invite.");
            }

            if (referral.Referrals.FirstOrDefault(x => x.SignupEmail.Equals(req.Email)) != null)
            {
                //SafeCreateReferral here
                 return StatusCode((int)HttpStatusCode.Conflict, "The user is already invited to the platform.");               
            }

            var r = new Referral()
            {
                Code = ObjectId.GenerateNewId().ToString(),
                SignupEmail = req.Email,
                ReferralCode = referral.Code
            };

            await _databaseContext.AddAsync(r);
            await _databaseContext.SaveChangesAsync();
            return Ok(new
            {
                Code = r.Code,
                Email = r.SignupEmail,
                InvitedBy = r.Invite?.Email
            });
        }
    }
}