using Brook.DuDuRiBao.Models;
using System;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.Authorization
{
    public interface IAuthorize
    {
        Task Login(ZhiHuLoginInfo info, Action<RiBaoAuthoInfo> loginCallback);

        void Logout();

        TokenInfo TokenInfo { get; }

        bool IsAuthorized { get; }
    }
}
