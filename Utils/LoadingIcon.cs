using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Brook.DuDuRiBao.Utils
{
    public class LoadingIcon
    {
        private static Popup _popup = new Popup();
        private static ProgressRing _ring = new ProgressRing();

        static LoadingIcon()
        {
            _ring.Width = 100;
            _ring.Height = 100;
            _ring.IsActive = true;

            _popup.Child = _ring;
            _popup.RenderTransform = new TranslateTransform();
            _popup.IsOpen = false;
            UpdatePosition();
        }

        public static void Display()
        {
            _popup.IsOpen = true;
        }

        public static void Hide()
        {
            _popup.IsOpen = false;
        }

        private static void UpdatePosition()
        {
            MeasureRing();

            var window = Window.Current.CoreWindow;
            _popup.HorizontalOffset = window.Bounds.Width / 2 - _ring.ActualWidth / 2;
            _popup.VerticalOffset = window.Bounds.Height / 2 - _ring.ActualWidth / 2;
        }

        private static void MeasureRing()
        {
            var size = new Size(1000, 1000);
            _ring.Measure(size);
            _ring.Arrange(new Rect(new Point(0, 0), size));
        }
    }
}
