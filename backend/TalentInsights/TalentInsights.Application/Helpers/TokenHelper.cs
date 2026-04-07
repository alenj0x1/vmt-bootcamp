using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentInsights.Domain.Database.SqlServer.Entities;
using TalentInsights.Shared.Constants;
using TalentInsights.Shared.Helpers;

namespace TalentInsights.Application.Helpers
{
	public static class TokenHelper
	{
		public static string Create(Collaborator collaborator, IConfiguration configuration)
		{
			var issuer = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_ISSUER)
				?? configuration[ConfigurationConstants.JWT_ISSUER]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_ISSUER));

			var audience = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_AUDIENCE)
				?? configuration[ConfigurationConstants.JWT_AUDIENCE]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_AUDIENCE));

			var privateKey = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_PRIVATE_KEY)
				?? configuration[ConfigurationConstants.JWT_PRIVATE_KEY]
				?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_PRIVATE_KEY));

			var expirationInMinutes = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_EXPIRATION_IN_MINUTES)
				?? configuration[ConfigurationConstants.JWT_EXPIRATION_IN_MINUTES]
				?? "10";

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
			var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var now = DateTimeHelper.UtcNow();
			var expiration = now.AddMinutes(Convert.ToDouble(expirationInMinutes));

			var claims = new[]
			{
				new Claim(ClaimsConstants.COLLABORATOR_ID, collaborator.Id.ToString())
			};

			var token = new JwtSecurityToken(
				audience: audience,
				issuer: issuer,
				expires: expiration,
				signingCredentials: signingCredentials,
				claims: claims
				);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
