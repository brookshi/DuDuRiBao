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
    public class Circle
    {
        public string Thumbnail { get; set; }
        public int Id { get; set; }
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
        public OriginCircle OriginCircle { get; set; }
        public Poster Poster { get; set; }
        public Circle Circle { get; set; }
        public OriginPoster OriginPoster { get; set; }
    }
    public class Story
    {
        public Count Count { get; set; }
        public string Title { get; set; }
        public IList<Post> Posts { get; set; }
        public int Time { get; set; }
        public IList<string> Images { get; set; }
        public int VoteStatus { get; set; }
        public int Type { get; set; }
        public int Id { get; set; }
        public string ExternalUrl { get; set; }
    }
    public class TimeLine
    {
        public int Dimension { get; set; }
        public IList<Story> Items { get; set; }
        public int Time { get; set; }
    }
}
