
using Confluent.Kafka;
using Invoice.Core.Abstraction;
using InvoiceService.API.InvoiceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RoomService.Infrastructure.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Invoice.API.KafkaConsumerService
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _topic;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IKafKaPublisherService _producer;

        public KafkaConsumerService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, ILogger logger, IKafKaPublisherService producer)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _config = configuration;
            _producer = producer;   

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"],
                GroupId = "your-group-id",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                FetchMinBytes = 1,
                EnableAutoCommit = true,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = _config["Kafka:SaslUsername"],
                SaslPassword = _config["Kafka:SaslPassword"],
            };

            _topic = "Notification-Topic";
            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);

            await Task.Run(async () =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);
                        if (consumeResult != null)
                        {
                            await ProcessMessage(consumeResult.Message.Value);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.Warning("Kafka consumer operation was cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred while consuming Kafka messages.");
                }
                finally
                {
                    _consumer.Close();
                }
            }, stoppingToken);
        }

        private async Task ProcessMessage(string messageValue)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var bidMessage = JsonConvert.DeserializeObject<BidderModel>(messageValue);

            if (bidMessage != null)
            {
                var existingBid = await context.Invoices
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == bidMessage.Id && c.UserName == bidMessage.UserName &&
                                              c.Cars == bidMessage.Cars && c.AmountPaid == bidMessage.AmountPaid);

                if (existingBid == null)
                {
                    await context.Invoices.AddAsync(bidMessage);
                    await context.SaveChangesAsync();
                }
            }

            var maxAmountPaid = await context.Invoices.MaxAsync(b => b.AmountPaid);
            var highestBidder = await context.Invoices
                .Where(b => b.AmountPaid == maxAmountPaid)
                .FirstOrDefaultAsync();
           
            var Invoice = new GenerateInvoiceModel()
            {
                UserName = highestBidder.UserName,
                Amount = highestBidder.AmountPaid,
                Cars = highestBidder.Cars,
                InvoiceDate = DateTime.UtcNow
            };

            var message = JsonConvert.SerializeObject(Invoice);

                Log.Information($"Sending invoice to Kafka: {Invoice}");

                await _producer.ProduceAsync(highestBidder.Id, message);
                Log.Information("Bid message sent successfully to Kafka");
        }

        public override void Dispose()
        {
            _consumer?.Dispose();
            base.Dispose();
        }
    }
}
