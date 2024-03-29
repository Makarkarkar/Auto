using Auto.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using Auto.Website.GraphQL.GraphTypes;
using Auto.Website.GraphQL.Schemas;
using Auto.Website.Hubs;
using EasyNetQ;
using GraphQL;
using Microsoft.OpenApi.Models;

namespace Auto.Website {
    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services) {
            // services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSingleton<IAutoDatabase, AutoCsvFileDatabase>();
            services.AddSignalR();

            services.AddSwaggerGen(
                config => {
                    config.SwaggerDoc("v1", new OpenApiInfo() {
                        Title = "Auto API"
                    });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    config.IncludeXmlComments(xmlPath);
                });
            services.AddGraphQL(builder => builder
                .AddNewtonsoftJson()
                .AddAutoSchema<AutoSchema>()
                .AddSchema<AutoSchema>()
                .AddAutoSchema<OwnerSchema>()
                .AddSchema<OwnerSchema>()
                .AddGraphTypes(typeof(VehicleGraphType).Assembly)
            );
            var bus = RabbitHutch.CreateBus(Configuration.GetConnectionString("AutoRabbitMQ"));
            services.AddSingleton<IBus>(bus);
        }
        

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseGraphQLAltair();
                app.UseSwagger();
                app.UseSwaggerUI();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseGraphQL<OwnerSchema>();
            app.UseGraphQL<AutoSchema>();

            
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapHub<AutoHub>("/hub");
                
            });
        }
    }
}
