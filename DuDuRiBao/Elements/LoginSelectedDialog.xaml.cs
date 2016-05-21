using DuDuRiBao.Utils;
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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


namespace Brook.DuDuRiBao.Elements
{
    public sealed partial class LoginSelectedDialog : ContentDialog
    {
        public LoginSelectedDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            if (parent != null)
            {
                var rectangle = VisualHelper.FindVisualChild<Rectangle>(parent);
                if (rectangle != null)
                {
                    rectangle.Tapped += (s, a) => { Hide(); };
                }
            }
        }

        private void ContentDialog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //to invalid rectangle's tap event
        }

        private void OnZhiHuLogin(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void OnWeiBoLogin(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
