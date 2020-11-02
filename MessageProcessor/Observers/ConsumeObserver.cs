using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageProcessor.Observers
{
	public sealed class ConsumeObserver<T> : IConsumeMessageObserver<T> where T : class
	{
		private readonly ILogger<ConsumeObserver<T>> _logger;

		public ConsumeObserver(ILogger<ConsumeObserver<T>> logger)
		{
			_logger = logger;
		}

		public Task ConsumeFault(ConsumeContext<T> context, Exception exception)
		{
			var loggingState = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
				[nameof(context.MessageId)] = context.MessageId.GetValueOrDefault()
			};

			using (_logger.BeginScope(loggingState))
			{
				_logger.LogError(exception, "Error in consuming message: {@message}", context.Message);
			}
			return Task.CompletedTask;
		}

		public Task PostConsume(ConsumeContext<T> context)
		{
			var loggingState = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
				[nameof(context.MessageId)] = context.MessageId.GetValueOrDefault()
			};

			using (_logger.BeginScope(loggingState))
			{
				_logger.LogDebug("Message consumed successfully");
			}
			return Task.CompletedTask;
		}

		public Task PreConsume(ConsumeContext<T> context)
		{
			var loggingState = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
				[nameof(context.MessageId)] = context.MessageId.GetValueOrDefault()
			};

			using (_logger.BeginScope(loggingState))
			{
				_logger.LogDebug("Received message to consume: {@message}", context.Message);
			}
			return Task.CompletedTask;
		}
	}
}