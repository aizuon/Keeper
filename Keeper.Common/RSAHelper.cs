using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Keeper.Common
{
    public static class RSAHelper
    {
        public static RSAParameters GetPrivateKeyParams(string pem)
        {
            var pr = new PemReader(new StringReader(pem));
            var KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            var rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)KeyPair.Private);

            return rsaParams;
        }

        public static RSAParameters GetPublicKeyParams(string pem)
        {
            var pr = new PemReader(new StringReader(pem));
            var publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            var rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

            return rsaParams;
        }

        public static RSACryptoServiceProvider ImportPrivateKey(string pem)
        {
            var rsaParams = GetPrivateKeyParams(pem);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(rsaParams);
            return csp;
        }

        public static RSACryptoServiceProvider ImportPublicKey(string pem)
        {
            var rsaParams = GetPublicKeyParams(pem);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(rsaParams);
            return csp;
        }

        public static string ExportPrivateKey(RSACryptoServiceProvider csp)
        {
            using (var outputStream = new StringWriter())
            {
                if (csp.PublicOnly)
                    throw new ArgumentException("CSP does not contain a private key", nameof(csp));
                var parameters = csp.ExportParameters(true);
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write((byte)0x30);
                        using (var innerStream = new MemoryStream())
                        {
                            using (var innerWriter = new BinaryWriter(innerStream))
                            {
                                EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 });
                                EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                                EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                                EncodeIntegerBigEndian(innerWriter, parameters.D);
                                EncodeIntegerBigEndian(innerWriter, parameters.P);
                                EncodeIntegerBigEndian(innerWriter, parameters.Q);
                                EncodeIntegerBigEndian(innerWriter, parameters.DP);
                                EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                                EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                                int length = (int)innerStream.Length;
                                EncodeLength(writer, length);
                                writer.Write(innerStream.GetBuffer(), 0, length);
                            }
                        }

                        char[] base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                        outputStream.Write("-----BEGIN RSA PRIVATE KEY-----\n");
                        for (int i = 0; i < base64.Length; i += 64)
                        {
                            outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                            outputStream.Write("\n");
                        }
                        outputStream.Write("-----END RSA PRIVATE KEY-----");
                    }
                }

                return outputStream.ToString();
            }
        }

        public static string ExportPublicKey(RSACryptoServiceProvider csp)
        {
            using (var outputStream = new StringWriter())
            {
                var parameters = csp.ExportParameters(false);
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write((byte)0x30);
                        using (var innerStream = new MemoryStream())
                        {
                            using (var innerWriter = new BinaryWriter(innerStream))
                            {
                                innerWriter.Write((byte)0x30);
                                EncodeLength(innerWriter, 13);
                                innerWriter.Write((byte)0x06);
                                byte[] rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                                EncodeLength(innerWriter, rsaEncryptionOid.Length);
                                innerWriter.Write(rsaEncryptionOid);
                                innerWriter.Write((byte)0x05);
                                EncodeLength(innerWriter, 0);
                                innerWriter.Write((byte)0x03);
                                using (var bitStringStream = new MemoryStream())
                                {
                                    using (var bitStringWriter = new BinaryWriter(bitStringStream))
                                    {
                                        bitStringWriter.Write((byte)0x00);
                                        bitStringWriter.Write((byte)0x30);
                                        using (var paramsStream = new MemoryStream())
                                        {
                                            using (var paramsWriter = new BinaryWriter(paramsStream))
                                            {
                                                EncodeIntegerBigEndian(paramsWriter, parameters.Modulus);
                                                EncodeIntegerBigEndian(paramsWriter, parameters.Exponent);
                                                int paramsLength = (int)paramsStream.Length;
                                                EncodeLength(bitStringWriter, paramsLength);
                                                bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                                            }
                                        }
                                        int bitStringLength = (int)bitStringStream.Length;
                                        EncodeLength(innerWriter, bitStringLength);
                                        innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                                    }
                                }
                                int length = (int)innerStream.Length;
                                EncodeLength(writer, length);
                                writer.Write(innerStream.GetBuffer(), 0, length);
                            }
                        }

                        char[] base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                        outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
                        for (int i = 0; i < base64.Length; i += 64)
                        {
                            outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                            outputStream.Write("\n");
                        }
                        outputStream.Write("-----END PUBLIC KEY-----");
                    }
                }

                return outputStream.ToString();
            }
        }

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative");
            if (length < 0x80)
            {
                stream.Write((byte)length);
            }
            else
            {
                int temp = length;
                int bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (int i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02);
            int prefixZeros = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (int i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        public static RSACryptoServiceProvider CreateRsaProviderFromPrivateKey(string privateKey)
        {
            byte[] privateKeyBits = Convert.FromBase64String(privateKey);

            var RSA = new RSACryptoServiceProvider();
            var RSAparams = new RSAParameters();

            using (var binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            RSA.ImportParameters(RSAparams);
            return RSA;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            int count;
            if (bt == 0x81)
                count = binr.ReadByte();
            else if (bt == 0x82)
            {
                byte highbyte = binr.ReadByte();
                byte lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        public static RSACryptoServiceProvider CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

            byte[] x509key = Convert.FromBase64String(publicKeyString);

            using (var mem = new MemoryStream(x509key))
            using (var binr = new BinaryReader(mem))
            {
                byte bt = 0;
                ushort twobytes = 0;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                byte[] seq = binr.ReadBytes(15);
                if (!CompareByteArrays(seq, SeqOID))
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103)
                    binr.ReadByte();
                else if (twobytes == 0x8203)
                    binr.ReadInt16();
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102)
                    lowbyte = binr.ReadByte();
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte();
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                int modsize = BitConverter.ToInt32(modint, 0);

                int firstbyte = binr.PeekChar();
                if (firstbyte == 0x00)
                {
                    binr.ReadByte();
                    modsize -= 1;
                }

                byte[] modulus = binr.ReadBytes(modsize);

                if (binr.ReadByte() != 0x02)
                    return null;
                int expbytes = binr.ReadByte();
                byte[] exponent = binr.ReadBytes(expbytes);

                var RSA = new RSACryptoServiceProvider();
                var RSAKeyInfo = new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = exponent
                };
                RSA.ImportParameters(RSAKeyInfo);

                return RSA;
            }
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
    }
}
