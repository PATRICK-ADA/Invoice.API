namespace InvoiceService.API.InvoiceService.Core.Abstraction
{
    public interface IKafkaConsumerServices
    {
        void Consume(CancellationToken cancellationToken);
    }
}
