using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows;
using HSPacketAnalyzer.Services;
using Serilog;

namespace HSPacketAnalyzer.Controllers
{
	internal class WindowContextRepository : IContextRepository
	{
		private readonly Application _application;

		private readonly List<IContext> _contexts;

		private readonly BehaviorSubject<bool> _contextUpdateTrigger;

		public WindowContextRepository() : this(Application.Current) { }

		public WindowContextRepository(Application app)
		{
			_application = app;
			_contexts = new List<IContext>();
			_contextUpdateTrigger = new BehaviorSubject<bool>(false);
		}

		public IObservable<bool> WhenUpdated => _contextUpdateTrigger;

		public IReadOnlyList<IContext> Contexts => _contexts;

		private void StoreContext(IContext ctxt)
		{
			_contexts.Add(ctxt);
			Log.Debug("{amount} of contexts stored", _contexts.Count);

			_contextUpdateTrigger.OnNext(true);
		}

		public IContext NewContext()
		{
			var ctxt = WindowContext.Create();
			if (ctxt != null)
			{
				Log.Verbose("New {context} created", ctxt);
				StoreContext(ctxt);
			}

			return ctxt;
		}

		public void RemoveContext(IContext ctxt)
		{
			bool isRemoved = _contexts.Remove(ctxt);
			if (!isRemoved)
			{
				Log.Warning("The {context} wasn't found", ctxt);
				return;
			}

			if (_contexts.Count == 0)
			{
				Log.Debug("{amount} contexts left", _contexts.Count);
				Log.Information("Shutting down!");
				_application.Shutdown(0);
			}

			Log.Information("Removed {context} from the repository", ctxt);
		}
	}
}
