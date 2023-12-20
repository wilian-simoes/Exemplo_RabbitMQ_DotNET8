using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ConsumoFila
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly int _intervaloWorkerAtivo = 60000;
        private readonly ParametrosExecucao _parametrosExecucao;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration,
            ParametrosExecucao parametrosExecucao)
        {
            logger.LogInformation($"Queue = {parametrosExecucao.Queue}");

            _logger = logger;
            //_intervaloWorkerAtivo = Convert.ToInt32(configuration["IntervaloWorkerAtivo"]);
            _parametrosExecucao = parametrosExecucao;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Aguardando mensagens...");

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_parametrosExecucao.ConnectionString)
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _parametrosExecucao.Queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(queue: _parametrosExecucao.Queue,
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    $"Worker ativo em {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                await Task.Delay(_intervaloWorkerAtivo, stoppingToken);
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e) 
        {
            _logger.LogInformation(
                $"[Nova mensagem | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                Encoding.UTF8.GetString(e.Body.ToArray()));
        }
    }
}