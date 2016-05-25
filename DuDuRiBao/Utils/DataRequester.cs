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

using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using XPHttp;
using XPHttp.HttpContent;

namespace Brook.DuDuRiBao.Utils
{
    public class DataRequester
    {
        public static Task<RiBaoAuthoInfo> LoginUsingWeibo(TokenInfo info)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetBody(new HttpJsonContent(info))
                .AddHeader("x-client-id", "3");
            return XPHttpClient.DefaultClient.PostAsync<RiBaoAuthoInfo>(Urls.Login, httpParam);
        }

        public static Task<RiBaoAuthoInfo> LoginUsingZhiHu(TokenInfo info)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetBody(new HttpJsonContent(info));
            return XPHttpClient.DefaultClient.PostAsync<RiBaoAuthoInfo>(Urls.Login, httpParam);
        }

        public static Task<LoginToken> AnonymousLogin(string key)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetJsonStringBody("{\"data\":\"" + key + "\"}");

            return XPHttpClient.DefaultClient.PostAsync<LoginToken>(Urls.AnonymousLogin, httpParam);
        }

        public static Task<ZhiHuSignInfo> ZhiHuLogin(string userName, string password)
        {
            var postData = LoginKeyProvider.GetZhiHuLoginKey(userName, password);
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetStringBody(postData)
                .SetNeedBaseUrl(false)
                .SetContentEncoding(Windows.Storage.Streams.UnicodeEncoding.Utf8)
                .SetAuthorization("oauth", LoginKeyProvider.ClientId)
                .AddHeader("User-Agent", "Google-HTTP-Java-Client/1.20.0 (gzip)")
                .SetMediaType("application/x-www-form-urlencoded");

            return XPHttpClient.DefaultClient.PostAsync<ZhiHuSignInfo>(Urls.ZhiHuLogin, httpParam);
        }

        public static async Task<Captcha> GetCaptchaImage()
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetNeedBaseUrl(false)
                .SetAuthorization("oauth", LoginKeyProvider.ClientId);

            var captcha = await XPHttpClient.DefaultClient.GetAsync<Captcha>(Urls.ZhiHuCaptcha, httpParam);
            if(captcha.Show_Captcha)
            {
                return await XPHttpClient.DefaultClient.PutAsync<Captcha>(Urls.ZhiHuCaptcha, httpParam);
            }

            return null;
        }

        public static Task<CaptchaChecked> CheckCaptcha(string captcha)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetStringBody("input_text="+captcha)
                .SetNeedBaseUrl(false)
                .SetContentEncoding(Windows.Storage.Streams.UnicodeEncoding.Utf8)
                .SetAuthorization("oauth", LoginKeyProvider.ClientId)
                .SetMediaType("application/x-www-form-urlencoded");

            return XPHttpClient.DefaultClient.PostAsync<CaptchaChecked>(Urls.ZhiHuCaptcha, httpParam);
        }

        public static Task<ZhiHuAuthInfo> GetZhiHuAuthorization(ZhiHuSignInfo info)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetStringBody("app_id=6&redirect_uri=http://dudu.zhihu.com/zhihu/auth&response_type=code")
                .SetContentEncoding(Windows.Storage.Streams.UnicodeEncoding.Utf8)
                .SetAuthorization(info.Token_Type, info.Access_Token)
                .SetNeedBaseUrl(false)
                .SetMediaType("application/x-www-form-urlencoded");

            return XPHttpClient.DefaultClient.PostAsync<ZhiHuAuthInfo>(Urls.ZhiHuAuthorization, httpParam);
        }

        public static Task<TokenInfo> GetZhiHuToken(ZhiHuAuthInfo info)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .SetStringBody(string.Format($"code={info.Authorization_Code}&grant_type=authorization_code&redirect_uri=http://dudu.zhihu.com/zhihu/auth&app_id=6&app_key=90e026a5762d4a5e94c2d93b58e88955"))
                .SetContentEncoding(Windows.Storage.Streams.UnicodeEncoding.Utf8)
                .SetNeedBaseUrl(false)
                .SetMediaType("application/x-www-form-urlencoded");

            return XPHttpClient.DefaultClient.PostAsync<TokenInfo>(Urls.ZhiHuToken, httpParam);
        }

        public static Task<TimeLine> RequestLatestTimeLine()
        {
            return RequestDataForTimeLine<TimeLine>("", Urls.TimeLine);
        }

        public static Task<TimeLine> RequestNextTimeLine(string before)
        {
            return RequestDataForTimeLine<TimeLine>(before, Urls.NextTimeLine);
        }

        public static Task<HotCircleStories> RequestLatestStoriesForCircle(string circleId)
        {
            return RequestDataForCircleStories<HotCircleStories>(circleId, "", Urls.CircleStories);
        }

        public static Task<HotCircleStories> RequestNextStoriesForCircle(string circleId, string before)
        {
            return RequestDataForCircleStories<HotCircleStories>(circleId, before, Urls.NextCircleStories);
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

        public static Task<string> RequestHotCircles()
        {
            return XPHttpClient.DefaultClient.GetAsync<string>(Urls.HotCircle, null);
        }

        public static Task<string> RequestHotArticles()
        {
            return XPHttpClient.DefaultClient.GetAsync<string>(Urls.HotArticle, null);
        }

        public static Task JoinCircle(string circleId)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("circleid", circleId);
            return XPHttpClient.DefaultClient.PostAsync<RiBaoAuthoInfo>(Urls.JoinCircle, httpParam);
        }

        public static Task QuitCircle(string circleId)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("circleid", circleId);
            return XPHttpClient.DefaultClient.PostAsync<RiBaoAuthoInfo>(Urls.QuitCircle, httpParam);
        }

        public static Task<Favorites> RequestLatestFavorites()
        {
            return XPHttpClient.DefaultClient.GetAsync<Favorites>(Urls.LatestFavorites, null);
        }

        public static Task<Favorites> RequestFavorites(string lastTime)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("lasttime", lastTime);
            return XPHttpClient.DefaultClient.GetAsync<Favorites>(Urls.Favorites, httpParam);
        }

        public static void SetFavoriteStory(string storyId, bool isFav)
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

        public static Task<CommentList> RequestNormalComments(string storyId, string before)
        {
            return RequestDataForStory<CommentList>(storyId, before, string.IsNullOrEmpty(before) ? Urls.Comments : Urls.NextComments);
        }

        public static Task<CommentList> RequestRecommendComments(string storyId, string before)
        {
            return RequestDataForStory<CommentList>(storyId, before, string.IsNullOrEmpty(before) ? Urls.PostReason : Urls.NextPostReason);
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

        public static Task<SearchCircles> SearchCircles(string text)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("text", text != null ? WebUtility.UrlEncode(text) : "");

            return XPHttpClient.DefaultClient.GetAsync<SearchCircles>(Urls.SearchCircle, httpParam);
        }

        public static Task<SearchStories> SearchStories(string text)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("text", text != null ? WebUtility.UrlEncode(text) : "");

            return XPHttpClient.DefaultClient.GetAsync<SearchStories>(Urls.SearchStory, httpParam);
        }


        public static Task<T> RequestDataForTimeLine<T>(string before, string functionUrl)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("before", before ?? "");

            return XPHttpClient.DefaultClient.GetAsync<T>(functionUrl, httpParam);
        }

        public static Task<T> RequestDataForCircleStories<T>(string circleId, string before, string functionUrl)
        {
            var httpParam = XPHttpClient.DefaultClient.RequestParamBuilder
                .AddUrlSegements("circleid", circleId ?? "")
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
    }
}
