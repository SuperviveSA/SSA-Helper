namespace Host {
	/// <summary>
	/// A helper class for reading .env files.
	/// </summary>
	public static class DotEnv {
		/// <summary>
		/// Reads a .env file and set it's content as the environment variables.
		/// </summary>
		/// <param name="filePath">path to the .env file</param>
		public static void Load(string filePath = ".env") {
			if (!File.Exists(filePath)) return;

			foreach (string line in File.ReadAllLines(filePath)) {
				string[] parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

				// Skip empty/malformed lines
				if (parts.Length <= 1) continue;

				// Skip commented out lines
				if (parts[0].StartsWith('#')) continue;

				// Join the rest of the parts in case the value has a '='
				string env = string.Join('=', parts.Skip(1));

				Environment.SetEnvironmentVariable(parts[0], env);
			}
		}
	}
}