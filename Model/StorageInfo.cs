using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Utils;
using LLQ;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Brook.DuDuRiBao.Models
{
    public class StorageInfo : INotifyPropertyChanged
    {
        static StorageInfo _instance;
        static object _lockObj = new object();
        public static StorageInfo Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_lockObj)
                    {
                        if(_instance == null && Application.Current.Resources.MergedDictionaries[0].ContainsKey("StorageInfo"))
                            _instance = (StorageInfo)Application.Current.Resources.MergedDictionaries[0]["StorageInfo"];
                    }
                    
                } 
                return _instance;
            }
        }

        public RiBaoAuthoInfo ZhiHuAuthoInfo { get; set; }

        public LoginType LoginType { get; set; } = LoginType.None;

        public bool IsCommentPanelOpen { get; set; } = false;

        public bool LazyLoadImage { get; set; } = false;

        private bool _haveNewVersion = false;
        public bool HaveNewVersion
        {
            get { return _haveNewVersion; }
            set
            {
                _haveNewVersion = value;
                Notify("HaveNewVersion");
            }
        }

        public VersionDesc NewVersion { get; set; }

        public bool NeedLazyLoadImage { get { return LazyLoadImage && NetworkUtil.IsMobile; } }

        public bool IsZhiHuAuthoVaild() { return ZhiHuAuthoInfo != null && !string.IsNullOrEmpty(ZhiHuAuthoInfo.access_token); }

        private ElementTheme _appTheme = ElementTheme.Light;
        public ElementTheme AppTheme
        {
            get { return _appTheme; }
            set
            {
                if(value != _appTheme)
                {
                    _appTheme = value;
                    LLQNotifier.Default.Notify(new StoryEvent() { Type = StoryEventType.Night, IsChecked = _appTheme == ElementTheme.Light });
                    Notify("AppTheme");
                }
            }
        }

        public void CopyValue(StorageInfo info)
        {
            ZhiHuAuthoInfo = info.ZhiHuAuthoInfo;
            LoginType = info.LoginType;
            IsCommentPanelOpen = info.IsCommentPanelOpen;
            AppTheme = info.AppTheme;
            LazyLoadImage = info.LazyLoadImage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
