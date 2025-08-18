using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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

// Kuyruk adını kullanıcıdan al
Console.Write("Kuyruk Adı Giriniz: ");
string queueName = Console.ReadLine() ?? "fanout-queue-example";

// Kuyruk oluştur
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

var consumer = new AsyncEventingBasicConsumer(channel);

// Event ekle
consumer.ReceivedAsync += async (sender, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.Span);
    Console.WriteLine($"📩 Gelen mesaj: {message}");
};

// Dinlemeye başla
await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("📡 Mesajlar dinleniyor...");
Console.ReadLine();
