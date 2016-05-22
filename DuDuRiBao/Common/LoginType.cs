using System;

namespace Brook.DuDuRiBao.Common
{
    public enum LoginType
    {
        None = 0,
        Sina,
        QQ,
        ZhiHu
    }

    public static class LoginTypeClass
    {
        public const string NoneLoginType = "None";
        public const string SinaLoginType = "sina";
        public const string QQLoginType = "qq";
        public const string ZhiHuLoginType = "zhihu";

        public static string Convert(this LoginType loginType)
        {
            switch(loginType)
            {
                case LoginType.Sina:
                    return SinaLoginType;
                case LoginType.QQ:
                    return QQLoginType;
                case LoginType.ZhiHu:
                    return ZhiHuLoginType;
                default:
                    throw new NotSupportedException();
            }
        }

        public static LoginType ToEnum(string loginType)
        {
            switch(loginType)
            {
                case SinaLoginType:
                    return LoginType.Sina;
                case QQLoginType:
                    return LoginType.QQ;
                case ZhiHuLoginType:
                    return LoginType.ZhiHu;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
