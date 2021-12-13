using System;
using System.Security.Policy;
using System.Threading.Tasks;
using MarketEngine.Service;
using MarketEngineLib;
using MemeStation.Core;
using System.IO;
using System.Text.Json;
using MemeStation.Config;
using MemeStation.Contract;
using MemeStation.Database;
using MemeStation.Models;
using MemeStation.ResponseBuilder;
using MemeStation.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using OrderMatcher;
using System.Net.WebSockets;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Threading;
using MemeStation.Controllers;
using Microsoft.Extensions.Logging;
using SignalRChat.Hubs;

namespace MemeStation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = InstantiateConfig(Environment.GetEnvironmentVariable("CONFIG_PATH"));

            //Dependency injection
            services.AddScoped<IDatabaseHelper, DatabaseHelper>();

            var mh = new MongoHelper();
            services.AddSingleton<IMongoHelper, MongoHelper>();
            services.AddSingleton(x => mh);
            services.AddScoped<ITradeListener, NFTTradeListener>();
            services.AddScoped<IOrderLib, OrderLib>();
            services.AddScoped<IMarketLib, MarketLib>(x => new MarketLib(mh, MarketNameValidatorFunc));
            services.AddScoped<INFTEngine, NFTEngine>();
            services.AddScoped<IAuction, Auction>();



            services.AddSingleton(x => config);
            var web3 = new Web3(new Account(config.ContractOwnerPrivKey), config.InfuraUrl);
            services.AddScoped(x => web3);
            services.AddScoped(x => web3.Eth.GetContractHandler(config.ContractAddress));
            services.AddScoped<IMemeStationService, MemeStationService>();
            services.AddScoped<IContractService, ContractService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
          {
                  builder.AllowAnyOrigin()
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .AllowAnyHeader();
              });
            });

            services.AddControllers();


            services.AddDbContext<DatabaseContext>(
              options => options.UseSqlite(config.DbPath));

            services.AddSignalR();
        }

        private MemeStationConfig InstantiateConfig(string configPath)
        {
            if (configPath == null)
            {
                throw new Exception("CONFIG_PATH must be set");
            }

            var config = JsonSerializer.Deserialize<MemeStationConfig>(File.ReadAllText(configPath));
            if (config == null)
            {
                throw new Exception("Config file is missing.");
            }

            foreach (var prop in config.GetType().GetProperties())
            {
                if (prop.GetValue(config) == null)
                {
                    throw new Exception($"Couldn't find value for property: ${prop.Name}");
                }
            }

            return config;
        }

        private async Task<bool> MarketNameValidatorFunc(string arg)
        {
            //TODO:feature minting is not yet merged into develop!
            return true;
            throw new NotImplementedException("feature/minting should be merged into develop.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseAuthorization();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };

            app.UseWebSockets(webSocketOptions);

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
