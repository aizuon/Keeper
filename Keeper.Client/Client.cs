using Keeper.Common;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace Keeper.Client
{
    public class Client : IDisposable
    {
        public static Client Instance { get; private set; }

        public ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(Client));

        private readonly EventBasedNetListener _listener = new EventBasedNetListener();
        private readonly NetManager _client;

        private bool IsConnected => _client.FirstPeer?.ConnectionState.HasFlag(ConnectionState.Connected) ?? false;

        private volatile bool _disposed;

        private RSACryptoServiceProvider RSA;
        private static RSACryptoServiceProvider RSA_Password;

        private Crypt Crypt;

        private readonly object _sendLock = new object();
        private readonly object _recvLock = new object();

        private Client()
        {
            RSA = RSAHelper.CreateRsaProviderFromPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAw191Ozc0O1RvCj5nrm83Yz51IgR9ESRnDIU6D97Kb4ybKR29W6lx07ASkOACyUIa3IZPWX3De8Jbc2hf6Q1JxSsoDKgPihvAjbVDh+h5D2Z5x8+2x3ytcORSrPp40mfZZH13pi8+W7+GuwqkfFuraoGmfjz23ytcgolqBs5XnwBkMks+L0QZzmjGLwWMAepa7WHYrjVN7KKfitVq4cFlKOJheYe4fSuWvoxOxn30j+qCV3oE/2AB3m7ZZeSIRtPdHmGFBiSHnAMfWiKzhAX/+mTNKsXcQVwoEMqnu4KT/ZzVqinJPt/RbFP0wkRHs9sYGBKm4BVLllln8wIZEE/mlwIDAQAB");

            _client = new NetManager(_listener)
            {
                IPv6Enabled = IPv6Mode.Disabled,
                AutoRecycle = true,
                UnsyncedEvents = true,
                UnsyncedDeliveryEvent = false
            };
            _client.Start();

            _listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            _listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            _listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
        }

        public static void Initialize()
        {
            if (Instance != null)
                throw new InvalidOperationException("Client is already initialized");

            if (!File.Exists("pub.pem") || !File.Exists("priv.pem"))
            {
                RSA_Password = new RSACryptoServiceProvider(2048);
                File.WriteAllText("pub.pem", RSAHelper.ExportPublicKey(RSA_Password));
                File.WriteAllText("priv.pem", RSAHelper.ExportPrivateKey(RSA_Password));
            }
            else
            {
                RSA_Password = new RSACryptoServiceProvider();
                var pubKey = RSAHelper.GetPublicKeyParams(File.ReadAllText("pub.pem"));
                RSA_Password.ImportParameters(pubKey);
                var privKey = RSAHelper.GetPrivateKeyParams(File.ReadAllText("priv.pem"));
                RSA_Password.ImportParameters(privKey);
            }

            Instance = new Client();
        }

        public void Connect(IPEndPoint endPoint)
        {
            Crypt = new Crypt();
            _client.Connect(endPoint, "Keeper/TEST");

            Logger.Information("Connecting to server", endPoint);
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            Logger.Information("Connected to the server");
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Information("Disconnected from server. Reason: {0}", disconnectInfo.Reason);
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            lock (_recvLock)
            {
                try
                {
                    if (!reader.TryGetByte(out byte opcodeByte) || !reader.TryGetBool(out bool encrypt))
                        return;

                    var opcode = (Opcode)opcodeByte;

                    byte[] data = reader.GetRemainingBytes();

                    if (encrypt)
                    {
                        if (Crypt == null)
                            return;
                        data = Crypt.Decrypt(data);
                    }

                    Handle(opcode, new NetDataReader(data));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    return;
                }
            }
        }

        private void Handle(Opcode opcode, NetDataReader data)
        {
            Logger.Debug("Recieved {0}", opcode);

            switch (opcode)
            {
                //TODO
            }
        }

        private void Send(Opcode opcode, NetDataWriter message, bool encrypt = true, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            if (_disposed || !IsConnected)
                return;

            lock (_sendLock)
            {
                try
                {
                    Logger.Debug("Sending {0}", opcode);

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

                    _client.FirstPeer.Send(fullMessage, method);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
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
                _client.Stop();

            RSA?.Dispose();
            RSA = null;
            RSA_Password?.Dispose();
            RSA_Password = null;

            Instance = null;
        }
    }
}
