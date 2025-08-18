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

        string queueName = "example-queue";

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false

           
        );

        var properties = new BasicProperties
        {
            Persistent = true
        };

        byte[] body = Encoding.UTF8.GetBytes("Merhaba");

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
    }
}
