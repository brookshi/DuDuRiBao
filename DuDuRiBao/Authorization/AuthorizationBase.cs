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

using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.Authorization
{
    public abstract class AuthorizationBase : IAuthorize
    {
        protected const string LoginDataKey = "TokenInfo";
        public TokenInfo TokenInfo { get; protected set; }

        public AuthorizationBase()
        {
            TokenInfo tokenInfo;
            if (StorageUtil.TryGetJsonObj(LoginDataKey, out tokenInfo))
            {
                TokenInfo = tokenInfo;
            }
        }

        public abstract bool IsAuthorized { get; }

        public abstract Task Login(ZhiHuLoginInfo info, Action<RiBaoAuthoInfo> loginCallback);

        public virtual void Logout()
        {
            StorageUtil.Remove(LoginDataKey);
        }

        protected void StoreTokenInfo()
        {
            StorageUtil.AddObject(LoginDataKey, TokenInfo);
        }
    }
}
