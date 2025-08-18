using RabbitMQ.Client;
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

        // 1️⃣ Exchange oluştur
        await channel.ExchangeDeclareAsync(
            exchange: "topic-exchange-example",
            type: ExchangeType.Topic
        );

        // 2️⃣ Queue oluştur
        // Bu, kalıcı bir kuyruk (exclusive: false) olduğu için kod kapansa bile durur.
        await channel.QueueDeclareAsync(
            queue: "test-queue",
            exclusive: false
        );

        Console.WriteLine("Gönderim yapacağınız topic alanını giriniz (örn: haberler.spor.futbol):");
        string topic = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(topic))
        {
            Console.WriteLine("Topic alanı boş bırakılamaz.");
            return;
        }

        // 3️⃣ Kuyruğu exchange'e bağla
        // Bu işlem, girdiğin topic ile gönderilen mesajların "test-queue" kuyruğuna düşmesini sağlar.
        await channel.QueueBindAsync(
            queue: "test-queue",
            exchange: "topic-exchange-example",
            routingKey: topic
        );

        // 4️⃣ Mesaj gönder
        for (int i = 0; i < 10; i++)
        {
            string textMessage = $"Merhaba {i}";
            byte[] message = Encoding.UTF8.GetBytes(textMessage);

            await channel.BasicPublishAsync(
                exchange: "topic-exchange-example",
                routingKey: topic,
                body: message
            );

            Console.WriteLine($"[x] Gönderildi -> {topic} : {textMessage}");
        }

        Console.WriteLine("✅ Tüm mesajlar gönderildi!");
    }
}