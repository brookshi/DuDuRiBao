using Brook.DuDuRiBao.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Media;

namespace Brook.DuDuRiBao.Models
{
    public abstract class CircleBase
    {
        public Brush BackgroundBrush { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public string Id { get; set; }

        public virtual void Adjust()
        {
            if (string.IsNullOrEmpty(Thumbnail) && !string.IsNullOrEmpty(Name))
            {
                if(Name.StartsWith("<em>") && Name.Length > 4)
                    Thumbnail = Name[4].ToString();
                else
                    Thumbnail = Name[0].ToString();
            }
            BackgroundBrush = ColorUtil.GetBrushByCircleId(int.Parse(Id));
        }

        public abstract HotCircle Clone();
    }

    public class HotCircle : CircleBase
    {
        public string Articles { get; set; }

        public string Fans { get; set; }

        public override HotCircle Clone()
        {
            return new HotCircle()
            {
                Thumbnail = Thumbnail,
                Id = Id,
                Articles = Articles,
                Fans = Fans,
                BackgroundBrush = BackgroundBrush,
                Name = Name
            };
        }
    }
}
