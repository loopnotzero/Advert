using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Common;
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
        private readonly IArticlesCommentsCollection<MongoDbArticleComment> _articlesCommentsCollection;

        public MongoDbArticlesCommentsStoreTestFixture()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");           
            
            var configuration = builder.Build();
               
            _articlesCommentsStore = new MongoDbArticlesCommentsStore<MongoDbArticleComment>( new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]));
            
            _articlesCommentsCollection = _articlesCommentsStore.CreateArticleCommentsCollection(_articleCommentsCollectionName, _cancellationToken);
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
                ByWhoNormalized = "vfake@email.com".ToUpper(),
                ReplyTo = null,
                Depth = 0,
                CreatedAt = DateTime.UtcNow
            };
            
            var articleCommentCreatingResult = await _articlesCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);          
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);
            
            var articleCommentFindingResult = await _articlesCommentsCollection.FindArticleCommentById(newArticleComment.Id, _cancellationToken);           
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

            var articleCommentCreatingResult = await _articlesCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);           
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var articleCommentFindingResult = await _articlesCommentsCollection.FindArticleCommentById(newArticleComment.Id, _cancellationToken);
            articleCommentFindingResult.Text = "Changed comment";
            
            var articleCommentChangingResult = await _articlesCommentsCollection.UpdateArticleCommentByIdAsync(articleCommentFindingResult.Id, articleCommentFindingResult, _cancellationToken);                    
            Assert.Equal(OperationResult.Success, articleCommentChangingResult);

            var changedArticleCommentFindingResult = await _articlesCommentsCollection.FindArticleCommentById(articleCommentFindingResult.Id, _cancellationToken);           
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

            var articleCommentCreatingResult = await _articlesCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);           
            Assert.Equal(OperationResult.Success, articleCommentCreatingResult);

            var articleCommentDeletingResult = await _articlesCommentsCollection.DeleteArticleCommentByIdAsync(newArticleComment.Id, _cancellationToken);
            Assert.Equal(OperationResult.Success, articleCommentDeletingResult);

            var articleCommentFindingResult = await _articlesCommentsCollection.FindArticleCommentById(newArticleComment.Id, _cancellationToken);
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
            
            var articleCommentCreatingResult = await _articlesCommentsCollection.CreateArticleCommentAsync(newArticleComment, _cancellationToken);          
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
            
            var articleCommentReplyCreatingResult = await _articlesCommentsCollection.CreateArticleCommentAsync(newArticleCommentReply, _cancellationToken);             
            Assert.Equal(OperationResult.Success, articleCommentReplyCreatingResult);

            var articleComments = await _articlesCommentsCollection.GetArticleComments(_cancellationToken);
            Assert.Equal(2, articleComments.Count);
        }
    } 
}