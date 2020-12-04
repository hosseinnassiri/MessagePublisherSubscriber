using MassTransit;
using MessageContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MessageProcessor
{
	public sealed class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		// the worker service is going to be registered as singleton,
		// therefore use ServiceProvider to get required services instead of nijecting;
		// otherwise, the injected service will be treated as singleton.
		private readonly IServiceProvider _serviceProvider;
		private readonly IHostApplicationLifetime _hostApplicationLifetime;
		private readonly IBusControl _busControl;
		public Worker(
		IServiceProvider serviceProvider,
		IBusControl busControl,
		IHostApplicationLifetime hostApplicationLifetime,
		ILogger<Worker> logger)
		{
			_serviceProvider = serviceProvider;
			_hostApplicationLifetime = hostApplicationLifetime;
			_logger = logger;
			_busControl = busControl;
		}
		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting the bus...");
			using var scope = _serviceProvider.CreateScope();
			var consumerObserver = scope.ServiceProvider.GetRequiredService<IConsumeMessageObserver<ISomethingHappened>>();
			_busControl.ConnectConsumeMessageObserver(consumerObserver);
			//NOTE: passing cancellation token to bus
			await _busControl.StartAsync(cancellationToken).ConfigureAwait(false);
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					//using var scope = _serviceProvider.CreateScope();
					//var processor = scope.ServiceProvider.GetRequiredService<>();
					// PipelineProcessor
					// process message
					_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
					await Task.Delay(5000, stoppingToken).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException ex)
			{
				_logger.LogError(ex, "Operation canceled {message}", ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, "An unhandled exception occurred {message}", ex.Message);
			}
			finally
			{
				// complete
				_hostApplicationLifetime.StopApplication();
			}
		}
		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			var sw = Stopwatch.StartNew();
			await _busControl.StopAsync(cancellationToken).ConfigureAwait(false);
			await base.StopAsync(cancellationToken).ConfigureAwait(false);
			_logger.LogInformation("Completed shutdown in {elapsed} ms.", sw.ElapsedMilliseconds);
		}
	}
}
