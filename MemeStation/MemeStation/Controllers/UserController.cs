using System;
using MemeStation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemeStation.Database;
using MemeStation.Models.User;
using MemeStation.ResponseBuilder;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexConvertors.Extensions;

namespace MemeStation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<MemesController> _logger;
        private readonly DatabaseContext _databaseContext;
        private readonly IDatabaseHelper _databaseHelper;

        public UserController(ILogger<MemesController> logger, DatabaseContext databaseContext, IDatabaseHelper databaseHelper)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            _databaseHelper = databaseHelper;
        }

        /// <summary>
        ///     Get all Users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Meme>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_databaseHelper.GetAllUsers().OrderByDescending(u => u.OwnedMemes.Count));
        }

        /// <summary>
        ///     Create a new user.
        /// </summary>
        /// <param name="creationRequest">The requested meme to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req)
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var newAddress = ecKey.GetPublicAddress();

            if (req.Address != null)
            {
              newAddress = req.Address;
            }

            var u = new User()
            {
                Address = newAddress,
                Auth0Id = req.Auth0Id,
                WalletDescription = req.WalletDescription,
                Picture = req.Picture,
                UserName = req.UserName
            };
            await _databaseContext.AddAsync(u);
            await _databaseContext.SaveChangesAsync();
            return Ok(req);
        }

        /// <summary>
        ///     Get the owner of the specified memeId.
        /// </summary>
        /// <param name="id">The specific user based off a memeId</param>
        /// <returns></returns>
        [HttpGet("memeId/{memeId}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByMemeId([FromRoute] string memeId)
        {
            var u = _databaseHelper.GetAllUsers().Where(u => u.OwnedMemes.ToList().Exists(m => m.Id == memeId));

            if (!u.Any())
            {
                return NotFound("User not found");
            }

            return Ok(u.First());
        }

        /// <summary>
        ///     Get one specific User.
        /// </summary>
        /// <param name="id">The specific user based off his address OR Auth0Id</param>
        /// <returns></returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser([FromRoute] string address)
        {
            var u = _databaseHelper.GetAllUsers(x => x.Where(u => u.Address.Equals(address)));

            if (!u.Any())
            {
                u = _databaseHelper.GetAllUsers(x => x.Where(u => u.Auth0Id.Equals(address)));
            }

            if (!u.Any())
            {
                return NotFound("User not found");
            }

            return Ok(u.First());
        }

        /// <summary>
        ///     Update a specific user.
        /// </summary>
        /// <param name="id">The specific identifier of the requested meme</param>
        /// <param name="updateRequest">The updated meme information with the same idea otherwise request will be discarded.</param>
        /// <returns></returns>
        [HttpPut("{address}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromRoute] string address, [FromBody] UpdateUserRequest req)
        {
            var u = _databaseContext.Users.Where(x => x.Address.Equals(address)).ToList();

            if (!u.Any())
            {
                return NotFound("User not found");
            }

            var e = u.First();

            e.Picture = req.Picture ?? e.Picture;
            e.WalletDescription = req.WalletDescription ?? e.WalletDescription;
            e.UserName = req.UserName ?? e.UserName;

            if (req.Authenticated != null)
            {
              e.Authenticated = (bool)req.Authenticated;
            }

            await _databaseContext.SaveChangesAsync();
            return Ok(u);
        }
    }
}
