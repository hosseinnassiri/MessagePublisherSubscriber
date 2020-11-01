using MassTransit;
using MessageContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageProcessor
{
    public static class BusConfigurationExtensions
    {
        public static IServiceCollection AddMassTransitBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var busSettingsSection = configuration.GetSection("MassTransitSettings");
            services.Configure<MassTransitSettings>(busSettingsSection);
            var busSettings = busSettingsSection.Get<MassTransitSettings>();

			services.AddScoped(typeof(IConsumeMessageObserver<>), typeof(ConsumeObserver<>));
			services.AddTransient<IReceiveEndpointObserver, ReceiveObserver>();

            services.AddMassTransit(c =>
            {
                c.AddConsumer<SomethingHappenedHandler>();
                c.AddBus(serviceProvider =>
                    Bus.Factory.CreateUsingRabbitMq(config =>
                    {
						config.Host(busSettings.RabbitMqSettings.Host, host => {
							host.Username(busSettings.RabbitMqSettings.UserName);
							host.Password(busSettings.RabbitMqSettings.Password);
						});
						config.UseMongoDBAuditStore(x => {
							x.Connection = busSettings.MongoDbAuditStore.Connection;
							x.DatabaseName = busSettings.MongoDbAuditStore.DatabaseName;
							x.CollectionName = busSettings.MongoDbAuditStore.CollectionName;
						});

						var receiverObserver = serviceProvider.GetRequiredService<IReceiveEndpointObserver>();
						config.Message<ISomethingHappened>(x =>
                        {
                            x.SetEntityName(busSettings.RabbitMqSettings.PublishExchangeName);
                        });
                        config.ReceiveEndpoint(busSettings.RabbitMqSettings.ListenerQueueName, endpoint =>
                        {
                            endpoint.Consumer<SomethingHappenedHandler>(serviceProvider);
							endpoint.Bind(busSettings.RabbitMqSettings.PublishExchangeName);
							endpoint.ConnectReceiveEndpointObserver(receiverObserver);
                        });
                        //config.ReceiveEndpoint($"{queueSettings.CommandConsumerQueueName}_skip", endpoint =>
                        //{
                        //  endpoint.Consumer<CheckPersonSanctionHitStatusCommandConsumer>(serviceProvider);
                        //});
                    })
                );
            });
            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            return services;
        }
    }
}
