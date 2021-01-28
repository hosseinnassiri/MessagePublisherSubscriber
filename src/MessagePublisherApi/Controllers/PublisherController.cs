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
		private readonly IPublishEndpoint _publisherEndpoint;
		private readonly ILogger<PublisherController> _logger;

		public PublisherController(
			IPublishEndpoint publisherEndpoint,
			ILogger<PublisherController> logger)
		{
			_publisherEndpoint = publisherEndpoint;
			_logger = logger;
		}

		[HttpPost("publish")]
		public async Task<IActionResult> Publish([FromBody] PublishRequest request)
		{
			try
			{
				_logger.LogInformation("Start publishing message ...");
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
				_logger.LogInformation("Finished!");
			}
		}
	}
}
