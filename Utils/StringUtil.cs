﻿#region License
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
using System;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;

namespace Brook.DuDuRiBao.Utils
{
    public static class StringUtil
    {
        static ResourceLoader _resLoader = new ResourceLoader();

        public static string GetString(string id)
        {
            return _resLoader.GetString(id);
        }

        public static string GetStoryGroupName(string currentDate)
        {
            var date = DateTime.ParseExact(currentDate, "yyyyMMdd", null);
            if (date.Date.Equals(DateTime.Now.Date))
                return GetString("LatestNews");

            return date.Month + GetString("Month") + date.Day + GetString("Day") + " " + DateToWeek(date);
        }

        public static string GetCommentGroupName(CommentType type, string count)
        {
            return count + GetString("CommentItem") + (type == CommentType.Recommend ? GetString("RecommendComment") : GetString("NormalComment"));
        }

        public static string DateToWeek(DateTime date)
        {
            switch(date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return GetString("Monday");
                case DayOfWeek.Tuesday:
                    return GetString("Tuesday");
                case DayOfWeek.Wednesday:
                    return GetString("Wednesday");
                case DayOfWeek.Thursday:
                    return GetString("Thursday");
                case DayOfWeek.Friday:
                    return GetString("Friday");
                case DayOfWeek.Saturday:
                    return GetString("Saturday");
                case DayOfWeek.Sunday:
                    return GetString("Sunday");
                default:
                    return string.Empty;
            }
        }

        public static string DisplayTime(int sec)
        {
            return DisplayTime(CalcDateFromSec(sec));
        }

        public static DateTime CalcDateFromSec(int sec)
        {
            return new DateTime(1970, 1, 1).AddSeconds(sec);
        }

        public static string DisplayTime(DateTime date)
        {
            const int MINUTE = 60;
            TimeSpan ts = DateTime.Now - date;
            double delta = ts.TotalSeconds;

            if (delta < 0)
            {
                return "";
            }

            if (delta < 1 * MINUTE)
            {
                return ts.Seconds + GetString("BeforeSec");
            }

            if (delta < 60 * MINUTE)
            {
                return ts.Minutes + GetString("BeforeMin");
            }

            return date.ToString("MM-dd HH:mm");
        }

        public static string DecodeXmlString(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return xml;

            return xml.Replace("&lt;", "<")
                             .Replace("&gt;", ">")
                             .Replace("&apos;", "&apos;")
                             .Replace("&quot;", "\"")
                             .Replace("&amp;", "&");
        }

        public static bool CheckEmail(string email)
        {
            var pattern = @"^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        public static bool CheckPhoneNum(string num)
        {
            var pattern = "^[1]\\d{10}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(num);
        }

        public static bool CompareVersion(PackageVersion verA, PackageVersion verB)
        {
            return verA.Major * 100 + verA.Minor * 10 + verA.Build > verB.Major * 100 + verB.Minor * 10 + verB.Build;
        }
    }
}
