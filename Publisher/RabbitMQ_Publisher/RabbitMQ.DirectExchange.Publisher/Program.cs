using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("myURI")
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // Exchange oluştur
        await channel.ExchangeDeclareAsync(
            exchange: "direct-exchange-example",
            type: ExchangeType.Direct
        );

        // Kuyruğu oluştur
        await channel.QueueDeclareAsync(
            queue: "direct-queue-example",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // Kuyruğu exchange'e bağla
        await channel.QueueBindAsync(
            queue: "direct-queue-example",
            exchange: "direct-exchange-example",
            routingKey: "direct-queue-example"
        );

        var props = new BasicProperties { Persistent = true };

        while (true)
        {
            Console.Write("Mesaj: ");
            string? message = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(message))
                break;

            byte[] byteMessage = Encoding.UTF8.GetBytes(message);

            channel.BasicPublishAsync(exchange: "direct-exchange-example", routingKey: "direct-queue-example", body: byteMessage);

            Console.WriteLine("Mesaj gönderildi!");
        }
    }
}
