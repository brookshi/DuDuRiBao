#region License
//   Copyright 2015 Brook Shi
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using Brook.DuDuRiBao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Brook.DuDuRiBao.Utils
{
    public static class HotCircleBuilder
    {
        private const string HotRiBaoRegex = "circle/\\d*|cell-avatar.*>|cell-title\">.*</span>|<i>.*</i>";
        private const int Step = 5;

        public static List<HotCircle> Builder(string hotRiBaoContent)
        {
            Regex regex = new Regex(HotRiBaoRegex, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var matches = regex.Matches(hotRiBaoContent);
            if (matches.Count % Step != 0)
                throw new Exception("error matched count when do regex for hot ri bao");

            List<HotCircle> list = new List<HotCircle>();
            for (int i = 0; i < matches.Count; i += Step)
            {
                list.Add(GenerateByMatches(matches[i].Value, matches[i + 1].Value, matches[i + 2].Value, matches[i + 3].Value, matches[i + 4].Value));
            }

            return list;
        }

        public static HotCircle GenerateByMatches(string matchId, string matchThumbnail, string matchTitle, string matchArticles, string matchFans)
        {
            var id = matchId.Substring(matchId.LastIndexOf("/") + 1);
            var thumbnail = HandleThumbnail(matchThumbnail);
            var title = HandleTitle(matchTitle);
            var articles = matchArticles.Replace("<i>", "").Replace("</i>", "");
            var fans = matchFans.Replace("<i>", "").Replace("</i>", "");
            var circle = new HotCircle()
            {
                Id = id,
                Thumbnail = thumbnail,
                Name = title,
                Articles = articles,
                Fans = fans,
            };

            return circle;
        }

        static string HandleThumbnail(string matchThumbnail)
        {
            string thumbnail = "";
            var httpsIndex = matchThumbnail.IndexOf("http");
            if (httpsIndex > 0)
            {
                thumbnail = matchThumbnail.Substring(httpsIndex).Replace("\">", "");
            }
            else
            {
                var firstIndex = matchThumbnail.IndexOf(">");
                var lastIndex = matchThumbnail.LastIndexOf("<");
                if (firstIndex >= 0 && lastIndex >= 0 && lastIndex > firstIndex)
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
