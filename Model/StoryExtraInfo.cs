using System.Collections.Generic;

namespace Brook.DuDuRiBao.Models
{
    public class ExtraCount
    {
        public int Circles { get; set; }
        public int Post_Reasons { get; set; }
        public int Normal_Comments { get; set; }
        public int Comments { get; set; }
        public int Likes { get; set; }
        public int Posters { get; set; }
        public int Reposts { get; set; }
    }

    public class StoryExtraInfo
    {
        public ExtraCount Count { get; set; }
        public bool Favorite { get; set; }
        public int Vote_Status { get; set; }
    }
}
