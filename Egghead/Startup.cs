using Egghead.MongoDbStorage;
using Egghead.MongoDbStorage.Identity;
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

            services.AddSingleton<IUserStore<MongoDbIdentityUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                var databaseName = options.Value.DatabaseName;
                var connectionString = options.Value.ConnectionString;
                return new MongoDbUserStore<MongoDbIdentityUser>(new MongoClient(connectionString).GetDatabase(databaseName));
            });

            services.AddSingleton<IRoleStore<MongoDbIdentityRole>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                var databaseName = options.Value.DatabaseName;
                var connectionString = options.Value.ConnectionString;
                return new MongoDbRoleStore<MongoDbIdentityRole>(new MongoClient(connectionString).GetDatabase(databaseName));
            });

            services.AddIdentity<MongoDbIdentityUser, MongoDbIdentityRole>().AddDefaultTokenProviders();
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                //Captures synchronous and asynchronous database related exceptions from the pipeline that may be resolved using Entity Framework migrations. When these exceptions occur an HTML response with details of possible actions to resolve the issue is generated.
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //Adds the AuthenticationMiddleware to the specified IApplicationBuilder, which enables authentication capabilities.
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            //Adds MVC to the IApplicationBuilder request execution pipeline with a default route named 'default' and the following template: '{controller=Home}/{action=Index}/{id?}'.
//            app.UseMvcWithDefaultRoute();
        }
    }
}