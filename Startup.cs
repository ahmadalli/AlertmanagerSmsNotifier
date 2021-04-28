using AlertmanagerSmsNotifier.Models.Configs;
using AlertmanagerSmsNotifier.Services;
using AlertmanagerSmsNotifier.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AlertmanagerSmsNotifier
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
            services.AddHttpClient<ISmsSender, ArmaghanSender>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddOptions<GlobalConfigs>()
                .Bind(Configuration)
                .ValidateDataAnnotations();

            services.AddOptions<SmsIrConfigs>()
                .Bind(Configuration.GetSection(SmsIrConfigs.Position))
                .ValidateDataAnnotations();

            services.AddOptions<ArmaghanConfigs>()
                            .Bind(Configuration.GetSection(ArmaghanConfigs.Position))
                            .ValidateDataAnnotations();

            var provider = Configuration.Get<GlobalConfigs>().SmsProvider;
            switch (provider)
            {
                case SmsProvider.SmsIr:
                    services.AddTransient<ISmsSender, SmsIrSender>();
                    break;
                case SmsProvider.Armaghan:
                    services.AddTransient<ISmsSender, ArmaghanSender>();
                    break;
                default:
                    throw new System.Exception("SmsProvider hasn't been implemented yet");
            }

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
