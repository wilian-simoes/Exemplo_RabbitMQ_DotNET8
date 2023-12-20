namespace ConsumoFila
{
    public class ParametrosExecucao
    {
        // Exemplo de Connection String do RabbitMQ:
        // amqp://guest:guest@localhost:15672/
        public string ConnectionString { get; init; }
        public string Queue { get; set; }
    }
}