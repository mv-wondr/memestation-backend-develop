using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemeStation.Database;
using MemeStation.Models.News;
using MongoDB.Bson;

namespace MemeStation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly ILogger<MemesController> _logger;
        private readonly DatabaseContext _databaseContext;
        private readonly IDatabaseHelper _databaseHelper;

        public NewsController(ILogger<MemesController> logger, DatabaseContext databaseContext, IDatabaseHelper databaseHelper)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _databaseHelper = databaseHelper;
        }

        /// <summary>
        ///     Get all news.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NewsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_databaseHelper.GetAllNews());
        }

        /// <summary>
        ///     Create a news.
        /// </summary>
        /// <param name="req">The requested meme to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(MemeNews), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMeme([FromBody] CreateNewsRequest req)
        {
            var news = new MemeNews()
            {
                NewsId = ObjectId.GenerateNewId().ToString(),
                MemeId = req.MemeId,
                Title = req.Title,
                SubTitle = req.SubTitle,
                Url = req.Url,
                Date = DateTime.Now
            };

            await _databaseContext.AddRangeAsync(news);
            await _databaseContext.SaveChangesAsync();

            return Ok(news);
        }

        /// <summary>
        ///     Get all news of a given meme.
        /// </summary>
        /// <param name="memeId">The specific identifier of the requested meme</param>
        /// <returns></returns>
        [HttpGet("{memeId}")]
        [ProducesResponseType(typeof(IEnumerable<NewsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNewsByMemeId([FromRoute] string memeId)
        {
            var e = _databaseHelper.GetAllMemes(x => x.Where(m => m.Id.Equals(memeId)));
            if (!e.Any())
            {
                return NotFound("Meme not found");
            }

            var u = _databaseHelper.GetAllNews().Where(x => x.MemeId == memeId);

            if (!u.Any())
            {
                return NotFound("No news found");
            }

            return Ok(u);
        }
    }
}
