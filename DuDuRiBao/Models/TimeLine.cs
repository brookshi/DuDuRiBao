using System.Collections.Generic;

namespace Brook.DuDuRiBao.Models
{
    public class Count
    {
        public int Likes { get; set; }
        public int Comments { get; set; }
    }
    public class OriginCircle
    {
        public string Thumbnail { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Poster
    {
        public int Id { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
    }
    public class OriginPoster
    {
        public string Reason { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Post
    {
        public OriginCircle Origin_Circle { get; set; }
        public Poster Poster { get; set; }
        public HotCircle Circle { get; set; }
        public OriginPoster Origin_Poster { get; set; }
    }
    public class Story
    {
        public Count Count { get; set; }
        public string Title { get; set; }
        public IList<Post> Posts { get; set; }
        public int Time { get; set; }
        public IList<string> Images { get; set; }
        public int Vote_Status { get; set; }
        public int Type { get; set; }
        public int Id { get; set; }
        public string External_Url { get; set; }
    }
    public class TimeLine
    {
        public int Dimension { get; set; }
        public IList<Story> Items { get; set; }
        public int Time { get; set; }
    }
}
