using System;
using System.Collections.Generic;

namespace HSPacketAnalyzer.Services
{
	internal interface IContextRepository
	{
		IObservable<bool> WhenUpdated { get; }

		IReadOnlyList<IContext> Contexts { get; }

		IContext NewContext();

		void RemoveContext(IContext ctxt);
	}
}
