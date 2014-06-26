using System.IO;

namespace Test.Commons
{
	public static class TestHelper
	{
		public static string ReadFrom(string fileName)
		{
			string content;
			using (var fs = new FileStream(fileName, FileMode.Open))
			{
				using (var sr = new StreamReader(fs))
				{
					content = sr.ReadToEnd();
				}
			}
			return content;
		}
	}
}
