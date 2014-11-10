
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJ.SecureTokenService.Contracts;

namespace JJ.SecureTokenService.RequestContext
{
    /// <summary>
    /// Used to provide all necessary properties to build a Request Context for STS
    /// </summary>
    interface ITokenServiceRequestContext
    {
        string RelyingParty { get;set; }

        double Version { get; set; }

        Guid? CorrelationId { get; set; }

        TokenRequestBase TokenRequestBody { get; set; }
    }
}
