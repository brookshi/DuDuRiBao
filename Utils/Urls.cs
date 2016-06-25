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


namespace Brook.DuDuRiBao.Utils
{
    public static class Urls
    {
        public const string LocalUrlPrefx = "http://daily.zhihu.com";

        public const string BaseUrl = "http://news-at.zhihu.com/api/7/";

        public const string ZhiHuLogin = "https://api.zhihu.com/sign_in";

        public const string ZhiHuAuthorization = "https://api.zhihu.com/authorization";

        public const string ZhiHuToken = "https://openapi.zhihu.com/access_token";

        public const string ZhiHuCaptcha = "https://api.zhihu.com/captcha";

        public const string TimeLine = "home_timeline";

        public const string Explorer= "explore";

        public const string NextTimeLine = "home_timeline/before/{before}";

        public const string CircleInfo = "circle/{circleid}";

        public const string CircleStories = "circle/{circleid}/stories";

        public const string NextCircleStories = "circle/{circleid}/stories/before/{before}";

        public const string HotCircle = "explore/circles/hot";

        public const string HotArticle = "explore/stories/hot";

        public const string JoinCircle = "circle/{circleid}/join";

        public const string QuitCircle = "circle/{circleid}/quit";

        public const string StoryContent = "story/{storyid}";

        public const string StoryExtraInfo = "story-extra/{storyid}";

        public const string AnonymousLogin = "anonymous-login";

        public const string Comments = "story/{storyid}/normal-comments";

        public const string NextComments = "story/{storyid}/normal-comments/before/{before}";

        public const string PostReason = "story/{storyid}/post-reasons";

        public const string NextPostReason = "story/{storyid}/post-reasons/before/{before}";

        public const string Login = "login";

        public const string SendComment = "news/{storyid}/comment";

        public const string DeleteComment = "comment/{commentid}";

        public const string LikeComment = "vote/comment/{commentid}";

        public const string Vote = "vote/story/{storyid}";

        public const string Favorite = "favorite/{storyid}";

        public const string LatestFavorites = "favorites/";

        public const string Favorites = "favorites/before/{lasttime}";

        public const string SearchCircle = "search/circle?q={text}";

        public const string SearchStory = "search/story?q={text}";

        public const string SearchUser = "search/user?q={text}";


        public const string Version = "http://o8vbhnlwz.bkt.clouddn.com/ver.json";

        public const string Score = "ms-windows-store://review/?ProductId=9NBLGGH4NQFV";

        public const string Download = "ms-windows-store://pdp/?ProductId=9NBLGGH4NQFV";
    }
}
