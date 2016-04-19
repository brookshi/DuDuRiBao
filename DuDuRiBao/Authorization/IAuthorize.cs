using Brook.DuDuRiBao.Models;
using System;

namespace Brook.DuDuRiBao.Authorization
{
    public interface IAuthorize
    {
        void Login(Action<bool, object> loginCallback);

        void Logout();

        LoginData LoginData { get; }

        bool IsAuthorized { get; }
    }
}
