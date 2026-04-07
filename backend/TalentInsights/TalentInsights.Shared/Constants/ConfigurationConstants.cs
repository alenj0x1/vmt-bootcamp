namespace TalentInsights.Shared.Constants
{
	public static class ConfigurationConstants
	{
		// First app
		public const string FIRST_APP_TIME_USER_FULLNAME = "FirstAppTime:User:FullName";
		public const string FIRST_APP_TIME_USER_EMAIL = "FirstAppTime:User:Email";
		public const string FIRST_APP_TIME_USER_PASSWORD = "FirstAppTime:User:Password";
		public const string FIRST_APP_TIME_USER_POSITION = "FirstAppTime:User:Position";

		// Connection strings
		public const string CONNECTION_STRING_DATABASE = "ConnectionStrings:Database";

		// JWT
		public const string JWT_PRIVATE_KEY = "Jwt:PrivateKey";
		public const string JWT_AUDIENCE = "Jwt:Audience";
		public const string JWT_ISSUER = "Jwt:Issuer";
		public const string JWT_EXPIRATION_IN_MINUTES = "Jwt:ExpirationInMinutes";
	}
}
