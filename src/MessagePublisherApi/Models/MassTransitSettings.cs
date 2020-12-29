namespace MessagePublisherApi.Models
{
	public sealed class MassTransitSettings
	{
		public RabbitMqSettings RabbitMqSettings { get; set; } = default!;
	}
}
