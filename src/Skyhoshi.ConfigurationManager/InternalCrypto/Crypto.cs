using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Skyhoshi.Configuration.InternalCrypto
{
    public enum AvailableAlgorithms
    {

    }

    public class Crypto
    {
        private CryptoInformation cryptoInformation { get; set; } = new CryptoInformation(true);

        public CryptoInformation CryptoInformation => cryptoInformation;
        private SymmetricAlgorithm algorithm { get; set; }

        private AesManaged AesManaged { get; set; } = new AesManaged();

        public Crypto()
        {

        }

        public Crypto(SymmetricAlgorithm algorithm)
        {
            this.algorithm = algorithm;
        }

        public Crypto(CryptoInformation cryptoInformation) : this(Aes.Create(), cryptoInformation)
        {
            this.cryptoInformation = cryptoInformation;
        }

        public Crypto(SymmetricAlgorithm algorithm, CryptoInformation cryptoInformation)
        {
            this.algorithm = algorithm;
            this.cryptoInformation = cryptoInformation;
        }

        private string GetReturnValueFromByteArray(byte[] encryptedBytes)
        {
            System.Diagnostics.Debug.WriteLine($"byte Array Count: {encryptedBytes.Count()}");

            return Convert.ToBase64String(encryptedBytes);
        }

        #region Using CryptStream
        public string Encrypt(string plainText)
        {
            return EncryptAsHexString(plainText);
        }

        public string EncryptAsHexString(string plainText)
        {
            byte[] encryptedBytes = EncryptStringByKeyByIV(plainText, cryptoInformation.Key, cryptoInformation.IV);

            return BitConverter.ToString(encryptedBytes);
        }
        public byte[] EncryptByte(string plainText)
        {
            return EncryptStringByKeyByIV(plainText, cryptoInformation.Key, cryptoInformation.IV);
        }
        public byte[] EncryptStringByKeyByIV(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        public string Decrypt(string cipherText)
        {
            return DecryptFromHexString(cipherText);
        }

        public string DecryptFromHexString(string cipherText)
        {
            byte[] CipherBytes = cryptoInformation.StringToByteArray(cipherText);
            return DecryptByteByKeyByIV(CipherBytes, cryptoInformation.Key, cryptoInformation.IV);
        }
        public string DecryptByte(byte[] cipherText)
        {
            return DecryptByteByKeyByIV(cipherText, cryptoInformation.Key, cryptoInformation.IV);
        }
        public string DecryptByteByKeyByIV(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            plaintext = reader.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        #endregion

        #region MyRegion




        //#region PlainTextCryption
        //[Obsolete("", true)]
        //public string EncryptPlainText(string plainText)
        //{
        //    string encryptedReturnValue = "";
        //    object cryptographicObject = CryptoConfig.CreateFromName(cryptoInformation.TypeNameString);

        //    ICryptoTransform encryptProcessor = null;

        //    HashAlgorithm hashAlgorithm = HashAlgorithm.Create("System.Security.Cryptography.SHA256CryptoServiceProvider");


        //    switch (cryptographicObject)
        //    {
        //        case AesCryptoServiceProvider aesCryptoServiceProvider:
        //            encryptProcessor = aesCryptoServiceProvider.CreateEncryptor(cryptoInformation.Key, cryptoInformation.IV);
        //            break;
        //        case SHA256CryptoServiceProvider sha256CryptoServiceProvider:
        //            sha256CryptoServiceProvider.Initialize();
        //            encryptProcessor = sha256CryptoServiceProvider;
        //            break;
        //        case RSACryptoServiceProvider rasCryptoServiceProvider:
        //            break;
        //    }
        //    if (encryptProcessor == null) throw new CryptographicUnexpectedOperationException("No encryptor selected.");

        //    byte[] plainTextBytes = cryptoInformation.GetBytesFromString(plainText);

        //    //byte[] encryptedBytes  = encryptProcessor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
        //    ICryptoTransform trans = encryptProcessor;
        //    byte[] input = plainTextBytes;
        //    byte[] output = new byte[trans.OutputBlockSize];
        //    int bs = trans.InputBlockSize;
        //    int full = input.Length / bs;
        //    int partial = input.Length % bs;
        //    int pos = 0;
        //    byte[] final = new byte[full + partial];
        //    for (int i = 0; i < full; i++)
        //    {
        //        trans.TransformBlock(input, pos, bs, output, pos);
        //        pos += bs;
        //    }

        //    if (partial > 0)
        //    {
        //        final = trans.TransformFinalBlock(input, pos, partial);

        //    }
        //    //byte[] encryptedBytes = new byte[full + partial];
        //    Array.Copy(final, 0, output, pos, partial);
        //    encryptedReturnValue = GetReturnValueFromByteArray(output);
        //    return encryptedReturnValue;



        //}

        //[Obsolete("", true)]
        //public string DecryptCipherText(string cipherText)
        //{
        //    string decryptedReturnValue = "";
        //    object cryptographicObject = CryptoConfig.CreateFromName(cryptoInformation.TypeNameString);

        //    ICryptoTransform cryptoProcessor = null;

        //    HashAlgorithm hashAlgorithm = HashAlgorithm.Create("System.Security.Cryptography.SHA256CryptoServiceProvider");


        //    switch (cryptographicObject)
        //    {
        //        case AesCryptoServiceProvider aesCryptoServiceProvider:
        //            cryptoProcessor = aesCryptoServiceProvider.CreateDecryptor(cryptoInformation.Key, cryptoInformation.IV);
        //            break;
        //        case SHA256CryptoServiceProvider sha256CryptoServiceProvider:
        //            sha256CryptoServiceProvider.Initialize();
        //            cryptoProcessor = sha256CryptoServiceProvider;
        //            break;
        //        case RSACryptoServiceProvider rasCryptoServiceProvider:
        //            break;
        //    }
        //    if (cryptoProcessor == null) throw new CryptographicUnexpectedOperationException("No decryptor selected.");

        //    byte[] textBytes = cryptoInformation.GetBytesFromString(cipherText);
        //    byte[] decryptedBytes = cryptoProcessor.TransformFinalBlock(textBytes, 0, textBytes.Length);
        //    decryptedReturnValue = cryptoInformation.GetString(decryptedBytes);
        //    return decryptedReturnValue;
        //}

        //[Obsolete("", true)]
        //public string DecryptByteArrayAES(byte[] array)
        //{
        //    string cipherText = cryptoInformation.GetString(array);
        //    return DecryptStringAES(cipherText);
        //}
        //#endregion
        //[Obsolete("", false)]
        //public string EncryptStringAES(string plainText)
        //{

        //    byte[] array;
        //    using (System.Security.Cryptography.Aes aes = Aes.Create())
        //    {
        //        if (cryptoInformation.UseCustomKey)
        //        {
        //            aes.Key = cryptoInformation.Key;
        //            System.Diagnostics.Debug.WriteLine($"Key:{aes.Key}");
        //        }
        //        else
        //        {
        //            aes.Key = AesManaged.Key;
        //            string bytesOfKey = "";
        //            foreach (byte b in aes.Key)
        //            {
        //                bytesOfKey += $"{b.ToString()},";
        //            }

        //            bytesOfKey = bytesOfKey.Remove(bytesOfKey.Length - 1);
        //            System.Diagnostics.Debug.WriteLine($"Key:{bytesOfKey}");
        //        }

        //        if (cryptoInformation.UseCustomInitializationVector)
        //        {
        //            aes.IV = cryptoInformation.IV;
        //        }
        //        else
        //        {
        //            aes.IV = AesManaged.IV;
        //        }

        //        ICryptoTransform encryptProcessor = aes.CreateEncryptor(aes.Key, aes.IV);

        //        using (System.IO.MemoryStream memoryStream = new MemoryStream())
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptProcessor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
        //                {
        //                    streamWriter.Write(plainText);
        //                }

        //                array = memoryStream.ToArray();
        //            }
        //        }
        //    }

        //    string bytesOfArray = "";
        //    foreach (byte b in array)
        //    {
        //        bytesOfArray += $"{b.ToString()},";
        //    }

        //    bytesOfArray = bytesOfArray.Remove(bytesOfArray.Length - 1);
        //    System.Diagnostics.Debug.WriteLine($"EncryptedArray:{bytesOfArray}");

        //    //return Encoding.ASCII.GetString(array);
        //    return Convert.ToBase64String(array, Base64FormattingOptions.None);
        //    //return Convert.ToBase64String(array);

        //}
        //[Obsolete("", false)]
        //public string DecryptStringAES(string cipherText)
        //{
        //    byte[] buffer = cryptoInformation.GetBytesFromString(cipherText);

        //    using (Aes aes = Aes.Create())
        //    {
        //        if (cryptoInformation.UseCustomKey)
        //        {
        //            aes.Key = cryptoInformation.Key;
        //        }
        //        else
        //        {
        //            aes.Key = AesManaged.Key;
        //        }

        //        if (cryptoInformation.UseCustomInitializationVector)
        //        {
        //            aes.IV = cryptoInformation.IV;
        //        }
        //        else
        //        {
        //            aes.IV = AesManaged.IV;
        //        }
        //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        //        using (MemoryStream memoryStream = new MemoryStream(buffer))
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
        //                {
        //                    return streamReader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //}

        //[Obsolete("", true)]
        //public string EncryptString(string plainText)
        //{
        //    if (string.IsNullOrWhiteSpace(this.cryptoInformation.TypeNameString))
        //    {
        //        throw new ProviderException("Unable to identify selected Security Crypto Algorithm");
        //    }
        //    byte[] array;

        //    using (algorithm = SymmetricAlgorithm.Create(cryptoInformation.TypeNameString))
        //    {
        //        if (cryptoInformation.UseCustomKey)
        //        {
        //            algorithm.Key = cryptoInformation.Key;
        //        }
        //        else
        //        {
        //            algorithm.Key = AesManaged.Key;
        //        }

        //        if (cryptoInformation.UseCustomInitializationVector)
        //        {
        //            algorithm.IV = cryptoInformation.IV;
        //        }
        //        else
        //        {
        //            algorithm.IV = AesManaged.IV;
        //        }

        //        ICryptoTransform encryptProcessor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);

        //        using (System.IO.MemoryStream memoryStream = new MemoryStream())
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptProcessor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
        //                {
        //                    streamWriter.Write(plainText);
        //                }

        //                array = memoryStream.ToArray();
        //            }
        //        }
        //    }

        //    return Encoding.ASCII.GetString(array);// Convert.ToBase64String(array);
        //    //return Encoding.ASCII.GetString(array);
        //    //return Encoding.ASCII
        //    //return Convert.ToBase64String(array);
        //}
        //[Obsolete("", true)]
        //public string DecryptString(string cipherText)
        //{
        //    if (string.IsNullOrWhiteSpace(this.cryptoInformation.TypeNameString))
        //    {
        //        throw new ProviderException("Unable to identify selected Security Crypto Algorithm");
        //    }

        //    byte[] buffer = cryptoInformation.GetBytesFromString(cipherText);

        //    using (Aes aes = Aes.Create())
        //    {
        //        if (cryptoInformation.UseCustomKey)
        //        {
        //            aes.Key = cryptoInformation.Key;
        //        }
        //        else
        //        {
        //            aes.Key = AesManaged.Key;
        //        }

        //        if (cryptoInformation.UseCustomInitializationVector)
        //        {
        //            aes.IV = cryptoInformation.IV;
        //        }
        //        else
        //        {
        //            aes.IV = AesManaged.IV;
        //        }
        //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        //        using (MemoryStream memoryStream = new MemoryStream(buffer))
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
        //                {
        //                    return streamReader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
