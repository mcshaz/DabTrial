using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Linq;
using DabTrial.Infrastructure.Interfaces;
namespace DabTrial.Infrastructure.Crypto
{
    //http://stackoverflow.com/questions/165808/simple-2-way-encryption-for-c-sharp#5518092
    public class SimpleAES : ICryptoProvider
    {
        private ICryptoTransform encryptor, decryptor;
        private UTF8Encoding encoder;
        private readonly int saltLen;
        private readonly byte[][] saltPossibilities;

        private Random _rndm;
        private Random Rndm
        {
            get
            {
                return _rndm ?? (_rndm = new Random());
            }
        }

        private RNGCryptoServiceProvider _rNGcrypto;
        private RNGCryptoServiceProvider RNGcrypto 
        {
            get
            {
                return _rNGcrypto ?? (_rNGcrypto = new RNGCryptoServiceProvider());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vector"></param>
        /// <param name="saltList">a 2 dimensional array of bytes to randomly choose to salt from. The i variable is the salt, the j randomly selected</param>
        public SimpleAES(byte[] key, byte[] vector, byte[][] saltList) : this(key, vector,saltList[0].Length)
        {
#if DEBUG
            if (saltList.Any(s => s.Length != saltLen)) { throw new ArgumentException("jagged Array is not implemented for performance reasons, but should be rectangular"); }
#endif
            saltPossibilities = saltList;
        }

        public SimpleAES(byte[] key, byte[] vector, int saltLength = 0)
        {
            RijndaelManaged rm = new RijndaelManaged();
            encryptor = rm.CreateEncryptor(key, vector);
            decryptor = rm.CreateDecryptor(key, vector);
            encoder = new UTF8Encoding();
            saltLen = saltLength;
        }

        public string Encrypt(string unencrypted)
        {
            if (string.IsNullOrEmpty(unencrypted)) { return unencrypted; }
            return Convert.ToBase64String(Encrypt(AddSalt(unencrypted)));
        }

        public string[] PossibleEncryptionValues(string unencrypted)
        {
            if (saltPossibilities == null) { throw new ArgumentException("this method is only available if instantiated with the saltList argument"); }
            var len = saltPossibilities.Length;
            var returnVar = new string[len * len];
            Byte[] pepper = encoder.GetBytes(unencrypted);
            int nextIndx = 0;
            for (int i = 0; i < len; ++i)
            {
                for (int j = 0; j < len; ++j)
                {
                    returnVar[nextIndx++] = Convert.ToBase64String(Encrypt(AddSalt(pepper,saltPossibilities[i], saltPossibilities[j])));
                }
            }
            return returnVar;
        }

        public string Decrypt(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted)) { return encrypted; }
            return RemoveSalt(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public string EncryptToUrl(string unencrypted)
        {
            return HttpUtility.UrlEncode(Encrypt(unencrypted));
        }

        public string DecryptFromUrl(string encrypted)
        {
            return Decrypt(HttpUtility.UrlDecode(encrypted));
        }
        public double SaltingCombinations()
        {
            if (saltPossibilities == null)
            {
                return Math.Pow(255, saltLen); //because non 0 bytes only
            }
            return (saltLen * saltLen);
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, encryptor);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, decryptor);
        }

        protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            MemoryStream stream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return stream.ToArray();
        }
        //http://stackoverflow.com/questions/2449263/aes-two-way-encryption-with-salting#11543373
        private byte[] AddSalt(string unencrypted)
        {
            //Translates our text value into a byte array.
            Byte[] pepper = encoder.GetBytes(unencrypted);
            if (saltLen == 0) { return pepper; }
            return AddSalt(pepper, GetSalt(), GetSalt());
        }
        private byte[] AddSalt(byte[] pepper, byte[] beginSalt, byte[] endSalt)
        {
            Byte[] bytes = new byte[2 * saltLen + pepper.Length];
            System.Buffer.BlockCopy(beginSalt, 0, bytes, 0, saltLen);
            System.Buffer.BlockCopy(pepper, 0, bytes, saltLen, pepper.Length);
            System.Buffer.BlockCopy(endSalt, 0, bytes, saltLen + pepper.Length, saltLen);
            return bytes;
        }
        private byte[] GetSalt()
        {
            if (saltPossibilities == null)
            {
                Byte[] salt = new byte[saltLen];
                RNGcrypto.GetNonZeroBytes(salt);
                return salt;
            }
            else
            {
                return saltPossibilities[Rndm.Next(saltLen)];
            }
        }
        private string RemoveSalt(byte[] decryptedBytes)
        {
            // remove salt
            int len = decryptedBytes.Length - 2 * saltLen;
            Byte[] pepper = new Byte[len];
            System.Buffer.BlockCopy(decryptedBytes, saltLen, pepper, 0, len);
            return encoder.GetString(pepper);
        }
    }
}