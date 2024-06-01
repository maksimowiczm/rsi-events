using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;

namespace Events.Publisher.Rabbit;

public class RabbitPublisher : IPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitPublisher(string hostName)
    {
        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare("events", true, false, false, null);
    }

    public Task PublishAsync(INotification notification, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(notification, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        });
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish("", "events", null, body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}