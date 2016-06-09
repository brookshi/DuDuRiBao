using Brook.DuDuRiBao.Utils;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

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
        public Poster Poster { get; set; }
        public int Time { get; set; }
        public IList<string> Images { get; set; }
        public int Vote_Status { get; set; }
        public int Type { get; set; }
        public int Id { get; set; }
        public string External_Url { get; set; }
        public string FollowerCount { get; set; }
        public string WebImage { get; set; }

        public void AdjustForHotCircleStory(CircleBase circle)
        {
            if (Posts == null && circle != null)
            {
                Posts = new List<Post>();
                Posts.Add(new Post()
                {
                    Poster = Poster,
                    Circle = circle.Clone()
                });
            }
            AdjustForImage();
        }

        public void AdjustForImage()
        {
            if (Images == null || Images.Count == 0)
            {
                Images = new List<string>() { "ms-appx:///Assets/StoryPlaceHolder.png" };
            }
        }
    }
    public class TimeLine
    {
        public int Dimension { get; set; }
        public List<Story> Items { get; set; }
        public int Time { get; set; }

        public void Adjust()
        {
            Items.ForEach(item =>
            {
                if(item.Posts.Count > 0)
                    item.Posts[0].Circle.Adjust();
                item.AdjustForImage();
            });
        }
    }

    public class HotCircleStories
    {
        public List<Story> Stories { get; set; }
        public int Time { get; set; }
    }

    public class Favorites
    {
        public int count { get; set; }
        public List<Story> stories { get; set; }
        public int? last_time { get; set; } = null;
    }

    public class CircleCount
    {
        public int Stories { get; set; }
        public int Editors { get; set; }
        public int Members { get; set; }
    }
    public class Creator
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CircleInfo : CircleBase
    {
        public CircleCount Count { get; set; }
        public int Status { get; set; }
        public string Member_Alias { get; set; }
        public string Image { get; set; }
        public Creator Creator { get; set; }
        public Brush Mask { get; set; }

        public override HotCircle Clone()
        {
            return new HotCircle()
            {
                Thumbnail = Thumbnail,
                Id = Id,
                BackgroundBrush = BackgroundBrush,
                Name = Name
            };
        }

        public override void Adjust()
        {
            base.Adjust();
            Mask = Thumbnail.Length < 2 ? ColorUtil.GetDeepInBrush(BackgroundBrush) : ColorUtil.GetDefaultCirclePanelBackground();
        }
    }
}
