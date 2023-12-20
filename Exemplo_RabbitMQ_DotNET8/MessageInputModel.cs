namespace Exemplo_RabbitMQ_DotNET8
{
    public class MessageInputModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}