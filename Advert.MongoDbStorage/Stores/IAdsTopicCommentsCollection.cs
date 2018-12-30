using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.AdsTopic;
using Advert.Common.AdsTopics;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public interface IAdsTopicCommentsCollection<T> : IDisposable where T : IAdsTopicComment
    {        
        Task CreateAdsTopicCommentAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdsTopicCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<long> EstimatedAdsTopicCommentsCountAsync(CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicCommentsAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicCommentsAsync(int offset, int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicCommentsAsync(int offset, int? howManyElements, SortDefinition sortDef, CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicCommentsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdsTopicCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<ReplaceOneResult> UpdateAdsTopicCommentByIdAsync(ObjectId commentId, T entity, CancellationToken cancellationToken);
    }
}