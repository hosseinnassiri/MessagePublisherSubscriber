using MassTransit;
using MassTransit.MongoDbIntegration.Audit;
using System;

namespace MessageProcessor.BusConfiguration.AuditStore.MongoDb
{
	public static class MongoDBAuditStoreConfiguratorExtensions
	{
		/// <summary>
		/// Configure Audit Store against MongoDB
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="mongoDbConfigurator">The MongoDB configuration builder</param>
		/// <param name="configureFilter">Configure messages to exclude or include from auditing.</param>
		public static void UseMongoDBAuditStore(
			this IBusFactoryConfigurator configurator,
			IMongoDbConfigurator mongoDbConfigurator,
			Action<IMessageFilterConfigurator> configureFilter)
		{
			ConfigureAuditStore(configurator, mongoDbConfigurator.Settings, configureFilter);
		}

		/// <summary>
		/// Configure Audit Store against MongoDB
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="mongoDbConfigurator">The MongoDB configuration builder</param>
		public static void UseMongoDBAuditStore(
			this IBusFactoryConfigurator configurator,
			IMongoDbConfigurator mongoDbConfigurator)
		{
			ConfigureAuditStore(configurator, mongoDbConfigurator.Settings);
		}

		/// <summary>
		/// Configure Audit Store against MongoDB
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="configureMongoDb">Configure MongoDB settings</param>
		public static void UseMongoDBAuditStore(
			this IBusFactoryConfigurator configurator,
			Action<IMongoDbConfigurator> configureMongoDb)
		{
			var config = new MongoDbConfigurator();
			configureMongoDb.Invoke(config);
			ConfigureAuditStore(configurator, config.Settings);
		}

		private static void ConfigureAuditStore(
			IBusFactoryConfigurator configurator,
			ConfigurationMongoDbSettings mongoDbSettings,
			Action<IMessageFilterConfigurator>? configureFilter = default)
		{
			var auditStore = new MongoDbAuditStore(
				mongoDbSettings.Connection,
				mongoDbSettings.DatabaseName,
				mongoDbSettings.CollectionName);
			configurator.ConnectSendAuditObservers(auditStore, configureFilter);
			configurator.ConnectConsumeAuditObserver(auditStore, configureFilter);
		}
	}
}