using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Stores;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Xunit;

namespace Egghead.IntegrationTests.Stores
{
    public class MongoDbArticlesCommentsStoreTestFixture : IDisposable
    {
        private readonly IMongoDatabase _mongoDatabase;
        
        private readonly CancellationToken _cancellationToken = CancellationToken.None;
        
        private readonly IArticlesCommentsStore<MongoDbArticleComment> _articlesCommentsStore;

        public MongoDbArticlesCommentsStoreTestFixture()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            
            IConfiguration configuration = builder.Build();

            _mongoDatabase = new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]);
          
            _articlesCommentsStore = new MongoDbArticlesCommentsStore<MongoDbArticleComment>(_mongoDatabase);
        }
  
        [Fact]
        public async Task CreateArticleComment()
        {
            var articleComment = new MongoDbArticleComment
            {
                Text = "Test comment",
                ByWho = "victor.chicu@hotmail.com",
                ByWhoNormalized = "victor.chicu@hotmail.com".ToUpper(),
                ReplyTo = null,
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };

            var articleCommentCreationResult = await _articlesCommentsStore.CreateArticleCommentAsync(articleComment, _cancellationToken);
            
            Assert.Equal(OperationResult.Success, articleCommentCreationResult);         
            
            Assert.NotNull(await _articlesCommentsStore.FindArticleCommentById(articleComment.Id, _cancellationToken));
        }

        public void Dispose()
        {
            _mongoDatabase.DropCollection(MongoDbCollections.ArticlesComments, _cancellationToken);
        }
    }
}