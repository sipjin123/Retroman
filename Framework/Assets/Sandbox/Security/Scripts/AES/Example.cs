using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Framework;

using Sandbox.Security;

namespace Assets.USecurity
{
    /// <summary>
    /// Usage example.
    /// </summary>
    public class Example : MonoBehaviour
    {
        public Text Text;
        public string Sample = "test";

        private AESEncyrption Encryption;
        
        public void Encrypt()
        {
            Encryption = AESEncryptionFactory.CreateDefaultEncryption();

            string encrypted = Encryption.Encrypt(Sample);
            string decryped = Encryption.Decrypt(encrypted);

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Text:{0}\n", Sample);
            builder.AppendFormat("Mode:{0}\n", Encryption.Mode);
            builder.AppendFormat("KeySize:{0}\n", Encryption.KeySize);
            builder.AppendFormat("BlockSizE:{0}\n", Encryption.BlockSize);
            builder.AppendFormat("KEY:{0}\n", Encryption.SecretKey);
            builder.AppendFormat("IV:{0}\n", Encryption.InputVector);
            builder.AppendFormat("Encrypted:{0}\n", encrypted);
            builder.AppendFormat("Decrypted:{0}\n", decryped);

            Text.text = builder.ToString();
            Debug.LogErrorFormat(Text.text);
        }
    }
}