using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
namespace IHttpClientFactoryTest
{
    /// <summary>
    /// Implementa classe Abstrata para serviços rodando em 2 plano
    /// </summary>
    class WorkerProcess : BackgroundService
    {
        private readonly ILogger<WorkerProcess> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WorkerProcess(ILogger<WorkerProcess> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (false.Equals(stoppingToken.IsCancellationRequested))
            {
                _logger.LogInformation("Processamento Iniciado em: {time}", DateTimeOffset.UtcNow.AddHours(-3));

                // Seta uma chave 
                var _httpClient = _httpClientFactory.CreateClient(nameof(WorkerProcess));

                var response = await _httpClient.GetAsync("api/v3/ticker/price");

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"ERROR CODE: {response.StatusCode}");
                else
                    _logger.LogInformation(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                //Aguarda 5s para novo processamento.
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
