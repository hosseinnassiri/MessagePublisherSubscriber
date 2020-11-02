using System;

namespace MessageContracts
{
	public interface ISomethingHappened : IEvent
	{
		/// <summary>
		/// Message data
		/// </summary>
		string Data { get; }

		/// <summary>
		/// Message timestamp
		/// </summary>
		DateTimeOffset Timestamp { get; }
	}

	public interface IEvent
	{
	}
}