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
    public class User
    {
        public int Id { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
    }

    public class ReplyTo
    {
        public string Content { get; set; }
        public int Status { get; set; }
        public User User { get; set; }
        public int Id { get; set; }
        public string Author { get; set; }
    }

    public class Circle
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Comment
    {
        public Circle Circle { get; set; }
        public string Content { get; set; }
        public bool Own { get; set; }
        public User User { get; set; }
        public int Time { get; set; }
        public bool Voted { get; set; }
        public int Id { get; set; }
        public int Likes { get; set; }
        public ReplyTo Reply_To { get; set; }
    }

    public class CommentList
    {
        public List<Comment> Comments { get; set; }
    }

    public class GroupComments : ObservableCollectionExtended<Comment>
    {
        private string _groupName = "";
        public string GroupName
        {
            get { return _groupName; }
            set
            {
                if (value != _groupName)
                {
                    _groupName = value;
                    Notify("GroupName");
                }
            }
        }

        protected void Notify(string property)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(property));
        }
    }
}
