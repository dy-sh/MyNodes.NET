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
            bool useMSSQL = Boolean.Parse(Configuration["DataBase:UseMSSQL"]);

            if (useMSSQL)
            {
                string connectionString = Configuration["DataBase:MSSQLConnectionString"];
                services.AddEntityFramework()
                    .AddSqlServer()
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectionString))
                    .AddDbContext<LogicalNodesDbContext>(options =>
                        options.UseSqlServer(connectionString))
                    .AddDbContext<LogicalNodesStatesDbContext>(options =>
                        options.UseSqlServer(connectionString))
                    .AddDbContext<NodesDbContext>(options =>
                        options.UseSqlServer(connectionString))
                    .AddDbContext<NodesHistoryDbContext>(options =>
                        options.UseSqlServer(connectionString))
                    .AddDbContext<NodesMessagesDbContext>(options =>
                        options.UseSqlServer(connectionString))
                    .AddDbContext<NodesTasksDbContext>(options =>
                        options.UseSqlServer(connectionString));
            }
            else
            {
                services.AddEntityFramework()
                    .AddSqlite()
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite("Data Source=Application.sqlite"))
                    .AddDbContext<LogicalNodesDbContext>(options =>
                        options.UseSqlite("Data Source=LogicalNodes.sqlite"))
                    .AddDbContext<LogicalNodesStatesDbContext>(options =>
                        options.UseSqlite("Data Source=LogicalNodesStates.sqlite"))
                    .AddDbContext<NodesDbContext>(options =>
                        options.UseSqlite("Data Source=Nodes.sqlite"))
                    .AddDbContext<NodesHistoryDbContext>(options =>
                        options.UseSqlite("Data Source=NodesHistory.sqlite"))
                    .AddDbContext<NodesMessagesDbContext>(options =>
                        options.UseSqlite("Data Source=NodesMessages.sqlite"))
                    .AddDbContext<NodesTasksDbContext>(options =>
                        options.UseSqlite("Data Source=NodesTasks.sqlite"));
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

            services.AddSingleton(x => Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IConnectionManager connectionManager,
            LogicalNodesDbContext logicalNodesDbContext,
            LogicalNodesStatesDbContext logicalNodesStatesDbContext,
            NodesDbContext nodesDbContext,
            NodesHistoryDbContext nodesHistoryDbContext,
            NodesMessagesDbContext nodesMessagesDbContext,
            NodesTasksDbContext nodesTasksDbContext
            )
        {
            //Set up dot instead of comma in float values
            System.Globalization.CultureInfo customCulture =
                (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
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

                app.UseStatusCodePages();

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}/{id2?}/{id3?}");
                });
            }


            SignalRServer.Start(connectionManager);
            SerialControllerConfigurator.logicalNodesDbContext = logicalNodesDbContext;
            SerialControllerConfigurator.logicalNodesStatesDbContext = logicalNodesStatesDbContext;
            SerialControllerConfigurator.nodesDbContext = nodesDbContext;
            SerialControllerConfigurator.nodesHistoryDbContext = nodesHistoryDbContext;
            SerialControllerConfigurator.nodesMessagesDbContext = nodesMessagesDbContext;
            SerialControllerConfigurator.nodesTasksDbContext = nodesTasksDbContext;

            bool firstRun = Boolean.Parse(Configuration["FirstRun"]);
            if (!firstRun)
                SerialControllerConfigurator.Start(Configuration);
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nThis is the first run of the system. \nYou can configure MyNetSensors from the web interface.\n"); // <-- see note
                Console.ForegroundColor = ConsoleColor.Gray;

            }

        }





        // Entry point for the application.
        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        }





    }
}
