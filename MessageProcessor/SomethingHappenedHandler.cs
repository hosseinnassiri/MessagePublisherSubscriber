using MassTransit;
using MessageContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageProcessor
{
    public class SomethingHappenedHandler : IConsumer<ISomethingHappened>
    {
		private readonly ILogger<SomethingHappenedHandler> _logger;

		public SomethingHappenedHandler(ILogger<SomethingHappenedHandler> logger)
		{
			_logger = logger;
		}

		public Task Consume(ConsumeContext<ISomethingHappened> context)
        {
			var loggingState = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
				[nameof(context.MessageId)] = context.MessageId.GetValueOrDefault()
			};

			using (_logger.BeginScope(loggingState))
			{
				_logger.LogDebug("Consuming message: {@message}", context.Message);
			}
            return Task.CompletedTask;
        }
    }
}