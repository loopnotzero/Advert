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
    public class MongoDbArticlesCommentsStoreTestFixture : IDisposable
    {
        private readonly string _articleCommentsCollectionName;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        private readonly IArticlesCommentsStore<MongoDbArticleComment> _articlesCommentsStore;
        private readonly IArticleCommentsCollection<MongoDbArticleComment> _articleCommentsCollection;

        public MongoDbArticlesCommentsStoreTestFixture()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            _articlesCommentsStore = new MongoDbArticlesCommentsStore<MongoDbArticleComment>(new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]));

            IArticlesStore<MongoDbArticle> articlesStore = new MongoDbArticlesStore<MongoDbArticle>(new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]));

            var article = new MongoDbArticle();

            _articleCommentsCollectionName = article.Id;

            articlesStore.CreateArticleAsync(article, _cancellationToken).GetAwaiter().GetResult();

            _articlesCommentsStore = new MongoDbArticlesCommentsStore<MongoDbArticleComment>(new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]));

            _articleCommentsCollection = _articlesCommentsStore.CreateArticleCommentsCollection(_articleCommentsCollectionName, _cancellationToken);
        }

        public void Dispose()
        {
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(_articleCommentsCollectionName, _cancellationToken));
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(_articleCommentsCollectionName, _cancellationToken));
        }

        [Fact]
        public void GetExistingArticleCommentsCollection()
        {
            var collectionName = ObjectId.GenerateNewId().ToString();
            var newArticlesCommentsCollection = _articlesCommentsStore.CreateArticleCommentsCollection(collectionName, _cancellationToken);
            Assert.NotNull(newArticlesCommentsCollection);
            Assert.Equal(OperationResult.Success, newArticlesCommentsCollection.CreateArticleCommentAsync(new MongoDbArticleComment(), _cancellationToken).GetAwaiter().GetResult());
            var articlesCommentsCollection = _articlesCommentsStore.GetArticleCommentsCollection(collectionName, _cancellationToken);
            Assert.Equal(1, articlesCommentsCollection.EstimatedArticleCommentsCountAsync(_cancellationToken).GetAwaiter().GetResult());
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(collectionName, _cancellationToken));
        }

        [Fact]
        public void GetNotExistingArticleCommentsCollection()
        {
            var collectionName = ObjectId.GenerateNewId().ToString();
            var articlesCommentsCollection = _articlesCommentsStore.GetArticleCommentsCollection(collectionName, _cancellationToken);
            Assert.Equal(0, articlesCommentsCollection.EstimatedArticleCommentsCountAsync(_cancellationToken).GetAwaiter().GetResult());
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(collectionName, _cancellationToken));
        }

        [Fact]
        public void CreateArticleCommentsCollectionAssertThatCollectionExists()
        {
            var collectionName = ObjectId.GenerateNewId().ToString();
            Assert.NotNull(_articlesCommentsStore.CreateArticleCommentsCollection(collectionName, _cancellationToken));
            Assert.True(_articlesCommentsStore.ArticleCommentsCollectionExists(collectionName, _cancellationToken));
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(collectionName, _cancellationToken));
        }

        [Fact]
        public void DeleteArticleCommentsCollectionAssertThatCollectionNotExists()
        {
            var collectionName = ObjectId.GenerateNewId().ToString();
            Assert.NotNull(_articlesCommentsStore.CreateArticleCommentsCollection(collectionName, _cancellationToken));
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(collectionName, _cancellationToken));
            Assert.False(_articlesCommentsStore.ArticleCommentsCollectionExists(collectionName, _cancellationToken));
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(collectionName, _cancellationToken));
        }

        [Fact]
        public async Task CreateArticleComment()
        {
            var newArticleComment = new MongoDbArticleComment
            {
                Text = "New comment",
                ByWho = "fake@email.com",
                ByWhoNormalized = "fake@email.com".ToUpper(),
                ReplyTo = ObjectId.Empty.ToString(),
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };

            var articleCommentCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var articleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentByIdAsync(newArticleComment.Id, _cancellationToken);
            Assert.NotNull(articleCommentFindingResult);
        }

        [Fact]
        public async Task ChangeArticleComment()
        {
            var newArticleComment = new MongoDbArticleComment
            {
                Text = "New comment",
                ByWho = "fake@email.com",
                ByWhoNormalized = "fake@email.com".ToUpper(),
                ReplyTo = ObjectId.Empty.ToString(),
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };

            var articleCommentCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var articleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentByIdAsync(newArticleComment.Id, _cancellationToken);
            articleCommentFindingResult.Text = "Changed comment";

            var articleCommentChangingResult = await _articleCommentsCollection.UpdateArticleCommentByIdAsync(articleCommentFindingResult.Id, articleCommentFindingResult, _cancellationToken);
            Assert.Equal(OperationResult.Success, articleCommentChangingResult);

            var changedArticleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentByIdAsync(articleCommentFindingResult.Id, _cancellationToken);

            Assert.NotNull(articleCommentFindingResult);
        }

        [Fact]
        public async Task DeleteArticleComment()
        {
            var newArticleComment = new MongoDbArticleComment
            {
                Text = "New comment",
                ByWho = "fake@email.com",
                ByWhoNormalized = "fake@email.com".ToUpper(),
                ReplyTo = ObjectId.Empty.ToString(),
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };

            var articleCommentCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var articleCommentDeletingResult = await _articleCommentsCollection.DeleteArticleCommentByIdAsync(newArticleComment.Id, _cancellationToken);
            Assert.Equal(OperationResult.Success, articleCommentDeletingResult);

            var articleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentByIdAsync(newArticleComment.Id, _cancellationToken);
            Assert.Null(articleCommentFindingResult);
        }
    }
}