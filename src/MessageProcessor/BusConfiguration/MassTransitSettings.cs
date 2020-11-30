namespace MessageProcessor.BusConfiguration
{
	public sealed class MassTransitSettings
	{
		public RabbitMqSettings RabbitMqSettings { get; set; } = default!;
		public MongoDbAuditStoreSettings MongoDbAuditStore { get; set; } = default!;
	}

	public sealed class MongoDbAuditStoreSettings
	{
		public string Connection { get; set; } = string.Empty;
		public string DatabaseName { get; set; } = string.Empty;
		public string CollectionName { get; set; } = string.Empty;
	}
}
