using System;
using System.IO;
using System.Threading;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Stores;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Egghead.IntegrationTests.Stores
{
    public class MongoDbArticlesLikesStoreTestFixture : IDisposable 
    { 
        private readonly CancellationToken _cancellationToken = CancellationToken.None; 
        private readonly IArticlesVotesStore<MongoDbArticleVote> _articlesVotesStore; 
         
        public MongoDbArticlesLikesStoreTestFixture() 
        { 
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");            
             
            var configuration = builder.Build(); 
             
            _articlesVotesStore = new MongoDbArticlesVotesStore<MongoDbArticleVote>(new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"])); 
        } 
         
        public void Dispose() 
        { 
             
        } 
    } 
}