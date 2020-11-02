using System;

namespace MessageContracts
{
	public interface ISomethingHappened : IEvent
	{
		string Data { get; }
		DateTimeOffset Date { get; }
	}

	public interface IEvent
	{
	}
}