﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.SGP.Infra;
using SME.SGP.Worker.RabbitMQ;

namespace SME.SGP.Worker.Rabbbit
{
    public class Program {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    configurationBuilder.AddEnvironmentVariables();
                    configurationBuilder.AddUserSecrets<Program>();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<WorkerRabbitMQ>();
                    services.AddHealthChecks();
                    services.AddHealthChecksUiSgp();
                })
            ;
    }
}