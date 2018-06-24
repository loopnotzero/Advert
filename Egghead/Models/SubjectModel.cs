using System;
using Egghead.Common;

namespace Egghead.Models
{
    public class SubjectModel
    {
        public string Title { get; set; }
        public string Text { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public ReleaseType ReleaseType { get; set; }
    }
}