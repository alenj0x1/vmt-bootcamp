using Microsoft.Extensions.Configuration;
using TalentInsights.Application.Models.Helpers;
using TalentInsights.Shared.Constants;

namespace TalentInsights.Application.Helpers
{
	public static class CacheHelper
	{
		public static readonly Random rnd = new();

		public static string AuthTokenKey(string value)
		{
			return $"auth:tokens:{value}";
		}

		public static CacheKey AuthTokenCreation(string value, TimeSpan expiration)
		{
			return new CacheKey
			{
				Key = AuthTokenKey(value),
				Expiration = expiration
			};
		}

		public static string AuthRefreshTokenKey(string value)
		{
			return $"auth:refresh_tokens:{value}";
		}

		public static CacheKey AuthRefreshTokenCreation(string value, IConfiguration configuration)
		{
			return new CacheKey
			{
				Key = AuthRefreshTokenKey(value),
				Expiration = TimeSpan.FromDays(Convert.ToInt32(configuration[ConfigurationConstants.AUTH_REFRESH_TOKEN_EXPIRATION_IN_DAYS] ?? "15"))
			};
		}
	}
}
