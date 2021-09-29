// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Trackable.Common;
using Trackable.Services;
using Trackable.TripDetection;
using Trackable.Web.Auth;
using Trackable.Web.Dtos;
using Trackable.Web.Filters;

namespace Trackable.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Cors
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
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
                    options.Authority = Configuration["Authorization:Authority"];
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
            services.AddServices(
                    Configuration.GetConnectionString("DefaultConnection"),
                    Environment.WebRootPath)
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
                    c.SwaggerDoc("v1", new()
                    {
                        Title = "BMFT APIs",
                        Version = "v1",
                        Description = "Bing Maps Fleet Tracker is an open source fleet tracking solution. Read more at https://github.com/Microsoft/Bing-Maps-Fleet-Tracker",
                        License = new OpenApiLicense { Name = "MIT Licencse", Url = new Uri("https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/LICENSE") }
                    });

                    c.DescribeAllParametersInCamelCase();
                    c.DocInclusionPredicate((version, description) =>
                    {
                        return description.RelativePath.StartsWith("api");
                    });

                    var filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Trackable.Web.xml");
                    c.IncludeXmlComments(filePath);
                });
            }

            // Add MVC
            services
                .AddMvc(options =>
                {
                    // Add Exception Handler
                    options.Filters.Add(typeof(ExceptionHandlerFilter));

                    // Add Https require filter if not running locally
                    if (!Configuration.GetValue<bool>("Serving:IsDebug"))
                    {
                        options.Filters.Add(new RequireHttpsAttribute());
                    }
                });

            // Add socket management
            services
                .AddSignalR();

            services
                .AddApplicationInsightsTelemetry(Configuration.GetConnectionString("ApplicationInsights"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Cors
            app.UseCors("AllowAll");

            // Authentication
            app.UseAuthentication();

            // Logging
            app.SetHeavyDebugEnabled(Configuration.GetValue<bool>("Logging:HeavyDebugLogging"));

            // Business Logic
            app.UseServices(Configuration.GetConnectionString("DefaultConnection"),
                Configuration["Authorization:OwnerEmail"]);

            // Socket Management
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<DynamicHub>("deviceAddition");
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
            if (!Configuration.GetValue<bool>("Serving:IsDebug"))
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BMFT APIs V1");
                    c.HeadContent = "<script type=\"application/javascript\" src=\"/swagger/swagger.js\"></script><link rel=\"stylesheet\" href=\"/swagger/swagger.css\">";
                    c.DocumentTitle = "BMFT Swagger";
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