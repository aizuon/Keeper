using Dapper.FastCrud;
using Keeper.Common;
using Keeper.Server.Database;
using Keeper.Server.Database.Models;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace Keeper.Server
{
    using BCrypt.Net;
    using Org.BouncyCastle.Asn1;
    using System.Threading.Tasks;

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
            if (!_sessions.TryGetValue(peer.Id, out var session))
                return;

            lock (session._recvLock)
            {
                try
                {
                    if (!reader.TryGetByte(out byte opcodeByte) || !reader.TryGetBool(out bool encrypt))
                        return;

                    var opcode = (Opcode)opcodeByte;

                    byte[] data = reader.GetRemainingBytes();

                    if (encrypt)
                    {
                        if (session.Crypt == null)
                            return;
                        data = session.Crypt.Decrypt(data);
                    }

                    Handle(session, opcode, new NetDataReader(data));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    return;
                }
            }
        }

        private void Handle(Session session, Opcode opcode, NetDataReader data)
        {
            Logger.Debug("Recieved {0} from {1}", opcode, session.PeerId);

            switch (opcode)
            {
                case Opcode.C2SKeyExchange:
                {
                    if (!data.TryGetBytesWithLength(out byte[] key) || !data.TryGetULong(out ulong nonce))
                        return;

                    Handle_C2SKeyExchange(session, key, nonce);

                    break;
                }

                case Opcode.LoginReq:
                {
                    if (!data.TryGetString(out string username) || !data.TryGetString(out string hashedPassword) || !data.TryGetBytesWithLength(out byte[] encodedPublicKey))
                        return;

                    Handle_LoginReq(session, username, hashedPassword, encodedPublicKey);
                    break;
                }

                case Opcode.RegisterReq:
                {
                    if (!data.TryGetString(out string username) || !data.TryGetString(out string hashedPassword))
                        return;

                    Handle_RegisterReq(session, username, hashedPassword);
                    break;
                }

                case Opcode.AccountAddReq:
                {
                    if (!data.TryGetString(out string accountName) || !data.TryGetString(out string accountId) || !data.TryGetBytesWithLength(out byte[] encodedPassword))
                        return;

                    Handle_AddAccountReq(session, accountName, accountId, encodedPassword);
                    break;
                }

                case Opcode.AccountEditReq:
                {
                    if (!data.TryGetUInt(out uint id) ||
                        !data.TryGetString(out string accountName) || !data.TryGetString(out string accountId) || !data.TryGetBytesWithLength(out byte[] encodedPassword))
                        return;

                    Handle_AccountEditReq(session, id, accountName, accountId, encodedPassword);
                    break;
                }
            }
        }

        private void Handle_C2SKeyExchange(Session session, byte[] key, ulong nonce)
        {
            try
            {
                byte[] decryptedKey = RSA.Decrypt(key, true);
                session.Crypt = new Crypt(decryptedKey, nonce);
            }
            catch (Exception)
            {
                session.Dispose();
                return;
            }

            session.Send_S2CKeyExchangeSuccess();
        }

        private void Handle_LoginReq(Session session, string username, string hashedPassword, byte[] encodedPublicKey)
        {
            using (var db = DB.Open())
            {
                var user = db.Find<UserDto>(statement => statement
                        .Where($"{nameof(UserDto.username):C} = @username")
                        .Include<AccountDto>(join => join.LeftOuterJoin())
                        .WithParameters(new { username })).FirstOrDefault();

                if (user == null)
                {
                    session.Send_LoginAck(LoginResult.UsernameDoesntExist);
                    return;
                }

                if (!BCrypt.Verify(hashedPassword, user.password))
                {
                    Logger.Information("Wrong password for {0} from {1}", username, session.Peer.EndPoint);
                    session.Send_LoginAck(LoginResult.WrongPassword);
                    return;
                }

                var sequence = (DerSequence)Asn1Object.FromByteArray(encodedPublicKey);
                byte[] modulus = ((DerInteger)sequence[0]).Value.ToByteArrayUnsigned();
                byte[] exponent = ((DerInteger)sequence[1]).Value.ToByteArrayUnsigned();
                var publicKey = new RSAParameters { Exponent = exponent, Modulus = modulus };
                session.RSA = new RSACryptoServiceProvider();
                session.RSA.ImportParameters(publicKey);

                session.UserId = user.id;

                var accounts = user.Accounts.Select(a => new AccountInfo(a.account, a.account_id, a.account_password));
                session.Send_LoginAck(LoginResult.Success, accounts);
            }
        }

        private void Handle_RegisterReq(Session session, string username, string hashedPassword)
        {
            using (var db = DB.Open())
            {
                var user = db.Find<UserDto>(statement => statement
                        .Where($"{nameof(UserDto.username):C} = @username")
                        .Include<AccountDto>(join => join.LeftOuterJoin())
                        .WithParameters(new { username })).FirstOrDefault();

                if (user != null)
                {
                    session.Send_RegisterAck(RegisterResult.UsernameTaken);
                    return;
                }

                user = new UserDto { username = username };

                user.password = BCrypt.HashPassword(hashedPassword, 10);

                session.Send_RegisterAck(RegisterResult.Success);
            }
        }

        private void Handle_AddAccountReq(Session session, string accountName, string accountId, byte[] encodedPassword)
        {
            using (var db = DB.Open())
            {
                var account = new AccountDto
                {
                    user_id = session.UserId,
                    account = accountName,
                    account_id = accountId
                };

                string password = Convert.ToBase64String(encodedPassword);
                account.account_password = password;

                db.Insert(account);

                session.Send_AccountAddAck(account.id);
            }
        }

        private async Task Handle_AccountEditReq(Session session, uint id, string accountName, string accountId, byte[] encodedPassword)
        {
            using (var db = DB.Open())
            {
                var account = db.Find<AccountDto>(statement => statement
                        .Where($"{nameof(UserDto.id):C} = @id")
                        .WithParameters(new { id })).FirstOrDefault();

                if (account != null)
                {
                    session.Send_AccountEditAck(AccountEditResult.AccountNotFound);
                    return;
                }

                account.account = accountName;
                account.account_id = accountId;
                account.account_password = Convert.ToBase64String(encodedPassword);

                db.UpdateAsync(account);

                session.Send_AccountEditAck(AccountEditResult.Success);
            }
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
