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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Brook.DuDuRiBao.Common
{
    public abstract class DialogBase : ContentDialog
    {
        public DialogBase()
        {
            Loaded += DialogBase_Loaded;
            Tapped += DialogBase_Tapped; ;
        }

        private void DialogBase_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //to invalid rectangle's tap event
        }

        private void DialogBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
            Load();
        }

        protected virtual void Load() { }
    }
}
