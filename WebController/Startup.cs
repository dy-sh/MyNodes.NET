using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Query.ExpressionTranslators.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyNetSensors.Repositories.EF.SQLite;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //if (Boolean.Parse(Configuration["DataBase:Enable"])
            //    && Boolean.Parse(Configuration["DataBase:UseInternalDb"]))
            //{
                services.AddEntityFramework()
                    .AddSqlite()
                    .AddDbContext<NodesDbContext>(options =>
                        options.UseSqlite("Data Source=Nodes.sqlite"))
                    .AddDbContext<NodesStatesHistoryDbContext>(options =>
                        options.UseSqlite("Data Source=NodesStatesHistory.sqlite"))
                    .AddDbContext<MySensorsNodesDbContext>(options =>
                        options.UseSqlite("Data Source=MySensorsNodes.sqlite"))
                    .AddDbContext<MySensorsMessagesDbContext>(options =>
                        options.UseSqlite("Data Source=MySensorsMessages.sqlite"))
                    .AddDbContext<UITimerNodesDbContext>(options =>
                        options.UseSqlite("Data Source=UITimerNodes.sqlite"))
                    .AddDbContext<UsersDbContext>(options =>
                        options.UseSqlite("Data Source=Users.sqlite"));
            //}

            services.AddMvc();

            services.AddSignalR();

            services.AddSingleton(x => Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DashboardObserver", policy => { policy.RequireClaim("DashboardObserver"); });
                options.AddPolicy("DashboardEditor", policy => { policy.RequireClaim("DashboardEditor"); });
                options.AddPolicy("EditorObserver", policy => { policy.RequireClaim("EditorObserver"); });
                options.AddPolicy("EditorEditor", policy => { policy.RequireClaim("EditorEditor"); });
                options.AddPolicy("HardwareObserver", policy => { policy.RequireClaim("HardwareObserver"); });
                options.AddPolicy("LogsObserver", policy => { policy.RequireClaim("LogsObserver"); });
                options.AddPolicy("LogsEditor", policy => { policy.RequireClaim("LogsEditor"); });
                options.AddPolicy("ConfigObserver", policy => { policy.RequireClaim("ConfigObserver"); });
                options.AddPolicy("ConfigEditor", policy => { policy.RequireClaim("ConfigEditor"); });
                options.AddPolicy("UsersObserver", policy => { policy.RequireClaim("UsersObserver"); });
                options.AddPolicy("UsersEditor", policy => { policy.RequireClaim("UsersEditor"); });
            });


        }


        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IConnectionManager connectionManager,
            IServiceProvider serviceProvider
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
                }

                app.UseRuntimeInfoPage("/info");

                app.UseWebSockets();
                app.UseSignalR();

                app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

                app.UseStaticFiles();

                app.UseStatusCodePages();

                //redirect to /FirstRun
                app.Use(async (context, next) =>
                {
                    if (Boolean.Parse(Configuration["FirstRun"])
                    && !context.Request.Path.ToUriComponent().StartsWith("/FirstRun"))
                    {
                        context.Response.Redirect("/FirstRun");
                        return;
                    }
                    //invoke next component
                    await next.Invoke();
                });

                app.UseCookieAuthentication(options =>
                {
                    options.AuthenticationScheme = "Cookies";
                    options.LoginPath = new Microsoft.AspNet.Http.PathString("/User/Login");
                    options.AccessDeniedPath = "/User/AccessDenied";
                    options.AutomaticAuthenticate = true;
                    options.AutomaticChallenge = true;
                });


                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}/{id2?}/{id3?}");
                });


                NodesEngineSignalRServer.Start(connectionManager);
                MySensorsSignalRServer.Start(connectionManager);
            }

            SystemController.Start(Configuration, serviceProvider);
        }


        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        }
    }
}
