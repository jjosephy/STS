
using JJ.SecureTokenService.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JJ.SecureTokenService.Tests.AuthenticationTests
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void ConfigurationTest_TestCheckConfiguration()
        {
            var config = System.Configuration.ConfigurationManager.GetSection("services") as ServiceLocatorSection;
            foreach ( var el in config.Instances )
            {
                var element = el as ServiceInstanceElement;
                if (element != null )
                {
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(element.Assembly), "Assembly is null");
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(element.Type), "Type is null");
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(element.ProviderType), "Provider Type is null");
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(element.Name), "Name is null");
                    
                    element.Assembly = element.Assembly;
                    element.Type = element.Type;
                    element.ProviderType = element.ProviderType;
                    element.Name = element.Name;
                }
            }
        }
    }
}
