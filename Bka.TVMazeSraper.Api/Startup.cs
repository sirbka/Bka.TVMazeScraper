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
using Microsoft.Extensions.Hosting;

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
using System.Linq;

namespace Bka.TVMazeSraper.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
            services.AddTransient<ITVMazeAPIScraper, TVMazeAPIScraper>();

            services.AddDbContext<TVShowContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("TVShowDatabase"),
                config => config.MigrationsAssembly("Bka.TVMazeSraper.Repositories")), ServiceLifetime.Singleton);
            

            services.AddSingleton<ITVMazeScraperConfiguration, TVMazeScraperConfiguration>(config => 
                new TVMazeScraperConfiguration()
                {
                    TvMazeHttpClientName = Configuration.GetSection("HttpFactoryClients")["TVMazeHttpClient429Retry"],
                    TvMazeShowEmbedCastLinkPostfix = Configuration.GetSection("TVMazeLinks")["ShowEmbedCastPostfix"],
                    ScraperHostedServiceRepetition = TimeSpan.FromSeconds(int.Parse(Configuration.GetSection("ScraperHostedService")["RepetitionSeconds"]))
                });

            // mapper configuration
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<TVShow, OutputTVShow>()
                .ForMember(destinationMember => destinationMember.Cast, 
                    memberOptions => memberOptions.MapFrom(show => show.ActorsTVShows.Select(actorshow => actorshow.Actor)));
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

            // add background service to scrape and update TVShow data
            services.AddHostedService<TVMazeScraperHostedService>();            
        }

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}