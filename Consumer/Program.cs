using AutoMapper;
using Common.Models;
using Common.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserDTO, User>();
                    });

                    IMapper mapper = config.CreateMapper();
                    services.AddSingleton(mapper);

                    string connstring = $"mongodb://{Environment.GetEnvironmentVariable("MongoUser")}:{Environment.GetEnvironmentVariable("MongoPassword")}@{Environment.GetEnvironmentVariable("MongoServer")}:{Environment.GetEnvironmentVariable("MongoPort")}/{Environment.GetEnvironmentVariable("MongoDataBaseDefault")}";

                    services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(connstring));

                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddHostedService<Worker>();
                });
    }
}
