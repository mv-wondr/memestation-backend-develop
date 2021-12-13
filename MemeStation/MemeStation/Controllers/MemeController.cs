using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemeStation.Config;
using MemeStation.Contract;
using MemeStation.Database;
using MemeStation.Database.Enums;
using MemeStation.Models.Meme;
using MemeStation.Models.Reaction;
using MemeStation.ResponseBuilder;
using MemeStation.Service;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace MemeStation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemesController : ControllerBase
    {
        private readonly ILogger<MemesController> _logger;
        private readonly DatabaseContext _databaseContext;
        private readonly IDatabaseHelper _databaseHelper;
        private readonly MemeStationConfig _memeStationConfig;
        private readonly IMemeStationService _memeStationService;

        public MemesController(ILogger<MemesController> logger, DatabaseContext databaseContext, IDatabaseHelper databaseHelper, Config.MemeStationConfig memeStationConfig, IMemeStationService memeStationService)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _databaseHelper = databaseHelper;
            _memeStationConfig = memeStationConfig;
            _memeStationService = memeStationService;
        }


        /// <summary>
        ///     Get all memes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Meme>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_databaseHelper.GetAllMemes().OrderBy(x => x.TokenId));
        }

        /// <summary>
        ///     Create a meme.
        /// </summary>
        /// <param name="req">The requested meme to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Meme), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMeme([FromBody] CreateMemeRequest req)
        {
            if (req.AlphaSecret != _memeStationConfig.AlphaSecret)
            {
                return NotFound();
            }

            return Ok(await _memeStationService.CreateMeme(req));
        }

        [HttpPost("type")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMemeType([FromBody] CreateMemeTypeRequest req)
        {
            if (req.AlphaKey != _memeStationConfig.AlphaSecret)
            {
                return NotFound();
            }

            await _memeStationService.CreateMemeType(req);
            return Ok();
        }

        /// <summary>
        ///     Get one specific meme.
        /// </summary>
        /// <param name="id">The specific identifier of the requested meme</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Meme), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeme([FromRoute] string id)
        {
            var e = _databaseHelper.GetAllMemes(x => x.Where(m => m.Id.Equals(id)));
            if (!e.Any())
            {
                return NotFound("Meme not found");
            }

            return Ok(e.First());
        }

        /// <summary>
        ///     update a meme
        /// </summary>
        /// <param name="id">The specific identifier of the requested meme</param>
        /// <param name="updateRequest">The updated meme information with the same idea otherwise request will be discarded.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Meme), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMeme([FromRoute] string id, [FromBody] UpdateMemeRequest updateRequest)
        {
            var meme = _databaseContext.Memes.Where(m => m.Id.Equals(id)).ToList();
            if (!meme.Any())
            {
                return NotFound("No meme found with this id.");
            }

            var m = meme.First();

            m.ContractAddress = updateRequest.ContractAddress ?? m.ContractAddress;
            m.TokenId = updateRequest.TokenId ?? m.TokenId;
            m.Title = updateRequest.Title ?? m.Title;
            m.Subtitle = updateRequest.Subtitle ?? m.Subtitle;
            m.Description = updateRequest.Description ?? m.Description;
            m.FileJsonUrl = updateRequest.FileUrl ?? m.FileJsonUrl;
            m.Type = updateRequest.Type ?? m.Type;
            m.Price = updateRequest.Price ?? m.Price;
            m.Origin = updateRequest.Origin ?? m.Origin;
            m.MaxInstance = updateRequest.MaxInstance ?? m.MaxInstance;

            await _databaseContext.SaveChangesAsync();
            return Ok(m);
        }

        [Route("reaction/{memeId}")]
        [HttpPost]
        public async Task<IActionResult> PostMemeReaction([FromRoute] string memeId, [FromBody] CreateReactionRequest req)
        {
            //TODO: move me!
            if (req.ReactionType == ReactionType.Comment && req.Comment == null)
            {
                return BadRequest("The Comment field must be non-empty.");
            }

            if (req.ReactionType == ReactionType.Share && req.Platform == 0)
            {
                return BadRequest("Platform field must be non-empty");
            }

            var r = new MemeReaction()
            {
                ReactionId = ObjectId.GenerateNewId().ToString(),
                Date = DateTime.Now,
                MemeId = memeId,
                ReactionType = req.ReactionType,
                OriginatorAddress = req.OriginatorAddress
            };

            await _databaseContext.AddAsync(r);

            if (req.Comment != null)
            {
                var c = new MemeComment()
                {
                    MemeReactionId = r.ReactionId,
                    Comment = req.Comment,
                    CommentId = ObjectId.GenerateNewId().ToString()
                };

                await _databaseContext.AddAsync(c);
                await _databaseContext.SaveChangesAsync();

                return Ok(new ReactionResponse()
                {
                    Comment = c.Comment,
                    Date = r.Date,
                    MemeId = r.MemeId,
                    OriginatorAddress = r.OriginatorAddress,
                    ReactionId = r.ReactionId,
                    ReactionType = r.ReactionType.ToString()
                });
            }

            if (req.Platform != 0)
            {
                var s = new Shared()
                {
                    MemeReactionId = r.ReactionId,
                    ShareId = ObjectId.GenerateNewId().ToString(),
                    Platform = req.Platform,
                    Reaction = r
                };

                await _databaseContext.AddRangeAsync(s);
                await _databaseContext.SaveChangesAsync();

                return Ok(new ReactionResponse()
                {
                    Date = r.Date,
                    MemeId = r.MemeId,
                    OriginatorAddress = r.OriginatorAddress,
                    ReactionId = r.ReactionId,
                    ReactionType = r.ReactionType.ToString(),
                    // Platform = req
                });
            }

            await _databaseContext.SaveChangesAsync();
            return Ok(new ReactionResponse()
            {
                Date = r.Date,
                MemeId = r.MemeId,
                OriginatorAddress = r.OriginatorAddress,
                ReactionId = r.ReactionId,
                ReactionType = r.ReactionType.ToString(),
            });
        }

        [Route("reaction/{memeId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteMemeReaction([FromRoute] string memeId, [FromBody] CreateReactionRequest req)
        {
            //TODO: move me!
            if (req.ReactionType == ReactionType.Comment && req.Comment == null)
            {
                return BadRequest("The Comment field must be non-empty.");
            }

            if (req.ReactionType == ReactionType.Share && req.Platform == 0)
            {
                return BadRequest("Platform field must be non-empty");
            }


            _databaseContext.Reactions.RemoveRange(_databaseContext.Reactions.Where(r => r.ReactionType == req.ReactionType && r.OriginatorAddress == req.OriginatorAddress && r.MemeId == memeId));
            await _databaseContext.SaveChangesAsync();
            return Ok(req);
        }

        [Route("reaction/{memeId}")]
        [HttpGet]
        public async Task<IActionResult> GetMemeReaction([FromRoute] string memeId)
        {
            return Ok(_databaseHelper.GetAllReactions(x => x.Where(r => r.MemeId.Equals(memeId))).ToList());
        }

        [Route("reaction")]
        [HttpGet]
        public async Task<IActionResult> GetMemeReactions()
        {
            return Ok(_databaseHelper.GetAllReactions().ToList());
        }
    }
}
