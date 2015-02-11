using System;
using System.IO;
using System.Linq;

namespace Khet
{
	public static class Utils
	{
		public static string MainPath
		{
			get {
				return (new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)).ToString();
			}
		}

		public static string ResourcePath(string path)
		{
			return Path.Combine( MainPath,
                ("Resources/" + path).Split('/').Aggregate(
					(acc, i) => Path.Combine(acc, i)
				)
			);
		}

		public static string ReadFile(string path)
		{
			try {
				return File.ReadAllText(path);
			} catch {
				Console.Error.WriteLine("Failed to read text from file: " + path);
				throw;
			}
		}
	}
}
