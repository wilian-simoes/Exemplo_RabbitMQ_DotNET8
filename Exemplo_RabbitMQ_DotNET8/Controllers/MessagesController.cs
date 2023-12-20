using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace Exemplo_RabbitMQ_DotNET8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private const string QUEUE_NAME = "Mensagem";
        private const string RABBITMQ_CONNECTIONSTRING = "amqp://guest:guest@localhost/";

        private readonly ILogger<MessagesController> _logger;

        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }

        [HttpPost("EnviarMensagemParaFila")]
        public IActionResult EnviarMensagemParaFila([FromBody] MessageInputModel message)
        {
            try
            {
                _logger.LogInformation("Enviando mensagem para fila.");

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(RABBITMQ_CONNECTIONSTRING),
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: QUEUE_NAME,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var stringfieldMessage = System.Text.Json.JsonSerializer.Serialize(message);
                var bytesMessage = Encoding.UTF8.GetBytes(stringfieldMessage);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: QUEUE_NAME,
                    basicProperties: null,
                    body: bytesMessage);

                _logger.LogInformation("Mensagem adicionada a fila com sucesso.");
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error PostMessage. {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}