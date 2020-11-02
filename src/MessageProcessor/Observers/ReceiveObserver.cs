using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MessageProcessor.Observers
{
	public sealed class ReceiveObserver : IReceiveEndpointObserver
	{
		private readonly ILogger<ReceiveObserver> _logger;

		public ReceiveObserver(ILogger<ReceiveObserver> logger)
		{
			_logger = logger;
		}

		public Task Completed(ReceiveEndpointCompleted completed)
		{
			return Task.CompletedTask;
		}

		public Task Faulted(ReceiveEndpointFaulted faulted)
		{
			_logger.LogError(faulted.Exception, "Error in receive endpoint");
			return Task.CompletedTask;
		}

		public Task Ready(ReceiveEndpointReady ready)
		{
			_logger.LogDebug("Endpoint is read to use: {@endpoint}", ready.ReceiveEndpoint);
			return Task.CompletedTask;
		}

		public Task Stopping(ReceiveEndpointStopping stopping)
		{
			return Task.CompletedTask;
		}
	}
}