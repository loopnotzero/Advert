using System.Collections.Generic;
using Bazaar.Models.Profiles;

namespace Bazaar.Models.Post
{
    public class PostsAggregatorModel
    {
        public string PlacesApi { get; set; }
        public ProfileModel Profile { get; set; }       
        public IEnumerable<PostModel> Posts { get; set; }
        public IEnumerable<PostCommentModel> PostComments { get; set; } 
    }
}