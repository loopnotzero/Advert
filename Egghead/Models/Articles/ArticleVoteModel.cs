﻿using Egghead.Common.Articles;

namespace Egghead.Models.Articles
{
    public class ArticleVoteModel
    {
        public string ArticleId { get; set; }
        public VoteType VoteType { get; set; }
    }
}