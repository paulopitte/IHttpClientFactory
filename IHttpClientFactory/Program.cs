using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace IHttpClientFactoryTest
{
    internal sealed class Program
    {
        public static int Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(@"c:\log\binance.log")
                .CreateLogger();


            try
            {
                Log.Information("Iniciando Processamento");
                CreateHost(args).Build().Run();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Falha: Host finalizado inesperadamente.");
                return 1;                
            }
            finally
            {
                Log.CloseAndFlush();
            }
           
            
        }


        public static IHostBuilder CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostcontext, services) =>            
            {
                services.AddHttpClient<WorkerProcess>(nameof(WorkerProcess), options =>
                {
                    options.BaseAddress = new Uri("https://api.binance.com/");
                });

                // Habilita o serviço backgroud
                services.AddHostedService<WorkerProcess>();

            })
            // Habilita o Uso do Seq
            .UseSerilog();
        
        
    }
}
