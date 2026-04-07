namespace TalentInsights.Shared.Constants
{
	public static class ResponseConstants
	{
		// Collaborators
		public const string COLLABORATOR_NOT_EXISTS = "El colaborador no existe";

		// Projects
		public const string PROJECT_NOT_EXISTS = "El proyecto no existe";

		public static string ErrorUnexpected(string traceId)
		{
			return $"Ha ocurrido un error inesperado: Contacto con soporte, mencionando el siguiente código de error: {traceId}";
		}

		public static string ConfigurationPropertyNotFound(string property)
		{
			return $"Falta la propiedad '{property}' por establecer en la configuración del aplicativo.";
		}
	}
}
