using Egghead.Managers;
using Egghead.MongoDbStorage.AdsTopics;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Roles;
using Egghead.MongoDbStorage.Stores;
using Egghead.MongoDbStorage.Users;
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

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=AdsTopics}/{action=GetAdsTopics}/{id?}"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDatabase"));
            
            #region Scoped services
            services.AddScoped<ProfilesManager<MongoDbProfile>, ProfilesManager<MongoDbProfile>>();
            services.AddScoped<AdsTopicsManager<MongoDbAdsTopic>, AdsTopicsManager<MongoDbAdsTopic>>();
            services.AddScoped<AdsTopicsVotesManager<MongoDbAdsTopicVote>, AdsTopicsVotesManager<MongoDbAdsTopicVote>>();
            services.AddScoped<ProfilesImagesManager<MongoDbProfileImage>, ProfilesImagesManager<MongoDbProfileImage>>();
            services.AddScoped<AdsTopicCommentsManager<MongoDbAdsTopicComment>, AdsTopicCommentsManager<MongoDbAdsTopicComment>>();
            services.AddScoped<AdsTopicsViewsCountManager<MongoDbAdsTopicViewsCount>, AdsTopicsViewsCountManager<MongoDbAdsTopicViewsCount>>();
            services.AddScoped<AdsTopicCommentsVotesManager<MongoDbAdsTopicCommentVote>, AdsTopicCommentsVotesManager<MongoDbAdsTopicCommentVote>>(); 
            services.AddScoped<AdsTopicCommentsVotesAggregationManager<MongoDbAdsTopicCommentVote, MongoDbAdsTopicCommentVoteAggregation>, AdsTopicCommentsVotesAggregationManager<MongoDbAdsTopicCommentVote, MongoDbAdsTopicCommentVoteAggregation>>(); 
            #endregion

            #region Transient services
            
            services.AddTransient<IRoleStore<MongoDbRole>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbRoleStore<MongoDbRole>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IUserStore<MongoDbUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbUserStore<MongoDbUser>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
          
            services.AddTransient<IProfilesStore<MongoDbProfile>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbProfilesStore<MongoDbProfile>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });

            services.AddTransient<IAdsTopicsStore<MongoDbAdsTopic>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdsTopicsStore<MongoDbAdsTopic>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
             
            services.AddTransient<IAdsTopicsVotesStore<MongoDbAdsTopicVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdsTopicsVotesStore<MongoDbAdsTopicVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IProfilesImagesStore<MongoDbProfileImage>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbProfilesImagesStore<MongoDbProfileImage>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IAdsTopicsCommentsStore<MongoDbAdsTopicComment>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdsTopicsCommentsStore<MongoDbAdsTopicComment>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });

            services.AddTransient<IAdsTopicsViewCountStore<MongoDbAdsTopicViewsCount>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdsTopicsViewCountStore<MongoDbAdsTopicViewsCount>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IAdsTopicCommentsVotesStore<MongoDbAdsTopicCommentVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdsTopicCommentsVotesStore<MongoDbAdsTopicCommentVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
                          
            services.AddTransient<IAdsTopicCommentsVotesAggregationStore<MongoDbAdsTopicCommentVote, MongoDbAdsTopicCommentVoteAggregation>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdsTopicCommentsVotesAggregationStore<MongoDbAdsTopicCommentVote, MongoDbAdsTopicCommentVoteAggregation>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IUserValidator<MongoDbUser>, AdvertUserValidator<MongoDbUser>>();
               
            #endregion
            
            #region Identity services
            
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
            }).AddDefaultTokenProviders().AddUserValidator<AdvertUserValidator<MongoDbUser>>();
            
            #endregion
            
            services.AddMvc();
        }
    }
}