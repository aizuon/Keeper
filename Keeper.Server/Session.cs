using Keeper.Common;
using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace Keeper.Server
{
    public class Session : IDisposable
    {
        public readonly NetPeer Peer;
        private readonly Server owner;

        public Crypt Crypt;

        public int PeerId => Peer.Id;
        public bool IsConnected => Peer.ConnectionState.HasFlag(ConnectionState.Connected);
        public int Ping => Peer.Ping;

        public readonly object _sendLock = new object();
        public readonly object _recvLock = new object();

        private volatile bool _disposed = false;

        public Session(NetPeer peer, Server owner)
        {
            Peer = peer;
            this.owner = owner;
        }

        public void Send(Opcode opcode, NetDataWriter message, bool encrypt = true, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            if (_disposed || !IsConnected)
                return;

            lock (_sendLock)
            {
                try
                {
                    owner.Logger.Verbose("Sending {0} to {1}", opcode, PeerId);

                    byte[] data;
                    if (encrypt)
                    {
                        if (Crypt == null)
                            return;
                        data = Crypt.Encrypt(message.Data);
                    }
                    else
                    {
                        data = message.Data;
                    }

                    var fullMessage = new NetDataWriter();
                    fullMessage.Put((byte)opcode);
                    fullMessage.Put(encrypt);
                    fullMessage.Put(data);

                    Peer.Send(fullMessage, method);
                }
                catch (Exception ex)
                {
                    owner.Logger.Error(ex.ToString());
                    return;
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (IsConnected)
                Peer.Disconnect();
        }
    }
}
