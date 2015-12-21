using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyNetSensors.WebServer.Code;
using WebServer.Models;
using WebServer.Services;

namespace WebServer
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["DataBase:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            //Add all SignalR related services to IoC.
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IConnectionManager connectionManager)
        {
            bool webServerDebug = false;
            IConfigurationSection logging=null;
            bool webServerEnable = false;
            try
            {
                logging = Configuration.GetSection("WebServer:Logging");
                webServerDebug = Boolean.Parse(Configuration["WebServer:Debug"]);
                webServerEnable = Boolean.Parse(Configuration["WebServer:Enable"]);
            }
            catch
            {
                Console.WriteLine("Bad configuration in appsettings.json file.");
                return;
            }

            if (webServerDebug)
                loggerFactory.AddConsole(logging);
            else
                loggerFactory.AddConsole(LogLevel.Warning);

            loggerFactory.AddDebug();

            if (webServerEnable)
                {
                    if (env.IsDevelopment())
                    {
                        app.UseBrowserLink();
                        app.UseDeveloperExceptionPage();
                        app.UseDatabaseErrorPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Home/Error");

                        // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                        try
                        {
                            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                                .CreateScope())
                            {
                                serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                                    .Database.Migrate();
                            }
                        }
                        catch
                        {
                        }
                    }

                    app.UseSignalR();

                    app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

                    app.UseStaticFiles();

                    app.UseIdentity();

                    // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

                    app.UseMvc(routes =>
                    {
                        routes.MapRoute(
                            name: "default",
                            template: "{controller=Home}/{action=Index}/{id?}");
                    });
                }

            SerialControllerInitializer.Start(loggerFactory, Configuration, connectionManager);

        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        }





    }
}
