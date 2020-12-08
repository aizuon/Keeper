using Keeper.Common;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keeper.Server
{
    public class Session : IDisposable
    {
        public readonly NetPeer Peer;
        private readonly Server owner;

        public uint UserId;

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

        public void Send_S2CKeyExchangeSuccess()
        {
            var message = new NetDataWriter();

            Send(Opcode.S2CKeyExchangeSuccess, message, false);
        }

        public void Send_LoginAck(LoginResult result)
        {
            var message = new NetDataWriter();

            message.Put((byte)result);
            message.Put(0);

            Send(Opcode.LoginAck, message);
        }

        public void Send_LoginAck(LoginResult result, IEnumerable<AccountInfo> accounts)
        {
            var message = new NetDataWriter();

            message.Put((byte)result);
            message.Put(accounts.Count());
            foreach (var account in accounts)
            {
                message.Put(account.Name);
                message.Put(account.Id);
                message.PutBytesWithLength(Convert.FromBase64String(account.Password));
            }

            Send(Opcode.LoginAck, message);
        }

        public void Send_RegisterAck(RegisterResult result)
        {
            var message = new NetDataWriter();

            message.Put((byte)result);

            Send(Opcode.RegisterAck, message);
        }

        public void Send_AccountAddAck(uint accountId)
        {
            var message = new NetDataWriter();

            message.Put(accountId);

            Send(Opcode.AccountAddAck, message);
        }

        public void Send_AccountEditAck(AccountEditResult result)
        {
            var message = new NetDataWriter();

            message.Put((byte)result);

            Send(Opcode.AccountEditAck, message);
        }

        private void Send(Opcode opcode, NetDataWriter message, bool encrypt = true, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            if (_disposed || !IsConnected)
                return;

            lock (_sendLock)
            {
                try
                {
                    owner.Logger.Debug("Sending {0} to {1}", opcode, PeerId);

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
            {
                //TODO: Peer.Flush() gone? check
                Peer.Disconnect();
            }
        }
    }
}
