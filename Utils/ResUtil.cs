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

using DuDuRiBao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Brook.DuDuRiBao.Utils
{
    public class ResUtil
    {
        public static Brush GetAppThemeBrush(string key)
        {
            return Application.Current.Resources[key] as Brush;
        }

        const double _scrollBarWidth = 8;

        public static void SetScrollBarWidth(DependencyObject target)
        {
            SetScrollBarWidth(target, _scrollBarWidth);
        }

        public static void SetScrollBarWidth(DependencyObject target, double width)
        {
            var thumb = VisualHelper.FindVisualChild<ScrollBar>(target, "VerticalScrollBar");
            if (thumb != null)
            {
                thumb.MaxWidth = thumb.MinWidth = width;
            }
        }

        public static async void SetBase64ToImage(BitmapSource imageSource, string base64Str)
        {
            var imgBytes = Convert.FromBase64String(base64Str);
            var ms = new InMemoryRandomAccessStream();
            var dw = new DataWriter(ms);
            dw.WriteBytes(imgBytes);
            await dw.StoreAsync();
            ms.Seek(0);
            imageSource.SetSource(ms);
        }
    }
}
