using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Framework;

namespace Sandbox.Security
{
    public static class AESEncryptionFactory
    {
        public static readonly string SYNERGY88DIGITAL_SECRET_KEY = "26E9AF00970FE6639900E0170F42EFD2";
        public static readonly string SYNERGY88DIGITAL_INPUT_VECTOR = "8888888888888888";

        public static AESEncyrption CreateDefaultEncryption()
        {
            AESEncyrption encryption = new AESEncyrption()
            {
                SecretKey = SYNERGY88DIGITAL_SECRET_KEY,
                InputVector = SYNERGY88DIGITAL_INPUT_VECTOR,
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
            };

            return encryption;
        }
    }

    public sealed class AESEncyrption
    {
        private string _SecretKey;
        public string SecretKey
        {
            get { return _SecretKey; }
            set
            {
                _SecretKey = value;
                Key = value.ToASCIIBytes();
            }
        }

        private string _InputVector;
        public string InputVector
        {
            get { return _InputVector; }
            set
            {
                _InputVector = value;
                IV = value.ToASCIIBytes();
            }
        }

        private byte[] Key;
        private byte[] IV;
        //private int KeySize = 256;
        //private int BlockSize = 128;
        //private CipherMode Mode = CipherMode.CBC;
        //private PaddingMode Padding = PaddingMode.PKCS7;
        public int KeySize { get; set; }
        public int BlockSize { get; set; }
        public CipherMode Mode { get; set; }
        public PaddingMode Padding { get; set; }

        public void Validate()
        {

        }

        public string Encrypt(string text)
        {
            byte[] encrypted;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                rijAlg.KeySize = KeySize;
                rijAlg.BlockSize = BlockSize;
                rijAlg.Mode = Mode;
                rijAlg.Padding = Padding;
                
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(Key, IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(text);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted.ToBase64();
        }

        public string Decrypt(byte[] encrypted)
        {
            // Declare the string used to hold 
            // the decrypted text. 
            string decrypted = null;
            
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                //rijAlg.KeySize = KeySize;
                //rijAlg.BlockSize = BlockSize;
                //rijAlg.Mode = Mode;
                //rijAlg.Padding = Padding;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            decrypted = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return decrypted;
        }

        public string Decrypt(string encrypted)
        {
            return Decrypt(encrypted.FromBase64());
        }
    }
}
