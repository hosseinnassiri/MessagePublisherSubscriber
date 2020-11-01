using MassTransit;
using MessageContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace MessagePublisher
{
	internal class Program
    {
		public static async Task Main(string[] args)
		{
			// create service collection
			var services = new ServiceCollection();
			ConfigureServices(services);

			// create service provider
			var serviceProvider = services.BuildServiceProvider();

			// entry to run app
			await serviceProvider.GetService<Publisher>().Run().ConfigureAwait(false);
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			// configure logging
			services.AddLogging(builder =>
			{
				builder.AddConsole();
				builder.AddDebug();
			});

			// build config
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false)
				.Build();

			var busSettingsSection = configuration.GetSection("MassTransitSettings");
			services.Configure<MassTransitSettings>(busSettingsSection);
			var busSettings = busSettingsSection.Get<MassTransitSettings>();

			services.AddMassTransit(c => {
				c.AddBus(serviceProvider =>
					Bus.Factory.CreateUsingRabbitMq(config => {
						config.Host(busSettings.RabbitMqSettings.Host, host => {
							host.Username(busSettings.RabbitMqSettings.UserName);
							host.Password(busSettings.RabbitMqSettings.Password);
						});

						config.Message<ISomethingHappened>(x => {
							x.SetEntityName(busSettings.RabbitMqSettings.PublishExchangeName);
						});
					})
				);
			});
			services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

			// add app
			services.AddTransient<Publisher>();
		}
	}
}
