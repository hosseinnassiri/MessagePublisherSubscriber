using MessageProcessor.BusConfiguration.AuditStore.MongoDb;

namespace MessageProcessor.BusConfiguration
{
	public sealed class MassTransitSettings
	{
		public RabbitMqSettings RabbitMqSettings { get; set; } = default!;
		public MongoDbAuditStoreSettings MongoDbAuditStore { get; set; } = default!;
	}
}
