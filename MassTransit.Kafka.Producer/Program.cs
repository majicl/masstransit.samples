using MassTransit;
using MassTransit.Kafka.Contracts;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddMassTransit(x =>
{
    const string topicName = "topicName-medium";
    const string kafkaBrokerServers = "localhost:9092";

    x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });

    x.AddRider(rider =>
    {
        rider.AddProducer<IMessage>(topicName);
        rider.UsingKafka((context, k) => { k.Host(kafkaBrokerServers); });
    });
});

var provider = services.BuildServiceProvider();

var busControl = provider.GetRequiredService<IBusControl>();

await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

var producer = provider.GetRequiredService<ITopicProducer<IMessage>>();
while (true)
{
    Console.WriteLine("Enter your message:");
    var msg = Console.ReadLine();

    if (msg == "q") break;

    await producer.Produce(new
    {
        Text = msg
    });
}

Console.ReadKey();