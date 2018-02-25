using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Services;
using Trackable.TripDetection;
using Trackable.Web.Auth;
using Trackable.Web.DTOs;
using Trackable.Web.Filters;

namespace Trackable.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Cors
            services
                 .AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                                        .AllowAnyMethod()
                                                                        .AllowAnyHeader()
                                                                        .AllowCredentials()
                                                                        .Build()));

            // Security Key for Jwt
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authorization:SecurityKey"]));
            services.AddSingleton(new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256));

            // Cookie based OpenId Authentication + Jwt Auth for programmatic Apis
            // Use JwtBearer as default to stop automatic redirect
            services
                .AddAuthentication(cfg =>
                {
                    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    cfg.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(cookieopt =>
                {
                    cookieopt.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                    cookieopt.Events.OnRedirectToLogin = DoNotRedirectApiCalls;
                })
                .AddOpenIdConnect(options =>
                {
                    options.ClientId = Configuration["Authorization:ClientId"];
                    options.ClientSecret = Configuration["Authorization:ClientSecret"];
                    options.Authority = "https://login.microsoftonline.com/common/v2.0";
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.CallbackPath = "/signin-oidc";
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRemoteFailure = OnAuthenticationFailed,
                        OnRedirectToIdentityProvider = DoNotRedirectApiCalls,

                    };
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false
                    };
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jwtKey,
                        ValidateIssuer = true,
                        ValidIssuer = JwtAuthConstants.Issuer,
                        ValidateAudience = true,
                        ValidAudiences = new[] { JwtAuthConstants.DeviceAudience, JwtAuthConstants.UserAudience, JwtAuthConstants.RegistrationAudience },
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5),
                    };
                });

            // Authorization
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy(UserRoles.Blocked, policy => policy.Requirements.Add(new RoleRequirement(UserRoles.Blocked)));
                    options.AddPolicy(UserRoles.Pending, policy => policy.Requirements.Add(new RoleRequirement(UserRoles.Pending)));
                    options.AddPolicy(UserRoles.DeviceRegistration, policy => policy.Requirements.Add(new RoleRequirement(UserRoles.DeviceRegistration)));
                    options.AddPolicy(UserRoles.TrackingDevice, policy => policy.Requirements.Add(new RoleRequirement(UserRoles.TrackingDevice)));
                    options.AddPolicy(UserRoles.Viewer, policy => policy.Requirements.Add(new RoleRequirement(UserRoles.Viewer)));
                    options.AddPolicy(UserRoles.Administrator, policy => policy.Requirements.Add(new RoleRequirement(UserRoles.Administrator)));
                    options.AddPolicy(UserRoles.Owner, policy => policy.Requirements.Add(new RoleRequirement(UserRoles.Owner)));
                });

            // Business Logic Services
            services
                .AddServices(
                    Configuration.GetConnectionString("DefaultConnection"),
                    this.HostingEnvironment.WebRootPath)
                .AddTransient<IAuthorizationHandler, RoleRequirementHandler>()
                .AddScoped<ExceptionHandlerFilter>()
                .AddTripDetection(Configuration["SubscriptionKeys:BingMaps"])
                .AddSingleton<IHostedService, HostedInstrumentationService>()
                .AddSingleton<Profile, DtoMappingProfile>();

            // Add AutoMapper profiles
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                foreach (var mapperProfile in services.Where(s => s.ServiceType == typeof(AutoMapper.Profile)))
                {
                    var type = mapperProfile.ImplementationType;
                    cfg.AddProfile(type);
                }
            });
            mapperConfiguration.CompileMappings();
            services.AddScoped<IMapper>(ctx => new Mapper(mapperConfiguration, t => ctx.GetService(t)));

            if (Configuration.GetValue<bool>("Serving:ServeSwagger"))
            {
                // Register the Swagger generator, defining one or more Swagger documents
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Trackable APIs", Version = "v1" });
                    c.DescribeAllEnumsAsStrings();
                    c.DescribeAllParametersInCamelCase();
                    c.DocInclusionPredicate((version, description) =>
                    {
                        return description.RelativePath.StartsWith("api");
                    });
                });
            }

            // Add MVC
            services
                .AddMvc(options =>
                {
                    // Add Exception Handler
                    options.Filters.Add(typeof(ExceptionHandlerFilter));

                    // Add Https require filter if not running locally
                    if (!this.Configuration.GetValue<bool>("Serving:IsDebug"))
                    {
                        options.Filters.Add(new RequireHttpsAttribute());
                    }
                });

            // Add socket management
            services
                .AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider seriveProvider)
        {
            // Cors
            app.UseCors("AllowAll");

            // Authentication
            app.UseAuthentication();

            // Logging
            loggerFactory.AddAzureWebAppDiagnostics();
            app.SetHeavyDebugEnabled(Configuration.GetValue<bool>("Logging:HeavyDebugLogging"));

            // Business Logic
            app.UseServices(Configuration.GetConnectionString("DefaultConnection"),
                Configuration["Authorization:OwnerEmail"]);

            // Socket Management
            app.UseSignalR(routes =>
            {
                routes.MapHub<DynamicHub>("deviceAddition");
            });

            // Static files setup for angular website
            if (Configuration.GetValue<bool>("Serving:ServeFrontend") || Configuration.GetValue<bool>("Serving:ServeSwagger"))
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dist")),
                    RequestPath = new PathString("")
                });
            }

            // Rewrite to Https if not running locally
            if (!this.Configuration.GetValue<bool>("Serving:IsDebug"))
            {
                var options = new RewriteOptions()
                    .AddRedirectToHttpsPermanent();

                app.UseRewriter(options);
            }

            if (Configuration.GetValue<bool>("Serving:ServeSwagger"))
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trackable APIs V1");
                    c.ShowJsonEditor();
                    c.ShowRequestHeaders();
                    c.InjectOnCompleteJavaScript("/swagger/swagger.js");
                });
            }

            app.UseMvc();
        }

        // Handle sign-in errors differently than generic errors.
        private Task OnAuthenticationFailed(RemoteFailureContext context)
        {
            context.HandleResponse();
            context.Response.Redirect("/api/users/accessdenied");
            return Task.FromResult(0);
        }

        private Task DoNotRedirectApiCalls<T>(PropertiesContext<T> context) where T : AuthenticationSchemeOptions
        {
            if (!context.Request.Path.StartsWithSegments(new PathString("/api/users/login")))
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
            }

            return Task.CompletedTask;
        }
    }
}
