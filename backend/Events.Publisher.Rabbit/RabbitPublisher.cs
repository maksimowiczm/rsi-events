using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;

namespace Events.Publisher.Rabbit;

public class RabbitPublisher(string hostName) : IPublisher
{
    private readonly ConnectionFactory _factory = new() { HostName = hostName };

    public Task PublishAsync(INotification notification)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("events", true, false, false, null);
        var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        });
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish("", "events", null, body);

        return Task.CompletedTask;
    }
}