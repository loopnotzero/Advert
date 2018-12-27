using Egghead.Managers;
using Egghead.MongoDbStorage.Advertisements;
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

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Advertisements}/{action=GetAdvertisements}/{id?}"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDatabase"));
            
            #region Scoped services
            services.AddScoped<ProfilesManager<MongoDbProfile>, ProfilesManager<MongoDbProfile>>();
            services.AddScoped<AdvertisementsManager<MongoDbAdvertisement>, AdvertisementsManager<MongoDbAdvertisement>>();
            services.AddScoped<AdvertisementsLikesManager<MongoDbAdvertisementVote>, AdvertisementsLikesManager<MongoDbAdvertisementVote>>();
            services.AddScoped<ProfilesImagesManager<MongoDbProfileImage>, ProfilesImagesManager<MongoDbProfileImage>>();
            services.AddScoped<AdvertisementsCommentsManager<MongoDbAdvertisementComment>, AdvertisementsCommentsManager<MongoDbAdvertisementComment>>();
            services.AddScoped<AdvertisementsViewCountManager<MongoDbAdvertisementViewsCount>, AdvertisementsViewCountManager<MongoDbAdvertisementViewsCount>>();
            services.AddScoped<AdvertisementCommentsVotesManager<MongoDbAdvertisementCommentVote>, AdvertisementCommentsVotesManager<MongoDbAdvertisementCommentVote>>(); 
            services.AddScoped<AdvertisementCommentsVotesAggregationManager<MongoDbAdvertisementCommentVote, MongoDbAdvertisementCommentVoteAggregation>, AdvertisementCommentsVotesAggregationManager<MongoDbAdvertisementCommentVote, MongoDbAdvertisementCommentVoteAggregation>>(); 
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

            services.AddTransient<IAdvertisementsStore<MongoDbAdvertisement>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdvertisementsStore<MongoDbAdvertisement>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
             
            services.AddTransient<IAdvertisementsVotesStore<MongoDbAdvertisementVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdvertisementsVotesStore<MongoDbAdvertisementVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IProfilesImagesStore<MongoDbProfileImage>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbProfilesImagesStore<MongoDbProfileImage>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IAdvertisementsCommentsStore<MongoDbAdvertisementComment>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdvertisementsCommentsStore<MongoDbAdvertisementComment>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });

            services.AddTransient<IAdvertisementsViewCountStore<MongoDbAdvertisementViewsCount>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdvertisementsViewCountStore<MongoDbAdvertisementViewsCount>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IAdvertisementCommentsVotesStore<MongoDbAdvertisementCommentVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdvertisementCommentsVotesStore<MongoDbAdvertisementCommentVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
                          
            services.AddTransient<IAdvertisementCommentsVotesAggregationStore<MongoDbAdvertisementCommentVote, MongoDbAdvertisementCommentVoteAggregation>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbAdvertisementCommentsVotesAggregationStore<MongoDbAdvertisementCommentVote, MongoDbAdvertisementCommentVoteAggregation>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IUserValidator<MongoDbUser>, EggheadUserValidator<MongoDbUser>>();
               
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
            }).AddDefaultTokenProviders().AddUserValidator<EggheadUserValidator<MongoDbUser>>();
            
            #endregion
            
            services.AddMvc();
        }
    }
}