using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Brook.DuDuRiBao.Models
{
    public class Storager
    {
        private const string StorageInfoKey = "StorageInfo";

        private const string TileBeInitedKey = "TileBeInited";

        static ApplicationDataContainer _localSetting = ApplicationData.Current.LocalSettings;

        static Storager()
        {
            StorageInfo StorageInfo;
            if (TryGetJsonObj(StorageInfoKey, out StorageInfo))
            {
                StorageInfo.Instance.CopyValue(StorageInfo);
            }
        }

        public static void UpdateStorageInfo()
        {
            if (StorageInfo.Instance == null)
                return;

            AddObject(StorageInfoKey, StorageInfo.Instance);
        }

        public static void SetCommentPanelStatus(bool isOpen)
        {
            StorageInfo.Instance.IsCommentPanelOpen = isOpen;
            UpdateStorageInfo();
        }

        public static void SetAppTheme(ElementTheme theme)
        {
            StorageInfo.Instance.AppTheme = theme;
            UpdateStorageInfo();
        }

        public static bool IsTileInited()
        {
            int value = 0;
            if (TryGet(TileBeInitedKey, out value))
            {
                return value == 1;
            }

            return false;
        }

        public static void SetTileInited()
        {
            Add(TileBeInitedKey, "1");
        }

        public static void Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            _localSetting.Values[key] = value;
        }

        public static void AddObject(string key, object value)
        {
            if (string .IsNullOrEmpty(key) || value == null)
                return;
            _localSetting.Values[key] = JsonSerializer.Serialize(value);
        }

        public static bool TryGet(string key, out string value)
        {
            if (!string.IsNullOrEmpty(key) && _localSetting.Values.ContainsKey(key))
            {
                value = _localSetting.Values[key].ToString();
                return true;
            }

            value = null;
            return false;
        }

        public static bool TryGet(string key, out int value)
        {
            if (!string.IsNullOrEmpty(key) && _localSetting.Values.ContainsKey(key))
            {
                bool ret = int.TryParse(_localSetting.Values[key].ToString(), out value);
                return ret;
            }

            value = -1;
            return false;
        }

        public static bool TryGetJsonObj<T>(string key, out T value) where T : class
        {
            if (!string.IsNullOrEmpty(key) && _localSetting.Values.ContainsKey(key))
            {
                try {
                    var content = _localSetting.Values[key].ToString();
                    value = JsonSerializer.Deserialize<T>(content);
                }
                catch(Exception)
                {
                    value = default(T);
                    return false;
                }
                return true;
            }

            value = default(T);
            return false;
        }

        public static void Remove(string key)
        {
            if(!string.IsNullOrEmpty(key) && _localSetting.Values.ContainsKey(key))
            {
                _localSetting.Values.Remove(key);
            }
        }
    }
}
