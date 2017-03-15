using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyNodes.Repositories.EF.SQLite;
using MyNodes.Users;
using MyNodes.WebController.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MyNodes.WebController
{
    public class Startup
    {
        public static string SettingsFilePath;

        private string dbPath = "Databases";
        private string applicationPath;

        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            applicationPath = env.ContentRootPath;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(Constants.SETTINGS_FILE_NAME)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            SettingsFilePath = Path.Combine(applicationPath, $"appsettings.{env.EnvironmentName}.json");
            if (!File.Exists(SettingsFilePath))
            {
                SettingsFilePath = Path.Combine(applicationPath, Constants.SETTINGS_FILE_NAME);
            }

            if (env.IsDevelopment())
            {
                //builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework()
                .AddEntityFrameworkSqlite()
                .AddDbContext<NodesDbContext>(options =>
                    options.UseSqlite("Data Source=" + Path.Combine(applicationPath, dbPath, "Nodes.sqlite")))
                .AddDbContext<NodesDataDbContext>(options =>
                    options.UseSqlite("Data Source=" + Path.Combine(applicationPath, dbPath, "NodesData.sqlite")))
                .AddDbContext<MySensorsNodesDbContext>(options =>
                    options.UseSqlite("Data Source=" + Path.Combine(applicationPath, dbPath, "MySensorsNodes.sqlite")))
                .AddDbContext<MySensorsMessagesDbContext>(options =>
                    options.UseSqlite("Data Source=" + Path.Combine(applicationPath, dbPath, "MySensorsMessages.sqlite")))
                .AddDbContext<UsersDbContext>(options =>
                    options.UseSqlite("Data Source=" + Path.Combine(applicationPath, dbPath, "Users.sqlite")));

            services.AddMvc()
                .AddJsonOptions((options) =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

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

                app.UseWebSockets();
                app.UseSignalR();

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

                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = "Cookies",
                    LoginPath = new PathString("/User/Login"),
                    AccessDeniedPath = "/User/AccessDenied",
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
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
    }
}
