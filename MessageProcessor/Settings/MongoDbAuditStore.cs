namespace MessageProcessor
{
    public class MongoDbAuditStoreSettings
    {
        public string Connection { get; set; } = string.Empty;
		public string DatabaseName { get; set; } = string.Empty;
		public string CollectionName { get; set; } = string.Empty;
	}
}
