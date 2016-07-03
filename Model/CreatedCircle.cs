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
using System.Collections.Generic;
using System.ComponentModel;

namespace Brook.DuDuRiBao.Models
{
    public class CreatedCircle
    {
        public string description { get; set; }
        public string name { get; set; }
    }

    public class CreatedCircleMessage
    {
        public int Status { get; set; }
        public string Error_Msg { get; set; }
        public int Id { get; set; }
    }

    public class PostedToCircle
    {
        public List<int> circle_ids { get; set; }
        public string reason { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }

    public class OwnCircleCount
    {
        public int Stories { get; set; }
        public int Editors { get; set; }
        public int Members { get; set; }
    }

    public class OwnCircleInfo
    {
        public OwnCircleCount Count { get; set; }
        public string Thumbnail { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class OwnCircles
    {
        public List<OwnCircleInfo> Circles { get; set; }
    }
}
