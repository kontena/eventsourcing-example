using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Kontena.EventSourcing;
using Kontena.ProductService.Models;

namespace Kontena.ProductService
{
    public class Startup
    {
        private Task _subscriber;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
            .AddJsonOptions(options =>
                {
                    // only do this for debug builds in a real application
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });
            services.AddSingleton<EventBusConfig<Product>, EventBusConfig<Product>>();
            services.AddSingleton<IRepository<Product>, InMemoryRepository<Product>>();
            services.AddSingleton<EventSerializer<Product>, EventSerializer<Product>>();
            services.AddSingleton<EventPublisher<Product, EventBusConfig<Product>>, EventPublisher<Product, EventBusConfig<Product>>>();
            services.AddSingleton<PersistingEventSubscriber<Product, EventBusConfig<Product>>, PersistingEventSubscriber<Product, EventBusConfig<Product>>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            _subscriber = app.ApplicationServices.
                              GetRequiredService<PersistingEventSubscriber<Product, EventBusConfig<Product>>>().
                              Run();
        }
    }
}
