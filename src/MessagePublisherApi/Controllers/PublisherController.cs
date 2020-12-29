using MassTransit;
using MessageContracts;
using MessagePublisherApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MessagePublisherApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PublisherController : Controller
	{
		private readonly IBusControl _busControl;
		private readonly IPublishEndpoint _publisherEndpoint;
		private readonly ILogger<PublisherController> _logger;

		public PublisherController(
			IBusControl busControl,
			IPublishEndpoint publisherEndpoint,
			ILogger<PublisherController> logger)
		{
			_busControl = busControl;
			_publisherEndpoint = publisherEndpoint;
			_logger = logger;
		}

		[HttpPost("publish")]
		public async Task<IActionResult> Publish([FromBody] PublishRequest request)
		{
			try
			{
				_logger.LogInformation("Start publishing message ...");
				await _busControl.StartAsync().ConfigureAwait(false);
				await _publisherEndpoint.Publish<ISomethingHappened>(new SomeEvent(request.Data)).ConfigureAwait(false);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred!");
				return BadRequest();
			}
			finally
			{
				await _busControl.StopAsync().ConfigureAwait(false);
				_logger.LogInformation("Finished!");
			}
		}
	}
}
