using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CashLoanTool.Helper
{
    public static class HdbRSA
    {
        public static readonly int KeySize = 1024;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string _salt;
        public static string Salt
        {
            get
            {
                if (string.IsNullOrEmpty(_salt)) throw new InvalidOperationException("Salt is not set");
                return _salt;
            }
            set
            {
                _salt = value;
            }
        }
        private static string _privateKey;
        public static string PrivateKey
        {
            get
            {
                if (string.IsNullOrEmpty(_privateKey)) throw new InvalidOperationException("Key is null or empty");
                return _privateKey;
            }
            set
            {
                _privateKey = value;
            }

        }
        private static string _publicKey;
        public static string PublicKey
        {
            get
            {
                if(string.IsNullOrEmpty(_publicKey)) throw new InvalidOperationException("Key is null or empty");
                return _publicKey;
            }
            set
            {
                _publicKey = value;
            }
        }
        private static string RSA_Section = "RSA";
        private static string PublicKeyBinding = "PublicKey";
        private static string PrivateKeyBinding = "PrivateKey";
        private static string SaltKeyBinding = "Salt";

        public static void ReadRSAKeys(IConfiguration configuration)
        {
            string publicKeyFilename = configuration.GetSection(RSA_Section).GetValue<string>(PublicKeyBinding);
            string privateKeyFilename = configuration.GetSection(RSA_Section).GetValue<string>(PrivateKeyBinding);
            string saltFilename = configuration.GetSection(RSA_Section).GetValue<string>(SaltKeyBinding);
            var publicKeyInfo = new FileInfo($"{Program.ExeDir}\\{publicKeyFilename}");
            var privateKeyInfo = new FileInfo($"{Program.ExeDir}\\{privateKeyFilename}");
            var saltInfo = new FileInfo($"{Program.ExeDir}\\{saltFilename}");

            if (!publicKeyInfo.Exists || !privateKeyInfo.Exists || !saltInfo.Exists)
                throw new InvalidOperationException("Keys file not exists.");
            PublicKey = File.ReadAllText(publicKeyInfo.FullName);
            PrivateKey = File.ReadAllText(privateKeyInfo.FullName);
            Salt = File.ReadAllText(saltInfo.FullName);
        }

        public static string Hash(string data)
        {
            //logger.Info($"B4 hash: {data}");
            using (var sha = SHA256.Create())
            {
                Byte[] hashedBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant().Trim();
                //logger.Info($"Hash: {hash}");
                return hash;
            }
        }

        public static string SignData(string dataHash)
        {
            var hashBytes = Encoding.UTF8.GetBytes(dataHash);

            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(PrivateKey);
                    var encryptedData = rsa.SignData(hashBytes, new SHA1CryptoServiceProvider());
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static bool Verify(string dataHash, string sign)
        {
            var hashBytes = Encoding.UTF8.GetBytes(dataHash);
            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                try
                {
                    rsa.FromXmlString(PublicKey);
                    var signData = Convert.FromBase64String(sign);
                    return rsa.VerifyData(hashBytes, new SHA1CryptoServiceProvider(), signData);
                }
                catch(FormatException ex) //Invalid base64 length
                {
                    logger.Error(ex.Message);
                    return false;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
