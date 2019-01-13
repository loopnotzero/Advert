using System.Collections.Generic;
using Advert.Models.Profiles;

namespace Advert.Models.Post
{
    public class PostsAggregatorViewModel
    {
        public bool IsProfileOwner { get; set; }
        public long BeginPage { get; set; }
        public long EndPage { get; set; }
        public long CurrentPage { get; set; }
        public long LastPage { get; set; }
        public string PlacesApi { get; set; }
        public ProfileModel Profile { get; set; }
        public IEnumerable<PostViewModel> Posts { get; set; }
        public IEnumerable<PostCommentViewModel> PostComments { get; set; } 
        
        public IEnumerable<RecommendedPostViewModel> RecommendedPosts { get; set; }
    }
}