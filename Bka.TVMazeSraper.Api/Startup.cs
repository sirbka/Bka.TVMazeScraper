using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;

using AutoMapper;
using Polly;

using Bka.TVMazeSraper.Api.Map;
using Bka.TVMazeSraper.Api.Configuration;
using Bka.TVMazeSraper.Models;
using Bka.TVMazeSraper.Models.Interfaces;
using Bka.TVMazeSraper.Repositories;
using Bka.TVMazeSraper.ShowScraper;
using Bka.TVMazeSraper.ShowScraper.Interfaces;
using Bka.TVMazeSraper.Services.Interfaces;
using Bka.TVMazeSraper.Services;

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

            services.AddTransient<ITVShowContext, TVShowContext>();
            services.AddTransient<ITVShowRepository, TVShowRepository>();
            services.AddTransient<ITVMazeRepository, TVMazeRepository>();
            services.AddTransient<ITVShowService, TVShowService>();
            services.AddTransient<ITVMazeService, TVMazeService>();
            services.AddScoped<ITVMazeAPIScraper, TVMazeAPIScraper>();

            services.AddSingleton<ITVMazeScraperConfiguration, TVMazeScraperConfiguration>(config => 
                new TVMazeScraperConfiguration()
                {
                    TvMazeHttpClientName = Configuration.GetSection("HttpFactoryClients")["TVMazeHttpClient429Retry"],
                    TvMazeShowEmbedCastLinkPostfix = Configuration.GetSection("TVMazeLinks")["ShowEmbedCastPostfix"]
                });

            services.AddDbContext<TVShowContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("TVShowDatabase"),
                x => x.MigrationsAssembly("Bka.TVMazeSraper.Repositories")));

            // mapper configuration
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<TVShow, OutputTVShow>();
                config.CreateMap<Actor, OutputActor>();
            });
            services.AddSingleton<IMapper, IMapper>(sp => mappingConfig.CreateMapper());

            // TVMaze HttpClient configuration
            var tvMaze429retryPolicy = Policy.HandleResult<HttpResponseMessage>(
                response => (int)response.StatusCode == 429)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(8),
                    TimeSpan.FromSeconds(10),
                });

            services.AddHttpClient(Configuration.GetSection("HttpFactoryClients")["TVMazeHttpClient429Retry"], client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("TVMazeLinks")["Base"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(tvMaze429retryPolicy);
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