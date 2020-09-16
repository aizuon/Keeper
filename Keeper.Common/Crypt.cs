using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace Keeper.Common
{
    public sealed class Crypt
    {
        private const int DefaultKeyLength = 32;

        private readonly object _AES_lock = new object();

        private readonly IBufferedCipher AES;

        private readonly ICipherParameters _AES_Key;

        private ICipherParameters _AES_IV;

        private readonly byte[] _nonce;

        private long _encryptCtr = 0;
        private long _decryptCtr = 0;

        public Crypt()
        {
            var rng = new RNGCryptoServiceProvider();

            AES = CipherUtilities.GetCipher("AES/CTR/NoPadding");

            byte[] AES_Key = new byte[DefaultKeyLength];
            rng.GetBytes(AES_Key);
            _AES_Key = ParameterUtilities.CreateKeyParameter("AES", AES_Key);

            _nonce = new byte[sizeof(ulong)];
            rng.GetBytes(_nonce);

            rng.Dispose();
        }

        public Crypt(byte[] key, ulong nonce)
        {
            AES = CipherUtilities.GetCipher("AES/CTR/NoPadding");

            _AES_Key = ParameterUtilities.CreateKeyParameter("AES", key);

            _nonce = BitConverter.GetBytes(nonce);
        }

        private byte[] GenerateIV(ulong ctr)
        {
            byte[] ctrBytes = BitConverter.GetBytes(ctr);

            byte[] iv = new byte[2 * sizeof(ulong)];
            Array.Copy(_nonce, 0, iv, 0, sizeof(ulong));
            Array.Copy(ctrBytes, 0, iv, sizeof(ulong), sizeof(ulong));
            return iv;
        }

        public byte[] GetKey()
        {
            return ((KeyParameter)_AES_Key).GetKey();
        }

        public ulong GetNonce()
        {
            return BitConverter.ToUInt64(_nonce, 0);
        }

        public byte[] Encrypt(byte[] data)
        {
            lock (_AES_lock)
            {
                if (AES == null)
                    throw new Exception("Encryption is not initialized");

                _AES_IV = new ParametersWithIV(_AES_Key, GenerateIV(unchecked((ulong)_encryptCtr)));
                AES.Init(true, _AES_IV);
                byte[] outputBytes = new byte[AES.GetOutputSize(data.Length)];
                int length = AES.ProcessBytes(data, outputBytes, 0);
                AES.DoFinal(outputBytes, length);
                Interlocked.Increment(ref _encryptCtr);
                return outputBytes;
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            lock (_AES_lock)
            {
                if (AES == null)
                    throw new Exception("Encryption is not initialized");

                _AES_IV = new ParametersWithIV(_AES_Key, GenerateIV(unchecked((ulong)_decryptCtr)));
                AES.Init(false, _AES_IV);
                byte[] comparisonBytes = new byte[AES.GetOutputSize(data.Length)];
                int length = AES.ProcessBytes(data, comparisonBytes, 0);
                AES.DoFinal(comparisonBytes, length);
                Interlocked.Increment(ref _decryptCtr);
                return comparisonBytes;
            }
        }
    }
}
