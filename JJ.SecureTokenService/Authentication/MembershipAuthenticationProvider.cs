using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace JJ.SecureTokenService.Authentication
{
    public class MembershipAuthenticationProvider : IAuthenticationProvider
    {
        public MembershipAuthenticationProvider()
        { }

        public Task<bool> IsAuthenticatedAsync(string userName, string password)
        {
            return Task.Run(()=>
            {
                return Membership.ValidateUser(userName, password);
            });
        }
    }
}