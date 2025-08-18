
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


ConnectionFactory factory = new();

factory.Uri = new Uri("myURI");

// 2. Bağlantıyı oluştur    
await using var connection = await factory.CreateConnectionAsync();

// Kanal (Channel) oluşturma
await using var channel = await connection.CreateChannelAsync();

channel.QueueDeclareAsync(queue: "example-queue", exclusive: false,durable:true);// PUBLİSHER İLE CONSUMER AYNI TANIMLANMALI


AsyncEventingBasicConsumer consumer=new(channel);
channel.BasicConsumeAsync(queue: "example-queue",autoAck:false /*okunan parametereler default olarak kuyrutkan silinir.
* bu parametreyi false yaparak default olarak silmez. bekletir. 30 dakika boyunca yanıt bekler. herhnagi silme işlemi olmazs
* tekrar kuyruğa alır farklı cunsormer lar da bu veriyi işler. veri tutarsızlığı olur.*/,consumer);
channel.BasicQosAsync(0, 1, false); //eşit dağıtımlı sevkiyat işine yarar
consumer.ReceivedAsync += async (sender, e) =>
{
    //kuyruğa gelen mesajların işlendiği yer 
    //e.body kuyruktaki mesajın verisini bütünsel olarak getirecektir.
    //e.body.span veya ebody.to.array  kuyruktaki mesajın byte verisii getirecektir.
    var data = Encoding.UTF8.GetString(e.Body.Span);
  //  Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));

    //for(int i = 0; i < 100; i++)
    //{
       
    //    Console.WriteLine(data);
    //    Console.WriteLine("veriler çekiliyor.");
    //    Task.Delay(100);
    //}
    Console.WriteLine(data);
    //if başaarılısı ise basicackt
    //silinmediği taktirde ya da silinemediği takdirde rabbit sunucusu read konumundan emanet durumunua çekiyor mesajı.
   /*kuyruktan silmeye yarar*/ //channel.BasicAckAsync(deliveryTag:e.DeliveryTag,multiple:false);//multiple sadece bu mesaja dair parametre
    //ack olarak kuyrutan veriyi sil.

    channel.BasicNackAsync(deliveryTag:e.DeliveryTag,multiple:false,requeue:false);//consumer eğer mesajı işleyemezse
    //tekrardan kuyrupa almak için bu parametre kullanulr. requeue true ise kuyruğa alır

    //BasicCancle ve basicreject te bu act alanına girer ve cancel tüm kuyruktaki verileri işlemez bunu redderede reject ise tek mesjaa entegre çalışır.

};
Console.Read();