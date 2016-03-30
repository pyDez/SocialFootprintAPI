using iRocks.AI;
using iRocks.WebAPI.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using iRocks.DataLayer;
using System.Linq;
using System.Text;

namespace iRocks.WebAPI.Helpers
{
    public interface IAuthenticationHelper
    {

        Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken);

        JObject GenerateLocalAccessTokenResponse(AppUser user);
        
       
    }
}