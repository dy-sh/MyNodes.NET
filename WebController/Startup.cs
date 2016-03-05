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
using Microsoft.Data.Entity.Infrastructure;
using MyNodes.Repositories.EF.SQLite;
using MyNodes.Users;
using MyNodes.WebController.Code;

namespace MyNodes.WebController
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
            services.AddEntityFramework()
                .AddSqlite()
                .AddDbContext<NodesDbContext>(options =>
                    options.UseSqlite("Data Source=Nodes.sqlite"))
                .AddDbContext<NodesDataDbContext>(options =>
                    options.UseSqlite("Data Source=NodesData.sqlite"))
                .AddDbContext<MySensorsNodesDbContext>(options =>
                    options.UseSqlite("Data Source=MySensorsNodes.sqlite"))
                .AddDbContext<MySensorsMessagesDbContext>(options =>
                    options.UseSqlite("Data Source=MySensorsMessages.sqlite"))
                .AddDbContext<UsersDbContext>(options =>
                    options.UseSqlite("Data Source=Users.sqlite"));

            services.AddMvc();

            services.AddSignalR();

            services.AddSingleton(x => Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(UserClaims.DashboardObserver, policy => { policy.RequireClaim(UserClaims.DashboardObserver); });
                options.AddPolicy(UserClaims.DashboardEditor, policy => { policy.RequireClaim(UserClaims.DashboardEditor); });
                options.AddPolicy(UserClaims.EditorObserver, policy => { policy.RequireClaim(UserClaims.EditorObserver); });
                options.AddPolicy(UserClaims.EditorEditor, policy => { policy.RequireClaim(UserClaims.EditorEditor); });
                options.AddPolicy(UserClaims.HardwareObserver, policy => { policy.RequireClaim(UserClaims.HardwareObserver); });
                options.AddPolicy(UserClaims.LogsObserver, policy => { policy.RequireClaim(UserClaims.LogsObserver); });
                options.AddPolicy(UserClaims.LogsEditor, policy => { policy.RequireClaim(UserClaims.LogsEditor); });
                options.AddPolicy(UserClaims.ConfigObserver, policy => { policy.RequireClaim(UserClaims.ConfigObserver); });
                options.AddPolicy(UserClaims.ConfigEditor, policy => { policy.RequireClaim(UserClaims.ConfigEditor); });
                options.AddPolicy(UserClaims.UsersObserver, policy => { policy.RequireClaim(UserClaims.UsersObserver); });
                options.AddPolicy(UserClaims.UsersEditor, policy => { policy.RequireClaim(UserClaims.UsersEditor); });
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
                webServerDebug = Boolean.Parse(Configuration["WebServer:Debug"]);
                webServerEnable = Boolean.Parse(Configuration["WebServer:Enable"]);
            }
            catch
            {
                Console.WriteLine("Bad configuration in appsettings.json file.");
                return;
            }

            if (webServerDebug)
                loggerFactory.AddConsole(LogLevel.Debug);
            else
                loggerFactory.AddConsole(LogLevel.Error);

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


                DashboardSignalRServer.Start(connectionManager);
                NodeEditorSignalRServer.Start(connectionManager);
                MySensorsSignalRServer.Start(connectionManager);
                LogsSignalRServer.Start(connectionManager);
            }

            SystemController.Start(Configuration, serviceProvider);
        }


        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        }
    }
}
