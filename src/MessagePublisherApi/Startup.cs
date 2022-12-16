using MassTransit;
using MessageContracts;
using MessagePublisherApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace MessagePublisherApi
{
	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "MessagePublisherApi", Version = "v1" }));

			var busSettingsSection = Configuration.GetSection("MassTransitSettings");
			services.Configure<MassTransitSettings>(busSettingsSection);
			var busSettings = busSettingsSection.Get<MassTransitSettings>();

			services.AddMassTransit(c => {
				c.AddBus(_ =>
					Bus.Factory.CreateUsingRabbitMq(config => {
						config.Host(busSettings.RabbitMqSettings.Host, host => {
							host.Username(busSettings.RabbitMqSettings.UserName);
							host.Password(busSettings.RabbitMqSettings.Password);
						});

						config.Message<ISomethingHappened>(x => x.SetEntityName(busSettings.RabbitMqSettings.PublishExchangeName));
					})
				);
			});
			services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

			//services.AddMassTransitHostedService();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MessagePublisherApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
