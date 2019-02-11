using System;
using System.IO;
using ExchangeRates.BL;
using ExchangeRates.Interfaces.BL;
using ExchangeRates.Interfaces.Storages;
using ExchangeRates.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace ExchangeRates.Service
{
    public class Startup : IDisposable
    {
        private readonly Container _container = new Container();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            IntegrateSimpleInjector(services);
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(_container));

            services.EnableSimpleInjectorCrossWiring(_container);
            services.UseSimpleInjectorAspNetRequestScoping(_container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            _container.RegisterMvcControllers(app);

            _container.Register<IRatesRepository, PgsqlRatesRepository>(Lifestyle.Singleton);
            _container.Register<IRatesLogic, RatesLogic>(Lifestyle.Transient);
            _container.Register<IRatesCache, SimpleRatesCache>(Lifestyle.Singleton);
            _container.Register<IRatesSource, AlphaVantageRatesSource>(Lifestyle.Transient);
            _container.Register(() => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build(), Lifestyle.Singleton);
            _container.Register(LogManager.GetCurrentClassLogger, Lifestyle.Singleton);

            _container.AutoCrossWireAspNetComponents(app);
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
