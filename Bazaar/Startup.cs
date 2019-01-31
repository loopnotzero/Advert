using Advert.Managers;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Posts;
using Bazaar.MongoDbStorage.Common;
using Bazaar.MongoDbStorage.Profiles;
using Bazaar.MongoDbStorage.Roles;
using Bazaar.MongoDbStorage.Stores;
using Bazaar.MongoDbStorage.Users;
using Bazaar.Managers;
using Bazaar.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Bazaar
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

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Posts}/{action=GetPosts}/{id?}"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDatabase"));
            
            #region Scoped services
            
            services.AddScoped<PostsManager<MongoDbPost>>();
            services.AddScoped<PostsVotesManager<MongoDbPostVote>>();
            services.AddScoped<PostCommentsManager<MongoDbPostComment>>();
            services.AddScoped<PostsViewsCountManager<MongoDbPostViewsCount>>();
            services.AddScoped<PostCommentsVotesManager<MongoDbPostCommentVote>>(); 
            services.AddScoped<ProfilesManager<MongoDbProfile>>();
            services.AddScoped<ProfilesImagesManager<MongoDbProfileImage>>();
            
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
          
            services.AddTransient<IPostsStore<MongoDbPost>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsStore<MongoDbPost>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
             
            services.AddTransient<IPostsVotesStore<MongoDbPostVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsVotesStore<MongoDbPostVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            
            services.AddTransient<IPostsCommentsStore<MongoDbPostComment>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsCommentsStore<MongoDbPostComment>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });

            services.AddTransient<IPostsViewCountStore<MongoDbPostViewsCount>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsViewCountStore<MongoDbPostViewsCount>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IPostCommentsVotesStore<MongoDbPostCommentVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostCommentsVotesStore<MongoDbPostCommentVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
  
            services.AddTransient<IProfilesStore<MongoDbProfile>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbProfilesStore<MongoDbProfile>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddTransient<IProfilesImagesStore<MongoDbProfileImage>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbProfilesImagesStore<MongoDbProfileImage>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
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
            
            BsonClassMap.RegisterClassMap<MongoDbUser>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(x => x._id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);              
            });
        }
    }
}