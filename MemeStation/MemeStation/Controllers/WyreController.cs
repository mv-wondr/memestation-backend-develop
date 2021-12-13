using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MarketEngine.Service;
using MemeStation.Models.Invite;
using MemeStation.Models.Wyre;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;

namespace MemeStation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WyreController : ControllerBase
    {
        private readonly MongoHelper _mongoHelper;

        public WyreController(MongoHelper mongoHelper)
        {
            _mongoHelper = mongoHelper;
        }

        [HttpPost]
        public async Task<IActionResult> WyreInsert([FromBody] WyreRequest req)
        {
            if (_mongoHelper.TryInsert("wyre", req))
            {
                return Ok(req);
            }
            // TODO: this should be handled differently
            Console.WriteLine($"cannot insert:{req}" );
            return BadRequest();
        }

        [HttpPost("createReserve")]
        public async Task<IActionResult> CreateReserve([FromBody] WyreCreateReserveBody body)
        {
            var client = new RestClient("https://api.testwyre.com/v3/orders/reserve");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
       
            request.AddHeader("authorization", "Bearer SK-VBC9PUDF-PEXVFD8H-ZPLNFCAH-RA3P6BL9");

         

            var requestBody = new WyreCreateReserveRequestcs() {
                // TODO: i let this amount only for testing purpose, it should be change when task is over
                amount = body.Amount,
                sourceCurrency = "USD",
                destCurrency = "USD",
                dest = "account:AC_JJ7QC3LAJ3V",
                country = "CA",
                redirectUrl = "www.facebook.com",
                lockFields = new string[3] { "amount", "sourceCurrency", "destCurrency"},
                paymentMethod = "debit-card",
                referrerAccountId = "AC_JJ7QC3LAJ3V"
            };

            // Serialize our concrete class into a JSON String
            var stringPayload = JsonConvert.SerializeObject(requestBody);

            request.AddParameter("application/json", stringPayload, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            var wyreResponse = JsonConvert.DeserializeObject<WyreCreateReserveResponse>(response.Content);

            if (response.IsSuccessful)
            {
                return Ok(wyreResponse);
            }

            return BadRequest();
    }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mongoHelper.GetCollection<WyreRequest>("wyry", FilterDefinition<WyreRequest>.Empty));
        }
    }
}