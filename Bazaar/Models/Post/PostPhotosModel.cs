using System.Collections.Generic;

namespace Bazaar.Models.Post
{
    public class PostPhotosModel
    {
        public string PostId { get; set; }
        public List<string> PostPhotos { get; set; }        
    }
}