using MassTransit;
using MessageContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace MessagePublisher
{

	public class RabbitMqSettings
	{
		public string Host { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string PublishExchangeName { get; set; } = string.Empty;
		public string ListenerQueueName { get; set; } = string.Empty;
	}
}
