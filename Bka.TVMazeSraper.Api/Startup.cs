using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AutoMapper;

using Bka.TVMazeSraper.Models;
using Bka.TVMazeSraper.Models.Interfaces;
using Bka.TVMazeSraper.Repositories;
using Bka.TVMazeSraper.ShowScraper;
using Bka.TVMazeSraper.ShowScraper.Interfaces;
using Bka.TVMazeSraper.Services.Interfaces;
using Bka.TVMazeSraper.Services;
using Bka.TVMazeSraper.Api.Map;

namespace Bka.TVMazeSraper.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            services.AddDbContext<TVShowContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("TVShowDatabase"),
                x => x.MigrationsAssembly("Bka.TVMazeSraper.Repositories")));

            services.AddTransient<ITVShowContext, TVShowContext>();
            services.AddTransient<ITVShowRepository, TVShowRepository>();
            services.AddTransient<ITVMazeRepository, TVMazeRepository>();
            services.AddTransient<ITVMazeAPIScraper, TVMazeAPIScraper>(
                scr => new TVMazeAPIScraper(Configuration.GetSection("TVMazeLinks")["ShowEmbedCast"]));
            services.AddTransient<ITVShowService, TVShowService>();
            services.AddTransient<ITVMazeService, TVMazeService>();

            var mappingConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TVShow, OutputTVShow>();
                cfg.CreateMap<Actor, OutputActor>();
            });
            services.AddSingleton<IMapper, IMapper>(sp => mappingConfig.CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}