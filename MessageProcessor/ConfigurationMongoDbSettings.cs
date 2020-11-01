namespace MessageProcessor
{
    public sealed class ConfigurationMongoDbSettings
    {
        public ConfigurationMongoDbSettings()
        {
        }

        public ConfigurationMongoDbSettings(string connection, string databaseName, string collectionName)
        {
            Connection = connection;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        /// <summary>
        /// MongoDB connection string
        /// </summary>
        public string Connection { get; set; } = string.Empty;

        /// <summary>
        /// The database name
        /// </summary>
        public string DatabaseName { get; set; } = string.Empty;

        /// <summary>
        /// The collection name
        /// </summary>
        public string CollectionName { get; set; } = string.Empty;
    }
}