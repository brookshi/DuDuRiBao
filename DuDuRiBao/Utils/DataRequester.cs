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
using System.Threading.Tasks;
using XPHttp;
using XPHttp.HttpContent;

namespace Brook.DuDuRiBao.Utils
{
    public class DataRequester
    {
        public static Task<TimeLine> RequestLatestTimeLine()
        {
            return RequestDataForTimeLine<TimeLine>("", Urls.TimeLine);
        }
        public static Task<TimeLine> RequestNextTimeLine(string before)
        {
            return RequestDataForTimeLine<TimeLine>(before, Urls.NextTimeLine);
        }

        public static Task<MainContent> RequestStoryContent(string storyId)
        {
            return RequestDataForStory<MainContent>(storyId, "", Urls.StoryContent);
        }

        public static Task<StoryExtraInfo> RequestStoryExtraInfo(string storyId)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("storyid", storyId ?? "")
                .SetIfModifiedSince(DateTime.Now);

            return XPHttpClient.DefaultClient.GetAsync<StoryExtraInfo>(Urls.StoryExtraInfo, httpParam);
        }

        public static Task<CommentList> RequestComments(string storyId, string before)
        {
            return RequestDataForStory<CommentList>(storyId, before, string.IsNullOrEmpty(before) ? Urls.Comments : Urls.NextComments);
        }

        public static Task<string> RequestHotCircles()
        {
            return XPHttpClient.DefaultClient.GetAsync<string>(Urls.HotCircle, null);
        }

        public static Task<LoginToken> AnonymousLogin(string key)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetJsonStringBody("{\"data\":\"" + key + "\"}");
            
            return XPHttpClient.DefaultClient.PostAsync<LoginToken>(Urls.AnonymousLogin, httpParam);
        }
        //public static Task<MainData> RequestStories(string before)
        //{
        //    return RequestDataForStory<MainData>("", before, Urls.Stories);
        //}

        //public static Task<MainData> RequestLatestStories()
        //{
        //    return RequestDataForStory<MainData>("", Urls.LatestStories);
        //}

        public static Task<MinorData> RequestCategoryStories(string categoryId, string before)
        {
            return RequestDataForCategory<MinorData>(categoryId, before, Urls.CategoryStories);
        }

        public static Task<MinorData> RequestCategoryLatestStories(string categoryId)
        {
            return RequestDataForCategory<MinorData>(categoryId, "", Urls.CategoryLatestStories);
        }

        public static Task<CommentList> RequestShortComment(string storyId, string before)
        {
            return RequestDataForStory<CommentList>(storyId, before, string.IsNullOrEmpty(before) ? Urls.ShortComment : Urls.ShortComment_More);
        }

        public static Task<ZhiHuAuthoInfo> Login(LoginData loginData)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder.SetBody(new HttpJsonContent(loginData));
            return XPHttpClient.DefaultClient.PostAsync<ZhiHuAuthoInfo>(Urls.Login, httpParam);
        }

        public static void SetStoryFavorite(string storyId, bool isFav)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("storyid", storyId ?? "");
            if (isFav)
            {
                XPHttpClient.DefaultClient.PostAsync(Urls.Favorite, httpParam, null);
            }
            else
            {
                XPHttpClient.DefaultClient.DeleteAsync(Urls.Favorite, httpParam, null);
            }
        }

        public static void SetStoryLike(string storyId, bool isLike)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("storyid", storyId ?? "")
                .SetStringBody(string.Format("data={0}", isLike ? "1" : "0"))
                .SetContentEncoding(Windows.Storage.Streams.UnicodeEncoding.Utf8)
                .SetMediaType("application/x-www-form-urlencoded");

            XPHttpClient.DefaultClient.PostAsync(Urls.Vote, httpParam, null);
        }

        public static async Task SendComment(string storyId, string content, int? replyCommentId)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("storyid", storyId ?? "")
                .SetObjectBody(new SendCommentData() { content = content, reply_to = replyCommentId }, HttpContentType.Json);

            await XPHttpClient.DefaultClient.PostAsync(Urls.SendComment, httpParam);
        }

        public static void DeleteComment(string commentId)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                  .AddUrlSegements("commentid", commentId ?? "");

            XPHttpClient.DefaultClient.DeleteAsync(Urls.DeleteComment, httpParam, null);
        }

        public static void LikeComment(string commentId)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("commentid", commentId ?? "")
                .SetContentEncoding(Windows.Storage.Streams.UnicodeEncoding.Utf8)
                .SetMediaType("application/x-www-form-urlencoded");

            XPHttpClient.DefaultClient.PostAsync(Urls.LikeComment, httpParam, null);
        }

        public static void UnlikeComment(string commentId)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("commentid", commentId ?? "");

            XPHttpClient.DefaultClient.DeleteAsync(Urls.LikeComment, httpParam, null);
        }

        //public static Task<Favorites> RequestLatestFavorites()
        //{
        //    return XPHttpClient.DefaultClient.GetAsync<Favorites>(Urls.LatestFavorites, null);
        //}

        //public static Task<Favorites> RequestFavorites(string lastTime)
        //{
        //    var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
        //        .AddUrlSegements("lasttime", lastTime);
        //    return XPHttpClient.DefaultClient.GetAsync<Favorites>(Urls.Favorites, httpParam);
        //}

        public static Task<T> RequestDataForTimeLine<T>(string before, string functionUrl)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("before", before ?? "");

            return XPHttpClient.DefaultClient.GetAsync<T>(functionUrl, httpParam);
        }

        public static Task<T> RequestDataForStory<T>(string storyId, string before, string functionUrl)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("storyid", storyId ?? "")
                .AddUrlSegements("before", before ?? "");

            return XPHttpClient.DefaultClient.GetAsync<T>(functionUrl, httpParam);
        }

        public static Task<T> RequestDataForStory<T>(string storyId, string functionUrl)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("storyid", storyId ?? "");

            return XPHttpClient.DefaultClient.GetAsync<T>(functionUrl, httpParam);
        }

        public static Task<T> RequestDataForCategory<T>(string categoryId, string before, string functionUrl)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("categoryid", categoryId.ToString())
                .AddUrlSegements("before", before ?? "");

            return XPHttpClient.DefaultClient.GetAsync<T>(functionUrl, httpParam);
        }
    }
}
