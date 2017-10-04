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
using Kontena.CustomerService.Models;

namespace Kontena.CustomerService
{
    public class Startup
    {
        private Task _subscriber;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
            .AddJsonOptions(options =>
                {
                    // only do this for debug builds in a real application
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });
            services.AddSingleton<EventBusConfig<Customer>, EventBusConfig<Customer>>();
            services.AddSingleton<IRepository<Customer>, InMemoryRepository<Customer>>();
            services.AddSingleton<EventSerializer<Customer>, EventSerializer<Customer>>();
            services.AddSingleton<EventPublisher<Customer, EventBusConfig<Customer>>, EventPublisher<Customer, EventBusConfig<Customer>>>();
            services.AddSingleton<PersistingEventSubscriber<Customer, EventBusConfig<Customer>>, PersistingEventSubscriber<Customer, EventBusConfig<Customer>>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            _subscriber = app.ApplicationServices.
                              GetRequiredService<PersistingEventSubscriber<Customer, EventBusConfig<Customer>>>().
                              Run();
        }
    }
}
