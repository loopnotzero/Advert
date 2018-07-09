using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.Stores;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Egghead.IntegrationTests.Stores
{
    public class MongoDbArticlesCommentsStoreTestFixture : IDisposable
    {
        private readonly string _articleCommentsCollectionName = ObjectId.GenerateNewId().ToString();
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        private readonly IArticlesCommentsStore<MongoDbArticleComment> _articlesCommentsStore;
        private readonly IArticleCommentsCollection<MongoDbArticleComment> _articleCommentsCollection;

        public MongoDbArticlesCommentsStoreTestFixture()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");           
            
            var configuration = builder.Build();
               
            _articlesCommentsStore = new MongoDbArticlesCommentsStore<MongoDbArticleComment>( new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]));
            
            _articleCommentsCollection = _articlesCommentsStore.CreateArticleCommentsCollection(_articleCommentsCollectionName, _cancellationToken);
        }
        
        
        public void Dispose()
        {
            Assert.Equal(OperationResult.Success, _articlesCommentsStore.DeleteArticleCommentsCollection(_articleCommentsCollectionName, _cancellationToken));
        }
        
        
        [Fact]
        public async Task CreateArticleComment()
        {
            var newArticleComment = new MongoDbArticleComment
            {
                Text = "New comment",
                ByWho = "fake@email.com",
                ByWhoNormalized = "fake@email.com".ToUpper(),
                ReplyTo = null,
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };
            
            var articleCommentCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);          
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);
            
            var articleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentById(newArticleComment.Id, _cancellationToken);           
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
                ReplyTo = null,
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };

            var articleCommentCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);           
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var articleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentById(newArticleComment.Id, _cancellationToken);
            articleCommentFindingResult.Text = "Changed comment";
            
            var articleCommentChangingResult = await _articleCommentsCollection.UpdateArticleCommentByIdAsync(articleCommentFindingResult.Id, articleCommentFindingResult, _cancellationToken);                    
            Assert.Equal(OperationResult.Success, articleCommentChangingResult);

            var changedArticleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentById(articleCommentFindingResult.Id, _cancellationToken);           
            Assert.NotNull(changedArticleCommentFindingResult);
            
            Assert.Equal(articleCommentFindingResult.Text, changedArticleCommentFindingResult.Text);
        }

        [Fact]
        public async Task DeleteArticleComment()
        {
            var newArticleComment = new MongoDbArticleComment
            {
                Text = "New comment",
                ByWho = "fake@email.com",
                ByWhoNormalized = "fake@email.com".ToUpper(),
                ReplyTo = null,
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };

            var articleCommentCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);           
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var articleCommentDeletingResult = await _articleCommentsCollection.DeleteArticleCommentByIdAsync(newArticleComment.Id, _cancellationToken);
            Assert.Equal(OperationResult.Success, articleCommentDeletingResult);

            var articleCommentFindingResult = await _articleCommentsCollection.FindArticleCommentById(newArticleComment.Id, _cancellationToken);
            Assert.Null(articleCommentFindingResult);
        }

        [Fact]
        public async Task CreateArticleCommentWithReply()
        {
            var newArticleComment = new MongoDbArticleComment
            {
                Text = "New comment",
                ByWho = "fake@email.com",
                ByWhoNormalized = "fake@email.com".ToUpper(),
                ReplyTo = null,
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };
            
            var articleCommentCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);          
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var newArticleCommentReply = new MongoDbArticleComment
            {
                Text = "Comment reply",
                ByWho = "fake@email.com",
                ByWhoNormalized = "fake@email.com".ToUpper(),
                ReplyTo = newArticleComment.Id,
                Depth = 1,
                CreatedAt = DateTime.UtcNow
            };
            
            var articleCommentReplyCreatingResult = await _articleCommentsCollection.CreateArticleCommentAsync(newArticleCommentReply, _cancellationToken);             
            Assert.Equal(OperationResult.Success, articleCommentReplyCreatingResult);

            var articleComments = await _articleCommentsCollection.GetArticleComments(_cancellationToken);
            Assert.Equal(2, articleComments.Count);
        }
    } 
}