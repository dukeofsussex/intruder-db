// Copyright (c) Chris Satchell. All rights reserved.

namespace API
{
    using System.IO;
    using System.Text;
    using API.Middleware;
    using API.Services;
    using API.Services.Jobs;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.DotNet.PlatformAbstractions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using NLog.Extensions.Logging;
    using NLog.LayoutRenderers;
    using NLog.Web;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();

            LayoutRenderer.Register("currentdir", (logEvent) => Directory.GetCurrentDirectory());
            env.ConfigureNLog("nlog.config");
        }

        public IConfigurationRoot Configuration { get; }

        private IJobManager JobManager { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.Configuration["JWT:Issuer"],
                        ValidAudience = this.Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:Key"])),
                        RequireSignedTokens = true
                    };
                });

            // Add framework services.
            services.AddMvc();
            services.AddCors();
            services.AddOptions();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new Info
                {
                    Version = "v2",
                    Title = "Intruder DB API",
                    Description = "Public Intruder API",
                    Contact = new Contact { Name = "Christopher 'DukeofSussex' Satchell", Url = "https://intruder.superbossgames.com/forum/memberlist.php?mode=viewprofile&u=23174" }
                });

                string filePath = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "API.xml");
                c.IncludeXmlComments(filePath);

                c.DescribeAllEnumsAsStrings();
            });

            IConfigurationSection dbSettings = this.Configuration.GetSection("DB");
            services.Add(new ServiceDescriptor(typeof(DB), new DB($"server={dbSettings["Host"]};port={dbSettings["Port"]};database={dbSettings["Database"]};user={dbSettings["User"]};password={dbSettings["Password"]};SslMode=none")));

            services.AddSingleton<JobFactory>();
            services.AddSingleton<AgentJobs>();
            services.AddSingleton<ForumMembersJob>();
            services.AddSingleton<ForumPMJob>();
            services.AddSingleton<MapsJob>();
            services.AddSingleton<OnlineAgentsJob>();
            services.AddSingleton<UpdateAgentsJob>();
            services.AddSingleton<ServersJob>();
            services.AddSingleton<IJobManager, JobManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            applicationLifetime.ApplicationStopping.Register(this.OnShutdown);

            loggerFactory.AddNLog();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseLogger();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "docs/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint("v2/swagger.json", "Intruder DB API");
            });
            app.UseAuthentication();
            app.UseMvc();

            Request.Setup();

            this.JobManager = app.ApplicationServices.GetService<IJobManager>();
            this.JobManager.StartAsync();
        }

        private void OnShutdown()
        {
            this.JobManager.StopAsync();
        }
    }
}
