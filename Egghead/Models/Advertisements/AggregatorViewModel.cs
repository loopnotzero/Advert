using System.Collections.Generic;
using Egghead.Models.Profiles;

namespace Egghead.Models.Advertisements
{
    public class AggregatorViewModel
    {
        public long BeginPage { get; set; }
        public long EndPage { get; set; }
        public long CurrentPage { get; set; }
        public long LastPage { get; set; }
        public ProfileModel Profile { get; set; }
        public IEnumerable<AdvertisementViewModel> Advertisements { get; set; }
        public IEnumerable<AdvertisementCommentViewModel> AdvertisementsComments { get; set; }   
        public IEnumerable<RecommendedAdvertisementViewModel> RecommendedAdvertisements { get; set; }
    }
}