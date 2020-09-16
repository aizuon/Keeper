using Keeper.Common;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace Keeper.Server
{
    public class Server : IDisposable
    {
        public static Server Instance { get; private set; }

        public ILogger Logger = Log.ForContext(Serilog.Core.Constants.SourceContextPropertyName, nameof(Server));

        private readonly EventBasedNetListener _listener = new EventBasedNetListener();
        private readonly NetManager _netManager;

        public RSACryptoServiceProvider RSA;

        private readonly ConcurrentDictionary<int, Session> _sessions = new ConcurrentDictionary<int, Session>();
        public IReadOnlyDictionary<int, Session> Sessions => _sessions;

        private Server()
        {
            RSA = RSAHelper.CreateRsaProviderFromPrivateKey(Config.Instance.PrivateKey);

            _netManager = new NetManager(_listener)
            {
                IPv6Enabled = IPv6Mode.Disabled,
                AutoRecycle = true,
                UnsyncedEvents = true,
                UnsyncedDeliveryEvent = false
            };

            _listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            _listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            _listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            _listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            Logger.Information("Client<{0}> successfully connected to the server", peer.Id);
            _sessions.TryAdd(peer.Id, new Session(peer, this));
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Information("Client<{0}> disconnected from the server. Reason: {1}", peer.Id, disconnectInfo.Reason);
            _sessions.TryRemove(peer.Id, out var session);
            session.Dispose();
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            Logger.Information("Incoming connection from {0}", request.RemoteEndPoint);
            if (_netManager.ConnectedPeersCount < Config.Instance.MaxConnections)
                request.AcceptIfKey(Config.Instance.ConnectionKey);
            else
                request.Reject();
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if (!_sessions.TryGetValue(peer.Id, out var sender))
                return;

            lock (sender._recvLock)
            {
                try
                {
                    var opcode = (Opcode)reader.GetByte();
                    bool encrypt = reader.GetBool();
                    byte[] data = reader.GetRemainingBytes();

                    if (encrypt)
                    {
                        if (sender.Crypt == null)
                            return;
                        data = sender.Crypt.Decrypt(data);
                    }

                    Handle(sender, opcode, new NetDataReader(data));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    return;
                }
            }
        }

        private void Handle(Session sender, Opcode opcode, NetDataReader data)
        {
            //TODO
        }

        public static void Initialize()
        {
            if (Instance != null)
                throw new InvalidOperationException("Server is already initialized");

            Instance = new Server();
        }

        public void Listen(IPEndPoint endPoint)
        {
            _netManager.Start(endPoint.Address, IPAddress.IPv6Loopback, endPoint.Port);

            Logger.Information("Server is working on {0}", endPoint);
        }

        private void Stop()
        {
            _netManager.Stop();
        }

        public void Dispose()
        {
            Stop();

            RSA?.Dispose();
            RSA = null;
        }
    }
}
