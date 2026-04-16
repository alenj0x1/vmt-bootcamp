namespace TalentInsights.Shared.Constants
{
	public static class ValidationConstants
	{
		public const string MAX_LENGTH = "El máximo de caracteres de {0} es {1}";
		public const string MIN_LENGTH = "El mínimo de caracteres de {0} es {1}";
		public const string REQUIRED = "La propiedad {0} es requerida";

		public const string EMAIL_ADDRESS = "La dirección de correo electrónico, no es correcta {0}";

		public const string VALIDATION_MESSAGE = "Una o más validaciones necesitan atención";

		public static string IsEmpty(string property) => $"El valor de la propiedad '{property}' es vacio. En casos de UUID, no está admitido '00000000-0000-0000-0000-000000000000'";
	}
}
