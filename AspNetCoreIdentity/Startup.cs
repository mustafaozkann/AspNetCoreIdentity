using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity.TwoFactorServices;
using DataAccess.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreIdentity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TwoFactorOptions>(Configuration.GetSection("TwoFactorOptions"));
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultAzureConnectionString")));

            //services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));

            var lockoutOptions = new LockoutOptions()
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30),
                MaxFailedAccessAttempts = 3
            };

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            }).AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientID"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            //.AddMicrosoftAccount(opts =>
            // {
            //     opts.ClientId = Configuration["Authentication:Microsoft:ClientID"];
            //     opts.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            // })

            //services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
            //  .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            //services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            //{
            //    options.Authority = options.Authority + "/v2.0/";

            //    options.TokenValidationParameters.ValidateIssuer = false;
            //});

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Lockout = lockoutOptions;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._";

                // Password validation
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true; // number between 0-9 
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;

            })
                .AddUserValidator<CustomValidation.CustomUserValidator>()
                .AddPasswordValidator<CustomValidation.CustomPasswordValidator>()
                .AddErrorDescriber<CustomValidation.CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()

                .AddDefaultTokenProviders();

            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "MyBlog";
            cookieBuilder.HttpOnly = false;
            cookieBuilder.SameSite = SameSiteMode.Lax;
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Home/Login");
                options.LogoutPath = new PathString("/Member/LogOut");
                options.Cookie = cookieBuilder;
                options.ExpireTimeSpan = System.TimeSpan.FromDays(60);
                options.SlidingExpiration = true;


            });

            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();


        }
    }
}
