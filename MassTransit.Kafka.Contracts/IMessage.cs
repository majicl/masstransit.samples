namespace MassTransit.Kafka.Contracts;

public interface IMessage
{
    string Text { get; }
}