using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TypeAnalyzer
{
	public class Analyzer : MarshalByRefObject
	{
		private static readonly string[] RequiredAssemblyFiles;
		private static readonly string[] EncryptedAssemblyFiles;

		private string _libPath;

		static Analyzer()
		{
			RequiredAssemblyFiles = new string[]
			{
				"Assembly-CSharp.dll",
				"Assembly-CSharp-firstpass.dll",
			};

			EncryptedAssemblyFiles = new string[]
			{
				"Assembly-CSharp.dll",
			};
		}

		public Analyzer()
		{

		}

		public Task<bool> Initialize(string libPath)
		{
			_libPath = libPath;

			AppDomain currentDomain = AppDomain.CurrentDomain;

			currentDomain.FirstChanceException += Domain_FirstChanceException;
			currentDomain.UnhandledException += Domain_UnhandledException;
			currentDomain.AssemblyResolve += Domain_RetrieveAlreadyLoadedLibs;
			currentDomain.AssemblyResolve += Domain_RetrieveInLibPath;
			currentDomain.AssemblyResolve += Domain_DecryptLibraries;
			currentDomain.AssemblyLoad += Domain_AssemblyLoad;

			foreach (string fileName in RequiredAssemblyFiles)
			{
				_ = Assembly.LoadFile(fileName);
			}

			return Task.FromResult(false);
		}

		#region Private Methods

		private void Domain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
		{
			Debugger.Break();
		}

		private void Domain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Debugger.Break();
		}

		private Assembly Domain_RetrieveAlreadyLoadedLibs(object sender, ResolveEventArgs args)
		{
			Debugger.Break();
			// Ignore missing resources
			if (args.Name.Contains(".resources"))
			{
				return null;
			}

			// check for assemblies already loaded
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
			if (assembly != null)
			{
				return assembly;
			}

			// Try to load by filename - split out the filename of the full assembly name
			// and append the base path of the original assembly (ie. look in the same dir)
			string filename = args.Name.Split(',')[0] + ".dll".ToLower();
			string asmFile = Path.Combine(@".\", "Addins", filename);

			try
			{
				return Assembly.LoadFrom(asmFile);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private Assembly Domain_RetrieveInLibPath(object sender, ResolveEventArgs args)
		{
			Debugger.Break();
			// Ignore missing resources
			if (args.Name.Contains(".resources"))
			{
				return null;
			}

			string subjectFileName = args.Name;
			foreach (string targetFilePath in Directory.GetFiles(_libPath, "*.dll"))
			{
				string targetFileName = Path.GetFileNameWithoutExtension(targetFilePath).ToLower();
				if (subjectFileName == targetFileName)
				{
					try
					{
						return Assembly.LoadFrom(targetFilePath);
					}
					catch (Exception)
					{
						return null;
					}
				}
			}

			return null;
		}

		private Assembly Domain_DecryptLibraries(object sender, ResolveEventArgs args)
		{
			Debugger.Break();
			// Ignore missing resources
			if (args.Name.Contains(".resources"))
			{
				return null;
			}

			return null;
		}

		private void Domain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			Debugger.Break();
		}

		#endregion
	}
}
