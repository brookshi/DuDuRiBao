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
        public const string HotRiBaoRegex = "circle/\\d*|cell-avatar.*>|cell-title\">.*</span>|<i>.*</i>";

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
            var thumbnail = HandleThumbnail(matchThumbnail);
            var title = HandleTitle(matchTitle);
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

        static string HandleThumbnail(string matchThumbnail)
        {
            string thumbnail = "";
            var httpsIndex = matchThumbnail.IndexOf("https");
            if (httpsIndex > 0)
            {
                thumbnail = matchThumbnail.Substring(httpsIndex).Replace("\">", "");
            }
            else
            {
                var firstIndex = matchThumbnail.IndexOf(">");
                var lastIndex = matchThumbnail.LastIndexOf("<");
                if(firstIndex >=0 && lastIndex >=0 && lastIndex > firstIndex)
                {
                    thumbnail = matchThumbnail.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                }
            }
            return thumbnail;
        }

        static string HandleTitle(string matchTitle)
        {
            string title = "";
            var firstIndex = matchTitle.IndexOf(">");
            var lastIndex = matchTitle.LastIndexOf("<");
            if (firstIndex >= 0 && lastIndex >= 0 && lastIndex > firstIndex)
            {
                title = matchTitle.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
            }
            return title;
        }
    }
}
