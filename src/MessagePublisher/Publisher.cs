using MassTransit;
using MessageContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePublisher
{
	public sealed class Publisher
	{
		private readonly IBusControl _busControl;
		private readonly IPublishEndpoint _publisherEndpoint;
		private readonly ILogger<Publisher> _logger;

		public Publisher(
			IBusControl busControl,
			IPublishEndpoint publisherEndpoint,
			ILogger<Publisher> logger)
		{
			_busControl = busControl;
			_publisherEndpoint = publisherEndpoint;
			_logger = logger;
		}

		public async Task Run()
		{
			var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
			try
			{
				_logger.LogInformation("Start publishing message ...");
				await _busControl.StartAsync(source.Token).ConfigureAwait(false);
				// while (true)
				// {
				// 	var value = await Task.Run(() => {
				// 		Console.WriteLine("Enter message (or quit to exit)");
				// 		Console.Write("> ");
				// 		return Console.ReadLine();
				// 	}).ConfigureAwait(false);

				// 	if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
				// 	{
				// 		break;
				// 	}

				// 	await _publisherEndpoint.Publish<ISomethingHappened>(new {
				// 		Data = value,
				// 		Timestamp = DateTimeOffset.UtcNow
				// 	}).ConfigureAwait(false);
				// }
				await _publisherEndpoint.Publish<ISomethingHappened>(new SomeEvent("goosfand")).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred!");
			}
			finally
			{
				await _busControl.StopAsync(source.Token).ConfigureAwait(false);
				_logger.LogInformation("Finished!");
			}
		}
	}

	internal sealed class SomeEvent : ISomethingHappened
	{
		public SomeEvent(string data)
		{
			Data = data;
			Timestamp = DateTimeOffset.UtcNow;
		}

		public string Data { get; }

		public DateTimeOffset Timestamp { get; }
	}
}
