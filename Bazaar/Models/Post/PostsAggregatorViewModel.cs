using System.Collections.Generic;
using Bazaar.Models.Profiles;

namespace Bazaar.Models.Post
{
    public class PostsAggregatorViewModel
    {
        public string PlacesApi { get; set; }
        public ProfileViewModel Profile { get; set; }       
        public IEnumerable<PostViewModel> Posts { get; set; }
        public IEnumerable<PostCommentViewModel> PostComments { get; set; } 
        
        public IEnumerable<RecommendedPostViewModel> RecommendedPosts { get; set; }
    }
}