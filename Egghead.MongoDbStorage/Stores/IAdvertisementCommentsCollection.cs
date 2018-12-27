using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Advertisements;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IAdvertisementCommentsCollection<T> : IDisposable where T : class
    {        
        Task CreateAdvertisementCommentAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdvertisementCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<long> EstimatedAdvertisementCommentsCountAsync(CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementCommentsAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementCommentsAsync(int offset, int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementCommentsAsync(int offset, int? howManyElements, SortDefinition sortDef, CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementCommentsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdvertisementCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<ReplaceOneResult> UpdateAdvertisementCommentByIdAsync(ObjectId commentId, T entity, CancellationToken cancellationToken);
    }
}