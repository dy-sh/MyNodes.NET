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
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Code;
using MyNetSensors.WebController.Models;
using MyNetSensors.WebController.Services;

namespace MyNetSensors.WebController
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
            //database
            string connectionString;
            bool useMSSQL = Boolean.Parse(Configuration["DataBase:UseMSSQL"]);
            if (useMSSQL)
                connectionString = Configuration["DataBase:MSSQLConnectionString"];
            else
                connectionString = Configuration["DataBase:SqliteConnectionString"];

            if (useMSSQL)
            {
                services.AddEntityFramework()
                    .AddSqlServer()
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectionString))
                    .AddDbContext<NodesDbContext>(options =>
                        options.UseSqlServer(connectionString));
            }
            else
            {
                services.AddEntityFramework()
                    .AddSqlite()
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(connectionString))
                    .AddDbContext<NodesDbContext>(options =>
                        options.UseSqlite(connectionString));
            }

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
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory, 
            IConnectionManager connectionManager,
            NodesDbContext nodesDbContext
            )
        {
            //Set up dot instead of comma in float values
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;


            //debug settings
            bool webServerDebug = false;
            IConfigurationSection logging = null;
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




            //web server settings
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

                app.UseRuntimeInfoPage("/info");

                app.UseSignalR();

                app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

                app.UseStaticFiles();

                app.UseIdentity();

                // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}/{id2?}/{id3?}");
                });
            }


            SignalRServer.Start(connectionManager);
            SerialControllerConfigurator.nodesDbContext = nodesDbContext;
            StartSerialController(connectionManager);

        }

        public async Task StartSerialController(IConnectionManager connectionManager)
        {
            await Task.Run(() => SerialControllerConfigurator.Start(Configuration));
        }



        // Entry point for the application.
        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        }





    }
}
