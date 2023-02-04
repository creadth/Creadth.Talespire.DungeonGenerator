using System;
using System.Net;
using Creadth.Talespire.DungeonGenerator.Framework;
using Creadth.Talespire.DungeonGenerator.Services.DungeonService;
using Creadth.Talespire.DungeonGenerator.Services.SlabService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Creadth.Talespire.DungeonGenerator
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
            if (bool.TryParse(Environment.GetEnvironmentVariable("ASPNET_USE_FORWARD_HEADERS"), out var useForward) &&
                useForward)
            {
                services.Configure<ForwardedHeadersOptions>(o =>
                {
                    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    o.KnownNetworks.Add(
                        new IPNetwork(IPAddress.Parse(Environment.GetEnvironmentVariable("ASPNET_PROXY_NETWORK")), 16));
                    o.KnownProxies.Add(IPAddress.Parse(Environment.GetEnvironmentVariable("ASPNET_PROXY_ADDRESS")));
                });
            }
            services.AddScoped<DungeonService>();
            services.AddScoped<SlabService>();
            services.AddSpaStaticFiles(x => x.RootPath = "client/dist");
            services.AddControllers(x => x.Conventions.Add(new ApiRouteConvention()))
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.Converters.Add(new Vector3Serializer());
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "0.0.1",
                Title = "Creadth TS",
            }));

            services
                .AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
            });
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client";
                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });

        }
    }
}
