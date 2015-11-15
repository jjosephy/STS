using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.SecureTokenService.Authentication
{
    public interface IAuthenticationProvider
    {
        Task<bool> IsAuthenticatedAsync(string userName, string password);
    }
}
