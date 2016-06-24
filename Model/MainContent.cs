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

using System.Collections.Generic;

namespace Brook.DuDuRiBao.Models
{
    public class Section
    {
        public string Thumbnail { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class MainContent
    {
        public string Body { get; set; }
        public string Image_Source { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Share_Url { get; set; }
        public string External_Url { get; set; }
        public List<object> Js { get; set; }
        public string Ga_Prefix { get; set; }
        public Section Section { get; set; }
        public List<string> Images { get; set; }
        public int Type { get; set; }
        public int Id { get; set; }
        public List<string> Css { get; set; }
    }
}
