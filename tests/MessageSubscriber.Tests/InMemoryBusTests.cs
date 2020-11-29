using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using MessageContracts;
using MessageProcessor.Handlers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace MessageSubscriber.Tests
{
	[TestClass]
	public class InMemoryBusTests
	{
		private InMemoryTestHarness _harness;

		[TestInitialize]
		public void Setup()
		{
			_harness = new InMemoryTestHarness();
		}

		[TestMethod]
		public async Task Should_consume_the_published_event()
		{
			var consumerHarness = _harness.Consumer(() =>
				new SomethingHappenedHandler(NullLogger<SomethingHappenedHandler>.Instance));

			try
			{
				await _harness.Start().ConfigureAwait(false);

				await _harness.InputQueueSendEndpoint.Send<ISomethingHappened>(new {
					Data = "something",
					Timestamp = DateTimeOffset.UtcNow
				}).ConfigureAwait(false);

				// did the endpoint consume the message
				(await _harness.Consumed.Any<ISomethingHappened>().ConfigureAwait(false)).Should().BeTrue();

				// did the actual consumer consume the message
				(await consumerHarness.Consumed.Any<ISomethingHappened>().ConfigureAwait(false)).Should().BeTrue();

				// ensure that no faults were published by the consumer
				(await _harness.Published.Any<Fault<ISomethingHappened>>().ConfigureAwait(false)).Should().BeFalse();
			}
			finally
			{
				await _harness.Stop().ConfigureAwait(false);
			}
		}

		[TestCleanup]
		public void Cleanup()
		{
			_harness.Dispose();
		}
	}
}
