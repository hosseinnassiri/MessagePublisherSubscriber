namespace MessagePublisher
{
	public sealed class RabbitMqSettings
	{
		public string Host { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string PublishExchangeName { get; set; } = string.Empty;
		public string ListenerQueueName { get; set; } = string.Empty;
	}
}
