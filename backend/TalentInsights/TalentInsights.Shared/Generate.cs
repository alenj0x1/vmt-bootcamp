using System.Text;

namespace TalentInsights.Shared
{
	public static class Generate
	{
		private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVXYWZabcdefghijklmnopqrstuvxywz0123456789";

		public static string RandomText(int length = 50)
		{
			var sb = new StringBuilder();
			var rnd = new Random();

			for (int i = 0; i < length; i++)
			{
				sb.Append(Characters[rnd.Next(0, Characters.Length - 1)]);
			}

			return sb.ToString();
		}
	}
}
