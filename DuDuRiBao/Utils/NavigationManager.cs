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
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace DuDuRiBao.Utils
{
    public class NavigationManager
    {
        public readonly static NavigationManager Instance = new NavigationManager();

        private Stack<Frame> _frameStack = new Stack<Frame>();

        private NavigationManager() { }

        public void Navigate(Frame frame, Type pageType)
        {
            _frameStack.Push(frame);
            frame.Navigate(pageType);
        }

        public void Navigate(Frame frame, Type pageType, object param)
        {
            _frameStack.Push(frame);
            frame.Navigate(pageType, param);
        }

        public void Navigate(Frame frame, Type pageType, object param, NavigationTransitionInfo infoOverride)
        {
            _frameStack.Push(frame);
            frame.Navigate(pageType, param, infoOverride);
        }

        public bool CanGoBack { get { return _frameStack.Count > 0; } }

        public void GoBack(BackRequestedEventArgs e)
        {
            var currFrame = _frameStack.Pop();
            if (currFrame == null)
                return;

            if (currFrame.CanGoBack)
            {
                currFrame.GoBack();
                e.Handled = true;
                UpdateGoBackBtnVisibility();
            }
            else
            {
                GoBack(e);
            }
        }

        public void UpdateGoBackBtnVisibility()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}
