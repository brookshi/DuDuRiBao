using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Brook.DuDuRiBao.Models
{
    public class HotCircle
    {
        public string Thumbnail { get; set; }

        public string Id { get; set; }

        public string Articles { get; set; }

        public string Fans { get; set; }

        public string Name { get; set; }
    }

    public static class HotCircleBuilder
    {
        public const string HotRiBaoRegex = "circle/\\d*|<img class=\"cell-avatar.*\">|cell-title\">.*</span>|<i>.*</i>";

        public static List<HotCircle> Builder(string hotRiBaoContent)
        {
            Regex regex = new Regex(HotRiBaoRegex, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var matches = regex.Matches(hotRiBaoContent);
            if (matches.Count % 5 != 0)
                throw new Exception("error matched count when do regex for hot ri bao");

            List<HotCircle> list = new List<HotCircle>();
            for(int i=0;i<matches.Count;i+=5)
            {
                list.Add(GenerateByMatches(matches[i].Value, matches[i + 1].Value, matches[i + 2].Value, matches[i + 3].Value, matches[i + 4].Value));
            }

            return list;
        }

        public static HotCircle GenerateByMatches(string matchId, string matchThumbnail, string matchTitle, string matchArticles, string matchFans)
        {
            var id = matchId.Replace("circle/", "");
            var thumbnail = matchThumbnail.Substring(matchThumbnail.IndexOf("https")).Replace("\">", "");
            var title = matchTitle.Substring(matchTitle.IndexOf("<") + 1, matchTitle.LastIndexOf("<") - matchTitle.IndexOf("<") - 1);
            var articles = matchArticles.Replace("<i>", "").Replace("</i>", "");
            var fans = matchFans.Replace("<i>", "").Replace("</i>", "");

            return new HotCircle()
            {
                Id = id,
                Thumbnail = thumbnail,
                Name = title,
                Articles = articles,
                Fans = fans
            };
        }
    }
}
