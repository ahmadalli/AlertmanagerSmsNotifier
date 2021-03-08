using AlertmanagerSmsNotifier.Models.Configs;
using AlertmanagerSmsNotifier.Services;
using AlertmanagerSmsNotifier.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddOptions<GlobalConfigs>()
                .Bind(Configuration)
                .ValidateDataAnnotations();

            services.AddOptions<SmsIrConfigs>()
                .Bind(Configuration.GetSection(SmsIrConfigs.Position))
                .ValidateDataAnnotations();

            services.AddTransient<ISmsSender, SmsIrSender>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
