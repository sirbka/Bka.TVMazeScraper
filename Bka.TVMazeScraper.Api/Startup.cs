using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using AutoMapper;
using Polly;
using Newtonsoft.Json;
using GlobalExceptionHandler.WebApi;

using Bka.TVMazeScraper.Api.Configurations;
using Bka.TVMazeScraper.Repositories;
using Bka.TVMazeScraper.ShowScraper;
using Bka.TVMazeScraper.Services;
using Bka.TVMazeScraper.Contracts;

namespace Bka.TVMazeScraper.Api
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

            var scraperSettings = new TVMazeScraperConfiguration();
            Configuration.GetSection("TVMazeScraperSettings").Bind(scraperSettings);
            Validator.ValidateObject(scraperSettings, new System.ComponentModel.DataAnnotations.ValidationContext(scraperSettings), true);
            services.AddSingleton<ITVMazeScraperConfiguration, TVMazeScraperConfiguration>(config => scraperSettings);

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();                
            });

            services.AddAutoMapper(typeof(AutoMapperProfileConfiguration));

            services.AddScoped<ITVShowContext, TVShowContext>();
            services.AddScoped<ITVShowRepository, TVShowRepository>();
            services.AddScoped<ITVMazeRepository, TVMazeRepository>();
            services.AddScoped<ITVShowService, TVShowService>();
            services.AddScoped<ITVMazeService, TVMazeService>();
            services.AddScoped<ITVMazeAPIScraper, TVMazeAPIScraper>();
            
            services.AddDbContext<TVShowContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("TVShowDatabase"),
                config => config.MigrationsAssembly(nameof(Bka.TVMazeScraper.Repositories))));

            // TVMaze HttpClient configuration
            var tvMaze429retryPolicy = Policy.HandleResult<HttpResponseMessage>(
                response => (int)response.StatusCode == 429)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(8),
                    TimeSpan.FromSeconds(10),
                });

            services.AddHttpClient(scraperSettings.TvMazeSrapperHttpClientName, client =>
            {
                client.BaseAddress = new Uri(scraperSettings.TVMazeBaseLink);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(tvMaze429retryPolicy);

            // add background service to scrape and update TVShow data
            services.AddHostedService<TVMazeScraperHostedService>();
        }

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            ConfigureExceptionHandler(app, env);

            app.UseMvc();
        }

        private void ConfigureExceptionHandler(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error").WithConventions(x =>
                {
                    x.ContentType = "application/json";
                    x.MessageFormatter(s => JsonConvert.SerializeObject(new
                    {
                        Message = "An error occurred whilst processing your request"
                    }));
                    x.ForException<TVMazeScraperCustomException>().ReturnStatusCode(HttpStatusCode.InternalServerError)
                        .UsingMessageFormatter((ex, context) => JsonConvert.SerializeObject(new
                        {
                            ex.Message
                        }));
                    x.ForException<TVMazeScraperBadRequestException>().ReturnStatusCode(HttpStatusCode.BadRequest)
                        .UsingMessageFormatter((ex, context) => JsonConvert.SerializeObject(new
                        {
                            ex.Message
                        }));
                });

                app.Map("/error", x => x.Run(y => throw new Exception()));
            }
        }
    }
}