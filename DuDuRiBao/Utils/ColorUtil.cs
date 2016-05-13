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
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Brook.DuDuRiBao.Utils
{
    public class ColorUtil
    {
        static string[] _colorPool = new string[] { "#4db6ac", "#ff8964", "#90a4ae", "#9575cd", "#aed581", "#9e9e9e", "#4dd0e1", "#a1887f" };

        public static string GetColorStringByCircleId(int circleId)
        {
            return _colorPool[circleId % _colorPool.Length];
        }

        public static Color GetColorByCircleId(int circleId)
        {
            return GetColorFromHexString(GetColorStringByCircleId(circleId));
        }

        public static Brush GetBrushByCircleId(int circleId)
        {
            return new SolidColorBrush(GetColorByCircleId(circleId));
        }

        public static Color GetColorFromHexString(string hexValue)
        {
            var r = Convert.ToByte(hexValue.Substring(1, 2), 16);
            var g = Convert.ToByte(hexValue.Substring(3, 2), 16);
            var b = Convert.ToByte(hexValue.Substring(4, 2), 16);
            return Color.FromArgb(255, r, g, b);
        }
    }
}
