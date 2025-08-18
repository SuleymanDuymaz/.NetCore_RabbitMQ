using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

// RabbitMQ bağlantı ayarları
var factory = new ConnectionFactory
{
    Uri = new Uri("myURI")
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

// Exchange oluştur (varsa sorun olmaz)
await channel.ExchangeDeclareAsync(
    exchange: "direct-exchange-example",
    type: ExchangeType.Direct
);

// Publisher ile aynı kuyruğu oluştur
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

var consumer = new AsyncEventingBasicConsumer(channel);

// Mesajları dinle
consumer.ReceivedAsync += async (sender, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.Span);
    Console.WriteLine($"📩 Gelen mesaj: {message}");

    // Mesajı onayla
    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
};

// Tüketmeye başla
await channel.BasicConsumeAsync(
    queue: "direct-queue-example",
    autoAck: false,
    consumer: consumer
);

Console.WriteLine("📡 Consumer başlatıldı. Mesaj bekleniyor...");
Console.ReadLine();
