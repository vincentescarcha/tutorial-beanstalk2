using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Shop.Data;
using Shop.Data.Models;
using Shop.Data.Seeds;
using Shop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace tutorial_beanstalk2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // for local
            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MSSqlConnection")));

            // for test env
            string conn = Configuration.GetConnectionString("MySqlConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString: conn, ServerVersion.AutoDetect(conn)));

            services.AddIdentity<ApplicationUser, IdentityRole>(
               options =>
               {
                   options.Password.RequireDigit = false;
                   options.Password.RequiredLength = 6;
                   options.Password.RequireLowercase = false;
                   options.Password.RequireUppercase = false;
                   options.Password.RequireNonAlphanumeric = false;
               }).AddEntityFrameworkStores<ApplicationDbContext>()
               // .AddDefaultUI(Microsoft.AspNetCore.Identity.UI.UIFramework.Bsootstrap4)
               .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(
                options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    options.LogoutPath = new PathString("/Account/Logout");
                });

            services.AddTransient<ICategory, CategoryService>();
            services.AddTransient<IFood, FoodService>();
            services.AddTransient<IOrder, OrderService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(sp => ShoppingCart.GetCart(sp));

            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
            app.UseStaticFiles();

            DbInitializer.Seed(app);

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedRoles.CreateRoles(serviceProvider, Configuration).Wait();
        }
    }
}
