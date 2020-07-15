using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Auther.WebApi
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
            services.AddControllers();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Slack";
            })
            .AddCookie()
            .AddOAuth("Slack",
                options =>
                {
                    options.AuthorizationEndpoint = "https://slack.com/oauth/authorize";
                    options.Scope.Add("identity.basic");
                    options.Scope.Add("identity.avatar");

                    options.CallbackPath = new PathString("/authorization-code/callback");
                    
                    options.ClientId = Configuration.GetValue<string>("Slack:ClientId");
                    options.ClientSecret = Configuration.GetValue<string>("Slack:ClientSecret");
                    options.TokenEndpoint = "https://slack.com/api/oauth.access";

                    options.UserInformationEndpoint = "https://slack.com/api/users.identity";
                    options.ClaimActions.MapJsonSubKey(ClaimTypes.NameIdentifier, "user", "id");
                    options.ClaimActions.MapJsonSubKey(ClaimTypes.Name, "user", "name");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

                            context.RunClaimActions(user);
                        }
                    };
                }
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
