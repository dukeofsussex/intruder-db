// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using API.Models;
    using API.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("authenticate")]
    public class AuthController : Controller
    {
        private IConfiguration config;
        private Uri openIDUri = new Uri("https://steamcommunity.com/openid/login");
        private Regex steamIDRegex = new Regex("^https:\\/\\/steamcommunity\\.com\\/openid\\/id\\/(7[0-9]{15,25})$", RegexOptions.Compiled);
        private Regex validityRegex = new Regex("is_valid:(.+)", RegexOptions.Compiled);

        public AuthController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Auth(
            [FromQuery]string claimedId,
            [FromQuery]string identity,
            [FromQuery]string responseNonce,
            [FromQuery]string assocHandle,
            [FromQuery]string returnTo,
#pragma warning disable CA1720 // Identifier contains type name
            [FromQuery]string signed,
#pragma warning restore CA1720 // Identifier contains type name
            [FromQuery]string sig)
        {
            long steamID = long.Parse(this.steamIDRegex.Match(identity).Groups[1].Value, CultureInfo.InvariantCulture);

            bool blacklisted = DB.Get(
                "SELECT EXISTS(SELECT steam_id FROM blacklist WHERE steam_id = @steamID) AS blacklisted",
                new Dictionary<string, dynamic>() { { "@steamID", steamID } },
                (x) => Create.Bool(x, "blacklisted"));

            if (blacklisted == true)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.Forbidden,
                    Message = "This Steam account has been blacklisted from using this site"
                });
            }

            string response = await Services.Request.PostAsync(
                this.openIDUri,
                new Dictionary<string, string>()
                {
                    { "openid.ns", "http://specs.openid.net/auth/2.0" },
                    { "openid.mode", "check_authentication" },
                    { "openid.claimed_id", claimedId },
                    { "openid.identity", identity },
                    { "openid.response_nonce", responseNonce.Replace(' ', '+') },
                    { "openid.op_endpoint", "https://steamcommunity.com/openid/login" },
                    { "openid.return_to", returnTo },
                    { "openid.assoc_handle", assocHandle },
                    { "openid.signed", signed },
                    { "openid.sig", sig.Replace(' ', '+') }
                }).ConfigureAwait(false);

            Match match = this.validityRegex.Match(response);

            if (!match.Success || Convert.ToBoolean(match.Groups[1].Value, CultureInfo.InvariantCulture) == false)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Unable to authenticate user with the provided Steam details"
                });
            }

            return this.GetToken(steamID);
        }

        [HttpGet("token")]
        public IActionResult GetToken(long steamID = 0)
        {
            ClaimsPrincipal user = this.HttpContext.User;

            if (steamID == 0 && !user.HasClaim(c => c.Type == "SteamID"))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "SteamID is missing, unable to provide a token"
                });
            }
            else if (steamID == 0)
            {
                steamID = long.Parse(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "SteamID").Value, CultureInfo.InvariantCulture);
            }

            AgentProfile agent = DB.Get(
                "SELECT id AS profile_id, steam_id AS profile_steam_id, name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url"
                + " FROM agent WHERE steam_id = @steamID",
                new Dictionary<string, dynamic>()
                {
                    { "@steamID", steamID }
                },
                Create.AgentProfile);

            if (agent == null)
            {
                agent = new AgentProfile();
            }

            string tokenString = this.BuildToken(agent.ID, steamID);

            return this.Ok(new { profile = agent, token = tokenString });
        }

        private string BuildToken(int agentID, long steamID)
        {
            System.Security.Claims.Claim[] claims = new[]
            {
                new System.Security.Claims.Claim("AgentID", agentID.ToString(CultureInfo.InvariantCulture)),
                new System.Security.Claims.Claim("SteamID", steamID.ToString(CultureInfo.InvariantCulture))
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["JWT:Key"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                this.config["JWT:Issuer"],
                this.config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
