using MongoDB.Driver;
using System;

namespace MessageProcessor
{
    public sealed class MongoDbConfigurator : IMongoDbConfigurator
    {
        public MongoDbConfigurator()
        {
			Settings = new ConfigurationMongoDbSettings();
        }

        public MongoDbConfigurator(string connection, string databaseName, string collectionName)
        {
			Settings = new ConfigurationMongoDbSettings(connection, databaseName, collectionName);
        }

		public ConfigurationMongoDbSettings Settings { get; }

		public string Connection
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Value should no be empty.", nameof(Connection));
                }
                var mongoUrl = MongoUrl.Create(value);
				Settings.Connection = mongoUrl.ToString();
                if (string.IsNullOrWhiteSpace(Settings.DatabaseName))
                {
					Settings.DatabaseName = mongoUrl.DatabaseName;
                }
            }
        }
        public string DatabaseName
        {
            set => Settings.DatabaseName = value;
        }
        public string CollectionName
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Value should no be empty.", nameof(CollectionName));
                }
				Settings.CollectionName = value;
            }
        }
    }
}
