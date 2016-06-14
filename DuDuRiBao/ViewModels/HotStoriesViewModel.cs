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

using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.ViewModels
{
    public partial class HotStoriesViewModel : ViewModelBase
    {
        private string _hotStories;
        public string HotStories
        {
            get { return _hotStories; }
            set
            {
                if (value != _hotStories)
                {
                    _hotStories = value;
                    Notify("HotStories");
                }
            }
        }

        public string Title
        {
            get { return StringUtil.GetString("HotStory"); }
        }

        public override void Init()
        {
            RefreshHotStories();
        }

        public async void RefreshHotStories()
        {
            HotStories = await DataRequester.RequestHotStories();
        }
    }
}
