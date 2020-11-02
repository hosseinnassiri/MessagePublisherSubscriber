﻿using MongoDB.Driver;

namespace MessageProcessor.BusConfiguration.AuditStore.MongoDb
{
	public interface IMongoDbConfigurator
	{
		/// <summary>
		/// Sets the database factory using connection string <see cref="MongoUrl" />
		/// </summary>
		string Connection { set; }

		/// <summary>
		/// Sets the database name
		/// </summary>
		string DatabaseName { set; }

		/// <summary>
		/// Sets the collection name
		/// </summary>
		string CollectionName { set; }

		/// <summary>
		/// Gets the MongoDb settings <see cref="ConfigurationMongoDbSettings"/>
		/// </summary>
		ConfigurationMongoDbSettings Settings { get; }
	}
}