﻿using MassTransit;
using MessageContracts;
using MessageProcessor.Handlers;
using MessageProcessor.Observers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageProcessor.BusConfiguration
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

			services.AddMassTransit(c => {
				c.AddConsumer<SomethingHappenedHandler>();
				c.UsingRabbitMq((context, config) => {
					config.Host(busSettings.RabbitMqSettings.Host, host => {
						host.Username(busSettings.RabbitMqSettings.UserName);
						host.Password(busSettings.RabbitMqSettings.Password);
					});
					config.UseMongoDbAuditStore(busSettings.MongoDbAuditStore.Connection,
						busSettings.MongoDbAuditStore.DatabaseName,
						busSettings.MongoDbAuditStore.CollectionName
					);

					var receiverObserver = context.GetRequiredService<IReceiveEndpointObserver>();
					config.Message<ISomethingHappened>(x =>
						x.SetEntityName(busSettings.RabbitMqSettings.PublishExchangeName));
					config.ReceiveEndpoint(busSettings.RabbitMqSettings.ListenerQueueName, endpoint => {
						endpoint.Consumer<SomethingHappenedHandler>(context);
						endpoint.Bind(busSettings.RabbitMqSettings.PublishExchangeName);
						endpoint.ConnectReceiveEndpointObserver(receiverObserver);
					});
				});
			});
			services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

			return services;
		}
	}
}
