using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brook.DuDuRiBao.Common
{
    public class WebViewExtend : DependencyObject
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(MainContent), typeof(WebViewExtend), new PropertyMetadata(null, LoadHtmlSource));

        public static readonly DependencyProperty StringContentProperty =
            DependencyProperty.Register("StringContent", typeof(string), typeof(WebViewExtend), new PropertyMetadata(null, LoadString));

        private static void LoadHtmlSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webView = d as WebView;
            if (webView == null)
                return;

            if(e.NewValue is string)
            {
                webView.NavigateToString(e.NewValue.ToString());
                return;
            }

            var mainHtmlContent = e.NewValue as MainContent;
            if (mainHtmlContent == null)
                return;

            if (!string.IsNullOrEmpty(mainHtmlContent.Body))
            {
                webView.NavigateToString(mainHtmlContent.Body);
            }
            else if (!string.IsNullOrEmpty(mainHtmlContent.External_Url))
            {
                webView.Navigate(new Uri(mainHtmlContent.External_Url));
            }
            else if(!string.IsNullOrEmpty(mainHtmlContent.Share_Url))
            {
                webView.Navigate(new Uri(mainHtmlContent.Share_Url));
            }
        }

        private static void LoadString(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webView = d as WebView;
            if (webView == null)
                return;

            if (e.NewValue is string)
            {
                var content = e.NewValue.ToString();
                content = content.Replace(Html.RelativeScriptFlag, Html.AbsoluteScriptFlag);
                content = content.Replace(Html.RelativeCSSFlag, Html.AbsoluteCSSFlag);
                content = content.Replace("</head>", Html._notifyScript + "</head>");
                if (StorageInfo.Instance.AppTheme == ElementTheme.Dark)
                {
                    content = content.Replace(Html.BodyFlag, Html.BodyNightFlag);
                }
                webView.NavigateToString(content);
            }
        }

        public static void SetContent(DependencyObject obj, string value)
        {
            obj.SetValue(ContentProperty, value);
        }

        public static string GetContent(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentProperty);
        }

        public static void SetStringContent(DependencyObject obj, string value)
        {
            obj.SetValue(StringContentProperty, value);
        }

        public static string GetStringContent(DependencyObject obj)
        {
            return (string)obj.GetValue(StringContentProperty);
        }
    }
}
