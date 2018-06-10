using Egghead.MongoDbStorage;
using Egghead.MongoDbStorage.Identities;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Egghead
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
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDbOptions"));

            services.AddIdentity<MongoDbIdentityUser, MongoDbIdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";               
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
            }).AddDefaultTokenProviders();

            services.AddTransient<IUserStore<MongoDbIdentityUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbUserStore<MongoDbIdentityUser>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });                  
            
            services.AddTransient<IRoleStore<MongoDbIdentityRole>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbRoleStore<MongoDbIdentityRole>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddMvc();    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error"); 
            }
            else       
            {
                app.UseDeveloperExceptionPage();       
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}