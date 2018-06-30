using System;

namespace Egghead.Models.Articles
{   
    public class Article
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Unlinkes { get; set; }
        public int Comments { get; set; }   
    }
}