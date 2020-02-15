using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using buscador.Data;
using Microsoft.EntityFrameworkCore;
using buscador.Services;
using buscador.Interfaces;
using buscador.Models;

namespace buscador
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
            services.AddDbContext<ScraperDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("default")));
            services.AddScoped<IScraper, Scraper>();
            services.AddScoped<IBusca, Busca>();

            services.AddSingleton<IScraperFactory, ScraperFactory>();
            //services.AddScoped<IScraperSite, ScraperSitePostHaus>();
            services.AddScoped<IScraperSite, ScraperSiteVkModas>();
            //services.AddScoped<IScraperSite, ScraperSiteDistritoModa>();

            services.Configure<List<TemplateBusca>>(Configuration.GetSection("ConfiguracoesBuscador:Sites"));
        
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ScraperDbContext>();
                context.Database.Migrate();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}