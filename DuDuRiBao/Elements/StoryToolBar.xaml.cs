﻿using System;
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
using XP;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Brook.DuDuRiBao.Elements
{
    public sealed partial class StoryToolBar : UserControl
    {
        public StoryToolBar()
        {
            this.InitializeComponent();
        }

        public int CommentCount
        {
            get { return (int)GetValue(CommentCountProperty); }
            set { SetValue(CommentCountProperty, value); }
        }
        public static readonly DependencyProperty CommentCountProperty =
            DependencyProperty.Register("CommentCount", typeof(int), typeof(StoryToolBar), new PropertyMetadata(0));

        public int LikeCount
        {
            get { return (int)GetValue(LikeCountProperty); }
            set { SetValue(LikeCountProperty, value); }
        }
        public static readonly DependencyProperty LikeCountProperty =
            DependencyProperty.Register("LikeCount", typeof(int), typeof(StoryToolBar), new PropertyMetadata(0));
    }
}
