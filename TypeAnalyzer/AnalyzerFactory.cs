using System;

namespace TypeAnalyzer
{
	public class AnalyzerFactory: MarshalByRefObject
    {
		private static Analyzer _instance;

		public AnalyzerFactory() { }

		public Analyzer GetAnalyzer()
		{
			if(_instance == null)
			{
				_instance = new Analyzer();
			}

			return _instance;
		}
    }
}
