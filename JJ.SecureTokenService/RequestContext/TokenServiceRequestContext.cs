
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JJ.SecureTokenService.Contracts;

namespace JJ.SecureTokenService.RequestContext
{
    internal class TokenServiceRequestContext : ITokenServiceRequestContext
    {
        public TokenServiceRequestContext()
        {
        }

        public TokenServiceRequestContext(
            string relyingParty,
            double version,
            Guid correlationId,
            TokenRequestBase tokenRequest)
        {
            this.RelyingParty = relyingParty;
            this.CorrelationId = correlationId;
            this.Version = version;
            this.TokenRequestBody = tokenRequest;
        }

        public TokenServiceRequestContext(ITokenServiceRequestContext context)
        {
            this.Version = context.Version;
        }

        public double Version
        {
            get;
            set;
        }

        public Guid? CorrelationId 
        { 
            get; 
            set; 
        }

        public TokenRequestBase TokenRequestBody
        {
            get;
            set;
        }

        public string RelyingParty
        {
            get;
            set;
        }
    }
}