using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Kontena.EventSourcing;
using Kontena.Dashboard.Middleware;
using Kontena.Dashboard.Models;
using Kontena.Dashboard.Services;

namespace Kontena.Dashboard
{
    public class Startup
    {
        private Task[] _subscriberTasks;

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
            services.AddSingleton<EventBusConfig<Customer>, EventBusConfig<Customer>>();
            services.AddSingleton<IRepository<Customer>, InMemoryRepository<Customer>>();
            services.AddSingleton<EventSerializer<Customer>, EventSerializer<Customer>>();
            services.AddSingleton<EventPublisher<Customer, EventBusConfig<Customer>>, EventPublisher<Customer, EventBusConfig<Customer>>>();
            services.AddSingleton<PersistingEventSubscriber<Customer, EventBusConfig<Customer>>, PersistingEventSubscriber<Customer, EventBusConfig<Customer>>>();

            services.AddSingleton<EventBusConfig<Product>, EventBusConfig<Product>>();
            services.AddSingleton<IRepository<Product>, InMemoryRepository<Product>>();
            services.AddSingleton<EventSerializer<Product>, EventSerializer<Product>>();
            services.AddSingleton<EventPublisher<Product, EventBusConfig<Product>>, EventPublisher<Product, EventBusConfig<Product>>>();
            services.AddSingleton<PersistingEventSubscriber<Product, EventBusConfig<Product>>, PersistingEventSubscriber<Product, EventBusConfig<Product>>>();

            services.AddSingleton<EventBusConfig<Purchase>, EventBusConfig<Purchase>>();
            services.AddSingleton<IRepository<Purchase>, InMemoryRepository<Purchase>>();
            services.AddSingleton<EventSerializer<Purchase>, EventSerializer<Purchase>>();
            services.AddSingleton<EventPublisher<Purchase, EventBusConfig<Purchase>>, EventPublisher<Purchase, EventBusConfig<Purchase>>>();
            services.AddSingleton<PersistingEventSubscriber<Purchase, EventBusConfig<Purchase>>, PersistingEventSubscriber<Purchase, EventBusConfig<Purchase>>>();

            services.AddSingleton<Repository, Repository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseWebSockets();

            _subscriberTasks = new[] {
                app.ApplicationServices.GetRequiredService<PersistingEventSubscriber<Customer, EventBusConfig<Customer>>>().Run(),
                app.ApplicationServices.GetRequiredService<PersistingEventSubscriber<Product, EventBusConfig<Product>>>().Run(),
                app.ApplicationServices.GetRequiredService<PersistingEventSubscriber<Purchase, EventBusConfig<Purchase>>>().Run()
            };

            app.UseWebsocketPubSub();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
