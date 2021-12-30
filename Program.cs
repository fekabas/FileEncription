using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileEncription
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("Must provide the global file path and the action to execute (encrypt or decrypt)");

            string fileName = args[0]; // $"C:\\Users\\file-to-encrypt.txt";

            bool encrypt = args[1] == "e" || args[1] == "encrypt" ? true : args[1] == "d" || args[1] == "decrypt" ? false : throw new ArgumentException("Encrypt/decrypt argument should be 'e' or 'd'");

            var fileContent = File.ReadAllText(fileName);

            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            var newContent = encrypt ? AesOperation.EncryptString(key, fileContent) : AesOperation.DecryptString(key, fileContent);

            File.WriteAllText(fileName, newContent);
        }
    }

    public class AesOperation
    {
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
