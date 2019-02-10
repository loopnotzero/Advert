using System;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Posts;
using Bazaar.MongoDbStorage.Common;
using Bazaar.MongoDbStorage.Profiles;
using Bazaar.MongoDbStorage.Roles;
using Bazaar.MongoDbStorage.Stores;
using Bazaar.MongoDbStorage.Users;
using Bazaar.Services;
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

            services.AddScoped<PostCommentsService<MongoDbPostComment>>();
            services.AddScoped<PostCommentsVotesService<MongoDbPostCommentVote>>(); 
            services.AddScoped<PostsService<MongoDbPost>>();
            services.AddScoped<PostsPhotosService<MongoDbPostPhoto>>();
            services.AddScoped<PostsViewsCountService<MongoDbPostViewsCount>>();
            services.AddScoped<PostsVotesService<MongoDbPostVote>>();
            services.AddScoped<ProfilesService<MongoDbProfile>>();
            
            services.AddSingleton<IPostCommentsVotesStore<MongoDbPostCommentVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostCommentsVotesStore<MongoDbPostCommentVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
                       
            services.AddSingleton<IPostsCommentsStore<MongoDbPostComment>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsCommentsStore<MongoDbPostComment>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddSingleton<IPostsPhotosStore<MongoDbPostPhoto>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();   

                var postsPhotosStore = new MongoDbPostsPhotosStore<MongoDbPostPhoto>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));

                postsPhotosStore.CreateDefaultIndexes();
                
                return postsPhotosStore;
            });
            
            services.AddSingleton<IPostsStore<MongoDbPost>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsStore<MongoDbPost>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
                       
            services.AddSingleton<IPostsViewCountStore<MongoDbPostViewsCount>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsViewCountStore<MongoDbPostViewsCount>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
              
            services.AddSingleton<IPostsVotesStore<MongoDbPostVote>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbPostsVotesStore<MongoDbPostVote>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddSingleton<IProfilesStore<MongoDbProfile>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbProfilesStore<MongoDbProfile>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddSingleton<IRoleStore<MongoDbRole>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbRoleStore<MongoDbRole>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddSingleton<IUserStore<MongoDbUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbOptions>>();
                return new MongoDbUserStore<MongoDbUser>(new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName));
            });
            
            services.AddSingleton<IUserValidator<MongoDbUser>, AdvertUserValidator<MongoDbUser>>();
            
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
            
            services.AddMvc();
            
            BsonClassMap.RegisterClassMap<MongoDbUser>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(x => x._id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);              
            });
        }
    }
}