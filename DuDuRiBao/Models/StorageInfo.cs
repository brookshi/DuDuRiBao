using Brook.DuDuRiBao.Common;

namespace Brook.DuDuRiBao.Models
{
    public class StorageInfo
    {
        public ZhiHuAuthoInfo ZhiHuAuthoInfo { get; set; }

        public LoginType LoginType { get; set; } = LoginType.None;

        public bool IsCommentPanelOpen { get; set; } = false;

        public bool IsZhiHuAuthoVaild() { return ZhiHuAuthoInfo != null && !string.IsNullOrEmpty(ZhiHuAuthoInfo.access_token); }
    }
}
