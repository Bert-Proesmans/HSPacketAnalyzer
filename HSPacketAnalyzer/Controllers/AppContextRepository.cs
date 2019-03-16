using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows;
using HSPacketAnalyzer.Services;
using Serilog;

namespace HSPacketAnalyzer.Controllers
{
	internal class AppContextRepository : IContextRepository
	{
		#region Private Fields

		private readonly Application _application;

		private readonly List<IContext> _contexts;

		private readonly BehaviorSubject<bool> _contextUpdateTrigger;

		#endregion

		#region Public Constructors
		public AppContextRepository() : this(Application.Current) { }

		public AppContextRepository(Application app)
		{
			_application = app;
			_contexts = new List<IContext>();
			_contextUpdateTrigger = new BehaviorSubject<bool>(false);
		}

		#endregion

		#region Public Properties

		public IObservable<bool> WhenUpdated => _contextUpdateTrigger;

		public IReadOnlyList<IContext> Contexts => _contexts;

		#endregion

		#region Public Methods

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

		#endregion

		#region Private Methods

		private void StoreContext(IContext ctxt)
		{
			_contexts.Add(ctxt);
			Log.Debug("{amount} of contexts stored", _contexts.Count);

			_contextUpdateTrigger.OnNext(true);
		}

		#endregion
	}
}
