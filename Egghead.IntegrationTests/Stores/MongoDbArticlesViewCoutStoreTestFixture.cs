using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Stores;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Egghead.IntegrationTests.Stores
{
    public class MongoDbArticlesViewCoutStoreTestFixture : IDisposable
    {
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        private readonly IArticlesViewCountStore<MongoDbArticleViewCount> _articlesViewCount;

        public MongoDbArticlesViewCoutStoreTestFixture()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            _articlesViewCount = new MongoDbArticlesViewCountStore<MongoDbArticleViewCount>(new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]));
        }

        public void Dispose()
        {
        }

        [Fact]
        public async Task AggregateArticlesWithLargestViewsCount()
        {
            var articleId = ObjectId.GenerateNewId();

            for (var n = 0; n < 10; n++)
            {
                var operationResult =  await _articlesViewCount.CreateArticleViewsCountAsync(new MongoDbArticleViewCount
                {
                    ByWho = "AggregateArticlesWithLargestViewsCount",
                    ByWhoNormalized = "AggregateArticlesWithLargestViewsCount".ToUpper(),
                    ArticleId = articleId,
                    CreatedAt = DateTime.UtcNow               
                }, _cancellationToken);  
                Assert.Equal(OperationResult.Success, operationResult);
            }
                      
            var articlesWithLargestViewsCount = await _articlesViewCount.AsQueryable(_cancellationToken);

//            Assert.Equal(articlesWithLargestViewsCount.Count, 5);
        }
    }
}