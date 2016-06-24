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
using System.Text;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Text.RegularExpressions;

namespace Brook.DuDuRiBao.Models
{
    public static class Html
    {
        public const string RelativeScriptFlag = "<script src=\"/";
        public const string RelativeCSSFlag = "<link rel=\"stylesheet\" href=\"/";
        public const string AbsoluteScriptFlag = "<script src=\"http://daily.zhihu.com/";
        public const string AbsoluteCSSFlag = "<link rel=\"stylesheet\" href=\"http://daily.zhihu.com/";
        public const string BodyFlag = "<body class=\"";
        public const string BodyNightFlag = "<body class=\"night ";
        public const string NotifyCircle = "circlely://";

        public const string NotifyPrex = "LinkNotify:";

        private const string _htmlTemplate = "<html><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes\" /><head><meta charset = \"utf-8\" > {0} {1} </head> {2} {3}</html> ";

        private const string _cssTemplate = "<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}\" />";

        private const string _jsTemplate = "<script src=\"{0}\" type=\"text/javascript\"></script>";

        public const string _notifyScript = "<script type=\"text/javascript\">for (var i = 0; i < document.links.length; i++) { document.links[i].onclick = function() { window.external.notify('"+ NotifyPrex + "' + this.href); return false; } }</script>";

        private const string _headerTemplate = "<div style=\"position:relative; height:250;  background:url({0}) no-repeat center center; background-size:100%; \">"
                                               + "<div style=\"position:relative; height:250;  background-image:url(ms-appx-web:///Assets/header_background.png); background-size:100% 100%;\">"
                                               + "<table style = \"position:absolute; Bottom:30px; color:{1}; font-weight:bold; font-size:30;\" >"
                                               +"<tr><td style=\"width:20px\"></td><td>{2}</td><td style=\"width:20px\"></td></tr>"
                                               +"</table>"
                                               +"<table style=\"position:absolute; right:4px; margin:0,20; Bottom:8px;color:{3};font-size:15;\">"
                                               + "<tr><td>{4}</td><td style=\"width:20px\"></td></tr>"
                                               + "</table>"
                                               +"</div>"
                                               +"</div>";

        private const string _lazyLoadDiv = "<div class='progressB' style='text-align:center; padding-top:40px; font-size:20px; color:#000000; background-color:gray; height:80px' data-src='{0}'> 点击加载图片 </div>";

        private const string _lazyLoadScript = "<script>function getElementsByClassName(tagName,className) { var tag = document.getElementsByTagName(tagName); var tagAll = []; for(var i = 0 ; i<tag.length ; i++){  if(tag[i].className.indexOf(className) != -1){ tagAll[tagAll.length] = tag[i]; }} return tagAll; }  function removeElement(_element){var _parentElement = _element.parentNode;if(_parentElement){_parentElement.removeChild(_element); }}function insertAfter(newElement,targetElement){ var parent=targetElement.parentNode; if(parent.lastChild==targetElement){ parent.appendChild(newElement); } else{ parent.insertBefore(newElement,targetElement.nextSibling); } } var $progresses = getElementsByClassName('div','progressB');$progresses.forEach(function($progress){var url = $progress.getAttribute('data-src');$progress.addEventListener('click', function() {var $img = $progress.nextSibling; if(!$img){$img = document.createElement('img');$img.setAttribute('class', 'content-image');insertAfter($img, $progress);}else{$img.setAttribute('src', url); removeElement($progress);}});});</script>";

        public static void ArrangeMainContent(MainContent content)
        {
            content.Body = ConstructorHtml(content);
        }

        public static string ConstructorHtml(MainContent content)
        {
            if (string.IsNullOrEmpty(content.Body))
                return string.Empty;

            var cssBuilder = new StringBuilder();
            var jsBuilder = new StringBuilder();

            content.Css.ForEach(o => cssBuilder.Append(string.Format(_cssTemplate, o)));
            content.Js.ForEach(o => jsBuilder.Append(string.Format(_jsTemplate, o)));

            var titleColor = ((SolidColorBrush)((ResourceDictionary)Application.Current.Resources.ThemeDictionaries[StorageInfo.Instance.AppTheme == ElementTheme.Dark?"Dark":"Light"])["BrushToolBarForeground"]).Color.ToString();
            if (titleColor.Length == 9)
                titleColor = titleColor.Replace("#FF", "#");
            var header = string.Format(_headerTemplate, content.Image, titleColor, content.Title, titleColor, content.Image_Source);
            string body = "";
            if(StorageInfo.Instance.AppTheme == ElementTheme.Dark)
            {
                var backgroundColor = ((SolidColorBrush)((ResourceDictionary)Application.Current.Resources.ThemeDictionaries["Dark"])["BrushPrimary"]).Color.ToString();
                body = "<body class=\"dudu-night\">" + content.Body + "</body>";
                body = body.Replace("class=\"main-wrap content-wrap\"", "class=\"main-wrap content-wrap\" style=\"background:"+ backgroundColor + "\"");
            }
            else
            {
                body = "<body>" + content.Body + "</body>";
            }
            var script = _notifyScript;
            if(StorageInfo.Instance.NeedLazyLoadImage)
            {
                body = LazyImageHandle(body);
                script += _lazyLoadScript;
            }
            var source = string.Format(_htmlTemplate, cssBuilder.ToString(), jsBuilder.ToString(), body, script);

            source = source.Replace("<div class=\"img-place-holder\"></div>", header);

            return source;
        }

        private static string LazyImageHandle(string body)
        {
            var newBody = body;
            var pattern = "<img.*?>";
            var srcPattern = "src=\".*?\"";
            var regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var srcRegex = new Regex(srcPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var matches = regex.Matches(newBody);
            for(int i=0; i< matches.Count;i++)
            {
                if (matches[i].Value.Contains("class=\"avatar\""))
                    continue;

                var srcMatch = srcRegex.Match(matches[i].Value);
                if (srcMatch == null || string.IsNullOrEmpty(srcMatch.Value))
                    continue;

                newBody = newBody.Replace(matches[i].Value, string.Format(_lazyLoadDiv, srcMatch.Value.Replace("src=\"", "").Replace("\"", "")) + srcRegex.Replace(matches[i].Value, ""));
            }

            return newBody;
        }
    }
}
