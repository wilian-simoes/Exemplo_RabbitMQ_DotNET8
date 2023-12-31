﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsumoFila
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(
                "*** Testando o consumo de mensagens com RabbitMQ + Filas ***");

            if (args.Length != 2)
            {
                Console.WriteLine(
                    "Informe 2 parametros: " +
                    "no primeiro a string de conexao com o RabbitMQ, " +
                    "no segundo a Fila/Queue a ser utilizado no consumo das mensagens...");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton(
                        new ParametrosExecucao()
                        {
                            ConnectionString = args[0],
                            Queue = args[1]
                        });
                    services.AddHostedService<Worker>();
                });
    }
}