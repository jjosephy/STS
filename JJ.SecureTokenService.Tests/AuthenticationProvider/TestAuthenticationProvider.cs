using JJ.SecureTokenService.Authentication;
using System.Threading.Tasks;

namespace JJ.SecureTokenService.Tests.AuthenticationProvider
{
    public class TestAuthenticationProvider : IAuthenticationProvider
    {
        public TestAuthenticationProvider()
        { }

        public Task<bool> IsAuthenticatedAsync(string userName, string password)
        {
            return Task.Run(() =>
            {
                if (userName.Equals("unauthorized@test.com", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                return true;
            });
        }
    }
}
