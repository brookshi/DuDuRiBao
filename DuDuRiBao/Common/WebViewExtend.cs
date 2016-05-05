using Brook.DuDuRiBao.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brook.DuDuRiBao.Common
{
    public class WebViewExtend : DependencyObject
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(MainContent), typeof(WebViewExtend), new PropertyMetadata(null, LoadHtmlSource));

        private static void LoadHtmlSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webView = d as WebView;
            if (webView == null)
                return;

            var mainHtmlContent = e.NewValue as MainContent;
            if (mainHtmlContent == null)
                return;

            if (!string.IsNullOrEmpty(mainHtmlContent.Body))
            {
                webView.NavigateToString(mainHtmlContent.Body);
            }
            else if(!string.IsNullOrEmpty(mainHtmlContent.Share_Url))
            {
                webView.Navigate(new Uri(mainHtmlContent.Share_Url));
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
    }
}
