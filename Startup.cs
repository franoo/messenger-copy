#region snippet_all
// Unused usings removed
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Authentication;
using WebApi.Services;
using Microsoft.AspNetCore.Http;
using WebApi.Hubs;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.SignalR;
using WebApi.Middleware;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyDBContext>(opt =>
                                               opt.UseInMemoryDatabase(databaseName:"Messenger"));
            //services.GetRequiredService<MyDBContext>();
            //.Initialize(services);
            services.AddControllers();
            //services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();

            services.AddCors(options => 
                options.AddPolicy("CorsPolicy", builder =>//alow angular to log in
            {
                builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }));
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration["Jwt:Issuer"],
            //        ValidAudience = Configuration["Jwt:Issuer"],
            //        IssuerSigningKey = new
            //        SymmetricSecurityKey
            //        (Encoding.UTF8.GetBytes
            //        (Configuration["Jwt:Key"]))
            //    };
            //});
            //signalR
            services.AddSignalR();


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new
                            SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes
                            (Configuration["Jwt:Key"]))
                };
                //comment 
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chatsocket")))
                        {
                            // Read the token out of the query string

                            ///looool token is empty in context
                            ///
                            context.Token = accessToken;
                            //Console.WriteLine(context.Token);
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            //new shit
            //services.AddSingleton<IUserIdProvider, UserIdProvider>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
                            
            }
            //CORS 
            
            //app.UsePreflightRequestHandler();
            //
            //app.UseHttpsRedirection();
            
            

            app.UseRouting();
            app.UseWebSockets();
            app.UseCors("CorsPolicy");
            app.UseMiddleware<WebSocketsMiddleware>();
            app.UseAuthentication();

            app.UseAuthorization();
            //app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                //signalR https://localhost:5000/chatsocket 
                endpoints.MapHub<ChatHub>("/chatsocket");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
#endregion