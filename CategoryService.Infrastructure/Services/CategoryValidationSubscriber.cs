using CategoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using ValidateCategoryEvents;


namespace CategoryService.Infrastructure.Services
{
    public class CategoryValidationSubscriber(ILogger<CategoryValidationSubscriber> logger, IServiceProvider serviceProvider) : BackgroundService
    {
        private IConnection connection = null!;
        private IChannel channel = null!;
        private readonly string requestQueue = "validate_category_request";
        private readonly string responseQueue = "validate_category_response";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // Replace with your RabbitMQ server hostname
                Port = 5672,            // Replace with your RabbitMQ server port
                UserName = "guest",     // Replace with your RabbitMQ username
                Password = "guest"      // Replace with your RabbitMQ password
            };
            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: requestQueue, durable: false, exclusive: false, autoDelete: false);
            await channel.QueueDeclareAsync(queue: responseQueue, durable: false, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var request = JsonSerializer.Deserialize<ValidateCategoryGuidEvent>(body);

                if (request != null)
                {
                    bool isValid = false;
                    // Resolve scoped DbContext
                    using (var scope = serviceProvider.CreateAsyncScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<CategoryDbContext>();
                        isValid = await dbContext.Categories.AnyAsync(c => c.CategoryGuid == request.CategoryGuid);
                    }

                    var response = new ValidateCategoryGuidResponseEvent
                    {
                        RequestId = request.RequestId,
                        IsValid = isValid
                    };

                    var responseBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                    await channel.BasicPublishAsync(exchange: "", routingKey: responseQueue, body: responseBody);

                    logger.LogInformation("Processed ValidateCategoryGuidEvent for RequestId {RequestId}, IsValid={IsValid}", request.RequestId, isValid);
                }
            };

            await channel.BasicConsumeAsync(queue: requestQueue, autoAck: true, consumer: consumer);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if(channel != null)
                await channel.CloseAsync();
            if(connection != null)
                await connection.DisposeAsync();
           
            await base.StopAsync(cancellationToken);
        }
    }
}
