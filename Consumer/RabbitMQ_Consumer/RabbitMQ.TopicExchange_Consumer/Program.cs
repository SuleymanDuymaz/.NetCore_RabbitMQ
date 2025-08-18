using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
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

        // 1️⃣ Exchange'i oluştur.
        await channel.ExchangeDeclareAsync(
            exchange: "topic-exchange-example",
            type: ExchangeType.Topic
        );

        // 2️⃣ Kuyruğu oluştur.
        // Publisher ile aynı kuyruğu kullandığımızdan, bu adımın gereği kalmayabilir.
        // Ancak yine de hata olmaması için eklemek iyi bir pratik.
        await channel.QueueDeclareAsync(
            queue: "shared-test-queue",
            exclusive: false
        );

        Console.WriteLine("Dinleme başladı. Mesajlar bekleniyor...");
        Console.WriteLine("Çıkmak için bir tuşa basın...");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (sender, e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.Span);
            Console.WriteLine($"[x] Alındı -> {e.RoutingKey}: {message}");
            return Task.CompletedTask;
        };

        // 3️⃣ "shared-test-queue" kuyruğunu dinle.
        await channel.BasicConsumeAsync(queue: "shared-test-queue", autoAck: true, consumer: consumer);

        Console.ReadLine();
    }
}