using Egghead.Managers;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Stores;
using Egghead.Validators;
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Errors/ErrorPartial");
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Articles}/{action=Index}/{id?}"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDbOptions"));

            services.AddTransient<IUserStore<MongoDbUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbUserStore<MongoDbUser>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            services.AddTransient<IRoleStore<MongoDbRole>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbRoleStore<MongoDbRole>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            services.AddTransient<IUserValidator<MongoDbUser>, CustomUserValidator<MongoDbUser>>();

            services.AddTransient<IArticlesStore<MongoDbArticle>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbArticlesStore<MongoDbArticle>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            services.AddTransient<IArticlesLikesStore<MongoDbArticleLike>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbArticlesLikesStore<MongoDbArticleLike>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            services.AddTransient<IArticlesCommentsStore<MongoDbArticleComment>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbArticlesCommentsStore<MongoDbArticleComment>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            services.AddTransient<IArticlesViewCountStore<MongoDbArticleViewCount>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbArticlesViewCountStore<MongoDbArticleViewCount>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });

            services.AddIdentity<MongoDbUser, MongoDbRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
            }).AddDefaultTokenProviders().AddUserValidator<CustomUserValidator<MongoDbUser>>();

            services.AddScoped<ArticlesManager<MongoDbArticle>, ArticlesManager<MongoDbArticle>>();
            services.AddScoped<ArticlesLikesManager<MongoDbArticleLike>, ArticlesLikesManager<MongoDbArticleLike>>();
            services.AddScoped<ArticlesCommentsManager<MongoDbArticleComment>, ArticlesCommentsManager<MongoDbArticleComment>>();
            services.AddScoped<ArticlesViewCountManager<MongoDbArticleViewCount>, ArticlesViewCountManager<MongoDbArticleViewCount>>();

            services.AddMvc();
        }
    }
}