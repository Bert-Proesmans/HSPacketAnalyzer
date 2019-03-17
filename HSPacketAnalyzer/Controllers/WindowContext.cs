using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading.Tasks;
using Foundation.Collections;
using HSPacketAnalyzer.Services;
using PacketModels;
using PacketModels.Packets;
using PacketModels.Packets.BNET;
using TypeAnalyzer;

namespace HSPacketAnalyzer.Controllers
{
	internal class WindowContext : IContext
	{
		#region Private Fields

		private AppDomain _analyzeDomain;
		private string _analyzeDomainSandbox;
		private string _packetPath;
		private readonly ExtendedObservableCollection<Payload> _payloads;
		private readonly ExtendedObservableCollection<PacketBase> _packets;

		#endregion

		#region Public Constructors

		private WindowContext()
		{
			_payloads = new ExtendedObservableCollection<Payload>();
			_packets = new ExtendedObservableCollection<PacketBase>();
		}

		#endregion

		#region Public Properties

		public string Name { get; set; }

		public string SupportedVersion { get; private set; }

		public string CurrentVersion { get; private set; }

		public int PayloadCount { get; private set; }

		public ObservableCollection<PacketBase> Packets => _packets;

		public string LibPath { get; set; }

		#endregion

		#region Public Methods

		public static WindowContext Create()
		{
			var ctxt = new WindowContext();

			string domainSandbox = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(domainSandbox);

			ctxt._analyzeDomainSandbox = domainSandbox;

			return ctxt;
		}

		public Task Initialise(string filePath)
		{
			_packetPath = filePath;

			SetSamplePackets();

			return null;
		}

		public Task<bool> RebuildAnalyzerAsync()
		{
			if (disposedValue)
			{
				return Task.FromException<bool>(new ObjectDisposedException(nameof(WindowContext)));
			}

			if (!Directory.Exists(LibPath))
			{
				return Task.FromResult(false);
			}

			// TODO; Clean up previous AppDomain
			if (_analyzeDomain != null)
			{
				// TODO; Find instance and stop any processing.
			}

			Type factoryType = typeof(AnalyzerFactory);

			// TODO; Find out how to lock down permissions in the new AppDomain!

			var domainEvidence = new Evidence();
			// NOTE; Probably should be UnTrusted!
			domainEvidence.AddHostEvidence(new Zone(SecurityZone.Trusted));

			PermissionSet permissions = SecurityManager.GetStandardSandbox(domainEvidence);
			permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

			var fullTrustAssemblies = new StrongName[] {
				factoryType.Assembly.Evidence.GetHostEvidence<StrongName>()
			};

			// TODO; Copy all required libraries to the sandbox folder.
			// Both Trusted and Untrusted libraries!
			string factoryLibPath = factoryType.Assembly.Location;
			File.Copy(factoryLibPath, Path.Combine(_analyzeDomainSandbox, Path.GetFileName(factoryLibPath)));

			var domainSetup = new AppDomainSetup()
			{
				ApplicationBase = _analyzeDomainSandbox,
			};

			string domainFriendlyName = $"AnalyzerDomain for ${Name}";
			_analyzeDomain = AppDomain.CreateDomain(domainFriendlyName, domainEvidence, domainSetup, permissions, fullTrustAssemblies);

			var proxyFactory = (AnalyzerFactory)_analyzeDomain.CreateInstanceAndUnwrap(factoryType.Assembly.FullName, factoryType.FullName);
			Analyzer analyzer = proxyFactory.GetAnalyzer();

			return analyzer.Initialize(LibPath);
		}

		public virtual void AddPacket() { }
		// public virtual void RemovePacket() { }
		// public virtual void GetPacket() { }
		public virtual void SetPackets() { }
		public virtual void AnalyzePackets() { }

		#endregion

		#region Protected Methods

		protected void SetSamplePackets()
		{
			var samplePackets = new PacketBase[] {
				new BNetPacket(null, false, new RPCHeader(-1, -1, -1), null),
				// new PegPacket(null, false, -1, null),
			};

			_packets.Clear();
			_packets.AddRange(samplePackets);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					if (_analyzeDomain != null)
					{
						AppDomain.Unload(_analyzeDomain);
					}

					if (_analyzeDomainSandbox != null)
					{
						Directory.Delete(_analyzeDomainSandbox, true);
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~WindowContext()
		// {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

		#endregion
	}
}
