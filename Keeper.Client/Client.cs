using Keeper.Common;
using Keeper.Common.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public List<AccountInfo> Accounts;

        private Crypt Crypt;
        public AsyncManualResetEvent KeyExchangeEvent = new AsyncManualResetEvent();

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

        public async Task Connect(IPEndPoint endPoint)
        {
            Crypt = new Crypt();
            _client.Start();
            _client.Connect(endPoint, "Keeper/TEST");

            Logger.Information("Connecting to server", endPoint);
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            Logger.Information("Connected to the server");
            Send_C2SKeyExchange();
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Information("Disconnected from server. Reason: {0}", disconnectInfo.Reason);

            KeyExchangeEvent.Reset();
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
                case Opcode.S2CKeyExchangeSuccess:
                {
                    Handle_S2CKeyExchangeSuccess();
                    break;
                }

                case Opcode.LoginAck:
                {
                    if (!data.TryGetByte(out byte resultByte) || !data.TryGetInt(out int accountCount))
                        return;

                    var result = (LoginResult)resultByte;

                    var accounts = new List<AccountInfo>(accountCount);
                    for (int i = 0; i < accountCount; i++)
                    {
                        if (!data.TryGetString(out string accountName) || !data.TryGetString(out string accountId) || !data.TryGetBytesWithLength(out byte[] encodedPassword))
                            return;

                        string password = Encoding.UTF8.GetString(RSA_Password.Decrypt(encodedPassword, true));

                        accounts.Add(new AccountInfo(accountName, accountId, password));
                    }

                    Handle_LoginAck(result, accounts);
                    break;
                }

                case Opcode.RegisterAck:
                {
                    if (!data.TryGetByte(out byte resultByte))
                        return;

                    var result = (RegisterResult)resultByte;

                    Handle_RegisterAck(result);
                    break;
                }

                case Opcode.AccountAddAck:
                {
                    if (!data.TryGetUInt(out uint accountId))
                        return;

                    Handle_AccountAddAck(accountId);
                    break;
                }

                case Opcode.AccountEditAck:
                {
                    if (!data.TryGetByte(out byte resultByte))
                        return;

                    var result = (AccountEditResult)resultByte;

                    Handle_AccountEditAck(result);
                    break;
                }
            }
        }

        private void Handle_S2CKeyExchangeSuccess()
        {
            KeyExchangeEvent.Set();
        }

        private void Handle_LoginAck(LoginResult result, List<AccountInfo> accounts)
        {
            switch (result)
            {
                case LoginResult.Success:
                {
                    Accounts = accounts;
                    var formThread = new Thread(() => LoginForm.Instance?.Invoke(new Action(() => LoginForm.Instance.MoveToMainForm(Accounts))))
                    {
                        IsBackground = true
                    };
                    formThread.Start();
                    break;
                }

                case LoginResult.UsernameDoesntExist:
                {
                    LoginForm.Instance?.Invoke(new Action(() => LoginForm.Instance.SetErrorLabel("Account doesn't exist.")));
                    break;
                }

                case LoginResult.WrongPassword:
                {
                    LoginForm.Instance?.Invoke(new Action(() => LoginForm.Instance.SetErrorLabel("Wrong password.")));
                    break;
                }
            }
        }

        private void Handle_RegisterAck(RegisterResult result)
        {
            switch (result)
            {
                case RegisterResult.Success:
                {
                    RegisterForm.Instance?.Invoke(new Action(() => RegisterForm.Instance.SetSuccessLabel("Account created.")));
                    break;
                }

                case RegisterResult.UsernameTaken:
                {
                    RegisterForm.Instance?.Invoke(new Action(() => RegisterForm.Instance.SetErrorLabel("Username is already taken.")));
                    break;
                }
            }
        }

        private void Handle_AccountAddAck(uint accountId)
        {
            //TODO
        }

        private void Handle_AccountEditAck(AccountEditResult result)
        {
            //TODO
        }

        public void Send_C2SKeyExchange()
        {
            var message = new NetDataWriter();

            byte[] encryptedKey = RSA.Encrypt(Crypt.GetKey(), true);
            message.PutBytesWithLength(encryptedKey);
            message.Put(Crypt.GetNonce());

            Send(Opcode.C2SKeyExchange, message, false);
        }

        public void Send_LoginReq(string username, string password)
        {
            var message = new NetDataWriter();

            message.Put(username);
            using (var sha384 = new SHA384CryptoServiceProvider())
            {
                byte[] hashedPassword = sha384.ComputeHash(Encoding.UTF8.GetBytes(password));
                message.Put(Convert.ToBase64String(hashedPassword));
            }

            Send(Opcode.LoginReq, message);
        }

        public void Send_RegisterReq(string username, string password)
        {
            var message = new NetDataWriter();

            message.Put(username);
            using (var sha384 = new SHA384CryptoServiceProvider())
            {
                byte[] hashedPassword = sha384.ComputeHash(Encoding.UTF8.GetBytes(password));
                message.Put(Convert.ToBase64String(hashedPassword));
            }

            Send(Opcode.RegisterReq, message);
        }

        public void Send_AccountAddReq(string accountName, string accountId, string password)
        {
            var message = new NetDataWriter();

            message.Put(accountName);
            message.Put(accountId);
            byte[] encodedPassword = RSA_Password.Encrypt(Encoding.UTF8.GetBytes(password), true);
            message.PutBytesWithLength(encodedPassword);

            Send(Opcode.AccountAddReq, message);
        }

        public void Send_AccountEditReq(uint id, string accountName, string accountId, string password)
        {
            var message = new NetDataWriter();

            message.Put(id);
            message.Put(accountName);
            message.Put(accountId);
            byte[] encodedPassword = RSA_Password.Encrypt(Encoding.UTF8.GetBytes(password), true);
            message.PutBytesWithLength(encodedPassword);

            Send(Opcode.AccountEditReq, message);
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

        public void Disconnect()
        {
            if (IsConnected)
                _client.Stop();
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
