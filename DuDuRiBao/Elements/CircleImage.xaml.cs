using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Brook.DuDuRiBao.Elements
{
    public sealed partial class CircleImage : UserControl
    {
        public CircleImage()
        {
            this.InitializeComponent();
        }

        public event RoutedEventHandler Click;

        public BitmapImage ImageUrl
        {
            get { return (BitmapImage)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }
        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.Register("ImageUrl", typeof(BitmapImage), typeof(CircleImage), new PropertyMetadata(null, (s, e) =>
            {
                var circleImage = s as CircleImage;
                if (circleImage == null)
                    return;

                var imageSource = e.NewValue as BitmapImage;
                if (imageSource == null || imageSource.UriSource == null)
                    return;

                var isWord = imageSource.UriSource.AbsoluteUri.StartsWith("ms-resource");
                if(isWord)
                {
                    circleImage.WordButton.Visibility = isWord ? Visibility.Visible : Visibility.Collapsed;
                    circleImage.ImageButton.Visibility = isWord ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    circleImage.WordButton.Visibility = isWord ? Visibility.Visible : Visibility.Collapsed;
                    circleImage.ImageButton.Visibility = isWord ? Visibility.Collapsed : Visibility.Visible;
                }
            }));

        public string Word
        {
            get { return (string)GetValue(WordProperty); }
            set { SetValue(WordProperty, value); }
        }
        public static readonly DependencyProperty WordProperty =
            DependencyProperty.Register("Word", typeof(string), typeof(CircleImage), new PropertyMetadata(""));

        private void ClickButton(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(sender, e);
        }
    }
}
