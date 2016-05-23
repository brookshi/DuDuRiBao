using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using static ZhiHu.Controls.Utils;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace ZhiHu.Controls
{
    public sealed class ZhiHuPivot : Pivot
    {
        #region Field

        private TranslateTransform _itemsPresenterTranslateTransform;

        private ScrollViewer _scrollViewer;

        private Line _tipLine;

        private TranslateTransform _tipLineTranslateTransform;

        private double _previsousOffset;

        #endregion

        #region Dependency Property

        public double HeaderWidth
        {
            get { return (double)GetValue(HeaderWidthProperty); }
            set { SetValue(HeaderWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderWidthProperty =
            DependencyProperty.Register("HeaderWidth", typeof(double), typeof(ZhiHuPivot), new PropertyMetadata(80.0));

        public double BackgroundLineStokeThickness
        {
            get { return (double)GetValue(BackgroundLineStokeThicknessProperty); }
            set { SetValue(BackgroundLineStokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundLineStokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundLineStokeThicknessProperty =
            DependencyProperty.Register("BackgroundLineStokeThickness", typeof(double), typeof(ZhiHuPivot), new PropertyMetadata(2.0));


        public Brush BackgroundLineStoke
        {
            get { return (Brush)GetValue(BackgroundLineStokeProperty); }
            set { SetValue(BackgroundLineStokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundLineStoke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundLineStokeProperty =
            DependencyProperty.Register("BackgroundLineStoke", typeof(Brush), typeof(ZhiHuPivot), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));


        public double IndicatorLineStokeThickness
        {
            get { return (double)GetValue(IndicatorLineStokeThicknessProperty); }
            set { SetValue(IndicatorLineStokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForegroundLineStokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorLineStokeThicknessProperty =
            DependencyProperty.Register(nameof(IndicatorLineStokeThickness), typeof(double), typeof(ZhiHuPivot), new PropertyMetadata(2.0));



        public Brush IndicatorLineStroke
        {
            get { return (Brush)GetValue(IndicatorLineStrokeProperty); }
            set { SetValue(IndicatorLineStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForegroundLineStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorLineStrokeProperty =
            DependencyProperty.Register(nameof(IndicatorLineStroke), typeof(Brush), typeof(ZhiHuPivot), new PropertyMetadata(new SolidColorBrush(Colors.Red)));


        #endregion

        public ZhiHuPivot()
        {
            this.DefaultStyleKey = typeof(ZhiHuPivot);

            if (!DesignMode.DesignModeEnabled)
                this.Loaded += ZhiHuPivot_Loaded;
        }


        private void ZhiHuPivot_Loaded(object sender, RoutedEventArgs e)
        {
            if (Items.Count > 1)
            {
                var res = ScreenSize().Width / Items.Count;
                if (IsMobile)
                    HeaderWidth = res;
                _tipLine.X2 = HeaderWidth;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsPresenterTranslateTransform = GetTemplateChild<TranslateTransform>("ItemsPresenterTranslateTransform");
            if (_itemsPresenterTranslateTransform != null)
            {
                _itemsPresenterTranslateTransform.RegisterPropertyChangedCallback(TranslateTransform.XProperty, Callback);
            }
            _scrollViewer = GetTemplateChild<ScrollViewer>("ScrollViewer");
            if (_scrollViewer != null)
            {
                _scrollViewer.RegisterPropertyChangedCallback(ScrollViewer.HorizontalOffsetProperty, HorizontalOffsetCallback);
            }
            _tipLine = GetTemplateChild<Line>("TipLine");
            _tipLineTranslateTransform = GetTemplateChild<TranslateTransform>("TipLineTranslateTransform");
        }

        private void HorizontalOffsetCallback(DependencyObject sender, DependencyProperty dp)
        {
            if (_previsousOffset != 0)
            {
                var x = (double)sender.GetValue(dp);
                var right = x > _previsousOffset;

                if (right)
                {
                    // 非边界
                    if (SelectedIndex + 1 != Items.Count)
                    {
                        var newX = (x - _previsousOffset) / Items.Count + (SelectedIndex * HeaderWidth);
                        var max = (SelectedIndex + 1) * HeaderWidth;

                        _tipLineTranslateTransform.X = newX < max ? newX : max;
                    }
                    else
                    {
                        _tipLineTranslateTransform.X = (SelectedIndex * HeaderWidth) - (x - _previsousOffset);
                    }
                }
                else
                {
                    // 非边界
                    if (SelectedIndex != 0)
                    {
                        var newX = (x - _previsousOffset) / Items.Count + (SelectedIndex * HeaderWidth);
                        var max = (SelectedIndex + 1) * HeaderWidth;

                        _tipLineTranslateTransform.X = newX < max ? newX : max;
                    }
                    else
                    {
                        _tipLineTranslateTransform.X = _previsousOffset - x;
                    }
                }
            }
        }

        private void Callback(DependencyObject sender, DependencyProperty dp)
        {
            _previsousOffset = (double)sender.GetValue(dp);
            _tipLineTranslateTransform.X = (SelectedIndex * HeaderWidth);
        }

        private T GetTemplateChild<T>(string name) where T : DependencyObject => GetTemplateChild(name) as T;
    }


    public static class Utils
    {
        public static bool IsMobile => Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons") ? true : false;

        public static Size ScreenSize()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            return new Size(bounds.Width, bounds.Height);
        }
    }
}
