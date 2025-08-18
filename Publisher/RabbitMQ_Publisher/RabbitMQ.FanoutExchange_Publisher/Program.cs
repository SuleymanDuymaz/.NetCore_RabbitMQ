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

        // Exchange oluştur (fanout)
        await channel.ExchangeDeclareAsync(
            exchange: "fanout-exchange-example",
            type: ExchangeType.Fanout,
            autoDelete: false
        );

        // Kuyruk oluştur (farklı isim)
        string queueName = "fanout-queue-example";
        await channel.QueueDeclareAsync(
            queue: queueName,
            exclusive: false,
            autoDelete: false
        );

        // Kuyruğu exchange'e bağla
        await channel.QueueBindAsync(
            queue: queueName,
            exchange: "fanout-exchange-example",
            routingKey: string.Empty
        );

        var props = new BasicProperties { Persistent = true };

        // Mesaj gönderme
        for (int i = 0; i < 10; i++)
        {
            string text = $"Merhaba {i}";
            byte[] message = Encoding.UTF8.GetBytes(text);

            await channel.BasicPublishAsync(
                exchange: "fanout-exchange-example",
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: props,
                body: message
            );

            Console.WriteLine($"📤 Gönderildi: {text}");
            await Task.Delay(200);
        }

        Console.WriteLine("✅ Tüm mesajlar gönderildi!");
        Console.ReadLine();
    }
}
