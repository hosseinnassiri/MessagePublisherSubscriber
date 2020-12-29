using MessageContracts;
using System;

namespace MessagePublisher
{
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
