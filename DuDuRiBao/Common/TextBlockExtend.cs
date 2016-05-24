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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Brook.DuDuRiBao.Utils
{
    public class TextBlockExtend : DependencyObject
    {
        const string Flag_Begin = "<em>";
        const string Flag_End = "</em>";
        const string Pattern = @"<em>.*?</em>";
        static Regex _regex = new Regex(Pattern);

        public string ColorfulText
        {
            get { return (string)GetValue(ColorfulTextProperty); }
            set { SetValue(ColorfulTextProperty, value); }
        }
        public static readonly DependencyProperty ColorfulTextProperty =
            DependencyProperty.Register("ColorfulText", typeof(string), typeof(TextBlockExtend), new PropertyMetadata("", (s, e)=>
            {
                var textBlock = s as TextBlock;
                if (textBlock == null || e.NewValue == null || string.IsNullOrEmpty(e.NewValue.ToString()))
                    return;

                textBlock.Inlines.Clear();
                var values = _regex.Split(e.NewValue.ToString());
                foreach (var value in values)
                {
                    var newTextBlock = new TextBlock();
                    if(value.Contains(Flag_Begin))
                    {
                        newTextBlock.Text = value.Replace(Flag_Begin, "").Replace(Flag_End, "");
                        newTextBlock.Foreground = (Brush)textBlock.GetValue(HighLightColorProperty);
                    }
                    else
                    {
                        newTextBlock.Text = value;
                        newTextBlock.Foreground = textBlock.Foreground;
                    }
                    newTextBlock.FontSize = textBlock.FontSize;
                    var inline = new InlineUIContainer() { Child = newTextBlock };
                    textBlock.Inlines.Add(inline);
                }
            }));

        public Brush HighLightColor
        {
            get { return (Brush)GetValue(HighLightColorProperty); }
            set { SetValue(HighLightColorProperty, value); }
        }
        public static readonly DependencyProperty HighLightColorProperty =
            DependencyProperty.Register("HighLightColor", typeof(Brush), typeof(TextBlockExtend), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



    }
}
