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

namespace Brook.DuDuRiBao.Utils
{
    public static class HotArticleBuilder
    {
        private const string HotRiBaoRegex = "circlely://story/\\d*|avatar.*>|title\">.*</span>|<i>.*</i>";
        private const int Step = 4;

        public static List<Story> Builder(string hotRiBaoContent)
        {
            Regex regex = new Regex(HotRiBaoRegex, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var matches = regex.Matches(hotRiBaoContent);
            if (matches.Count % Step != 0)
                throw new Exception("error matched count when do regex for hot article");

            List<Story> list = new List<Story>();
            for (int i = 0; i < matches.Count; i += Step)
            {
                list.Add(GenerateByMatches(matches[i].Value, matches[i + 1].Value, matches[i + 2].Value, matches[i + 3].Value));
            }

            return list;
        }

        public static Story GenerateByMatches(string matchId, string matchThumbnail, string matchTitle, string matchFollowers)
        {
            var id = matchId.Substring(matchId.LastIndexOf("/") + 1);
            var thumbnail = HandleThumbnail(matchThumbnail);
            var title = HandleTitle(matchTitle);
            var followers = matchFollowers.Replace("<i>", "").Replace("</i>", "");

            return new Story()
            {
                Id = int.Parse(id),
                WebImage = thumbnail,
                Title = title,
                FollowerCount = followers
            };
        }

        static string HandleThumbnail(string matchThumbnail)
        {
            string thumbnail = matchThumbnail.Substring(13, matchThumbnail.LastIndexOf("\"") - 13);
            var httpsIndex = matchThumbnail.IndexOf("http");
            if (httpsIndex < 0)
            {
                thumbnail = Urls.LocalUrlPrefx + "/" + thumbnail;
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
