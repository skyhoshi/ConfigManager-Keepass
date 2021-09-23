using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skyhoshi.Configuration.InternalCrypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeePassLib_Unit_Tests.Diagnostic;

namespace Skyhoshi.Configuration.InternalCrypto.Tests
{
    [TestClass()]
    public class CryptoTests
    {
        private Skyhoshi.Configuration.InternalCrypto.Crypto cryptoProcessor { get; set; }

        private Stopwatch stopWatch { get; set; }

        public static string KnownGoodHex { get; set; } = "13-AF-57-66-AA-3E-7F-0A-28-65-6C-45-2C-7C-05-25-48-C5-23-88-16-F2-02-F5-ED-C0-DA-F5-EB-61-5E-04";

        [TestInitialize]
        public void Setup()
        {
            CryptoInformation cryptoInformation = new CryptoInformation();
            cryptoInformation.TypeNameString = "AES";
            cryptoInformation.UseCustomInitializationVector = false;
            cryptoInformation.UseCustomKey = false;
            cryptoProcessor = new Crypto(cryptoInformation);
            stopWatch = Stopwatch.StartNew();
            stopWatch.Stop();
            stopWatch.Reset();
        }

        [TestClass]
        public class DynamicAlgorithmsTests
        {
            private Skyhoshi.Configuration.InternalCrypto.Crypto cryptoProcessor { get; set; }

            private Stopwatch stopWatch { get; set; }

            [TestInitialize]
            public void Setup()
            {
                CryptoInformation cryptoInformation = new CryptoInformation();
                cryptoInformation.TypeNameString = "AES";
                cryptoInformation.UseCustomInitializationVector = false;
                cryptoInformation.UseCustomKey = false;
                cryptoProcessor = new Crypto(cryptoInformation);
                stopWatch = Stopwatch.StartNew();
                stopWatch.Stop();
                stopWatch.Reset();
            }

            [TestMethod()]
            public void EncryptStringTest()
            {
                int threadSpinWaitIterations = 100000;
                string testString = "DecryptionTestString01";
                string actualEncryptedValue = "";
                string actualHexEncryptedValue = "";
                string actualDecryptedValue = "";
                string expectedValue = "DecryptionTestString01";

                actualEncryptedValue = cryptoProcessor.Encrypt(testString);
                actualHexEncryptedValue = BitConverter.ToString(Encoding.Default.GetBytes(actualEncryptedValue));
                //Debug.WriteLine($"Hex: {actualHexEncryptedValue}");
                System.Diagnostics.Debug.WriteLine($"Actual Encrypted Value: {actualEncryptedValue}");
                //stopWatch.Start();
                //System.Threading.Thread.SpinWait(threadSpinWaitIterations);
                //stopWatch.Stop();
                //TimeSpan elapsedTime = stopWatch.Elapsed;
                //System.Diagnostics.Debug.WriteLine($"StopWatch Result for Thread SpinWait({threadSpinWaitIterations}) : Microseconds = {stopWatch.Elapsed.Microseconds()} | NanoSeconds = {stopWatch.Elapsed.Nanoseconds()} | Ticks = {stopWatch.ElapsedTicks} | Milliseconds = {stopWatch.ElapsedMilliseconds}");
                ////actualDecryptedValue = cryptoProcessor.DecryptByteByKeyByIV(CryptoInformation.StringToByteArray(actualHexEncryptedValue), cryptoProcessor.CryptoInformation.Key, cryptoProcessor.CryptoInformation.IV);
                actualDecryptedValue = cryptoProcessor.Decrypt(actualEncryptedValue);
                System.Diagnostics.Debug.WriteLine($"Actual Decrypted Value: {actualDecryptedValue}");
                Assert.IsTrue(expectedValue == actualDecryptedValue, "expectedValue != actualDecryptedValue");
            }

            [TestMethod()]
            public void DecryptStringTest()
            {
                string testingString = KnownGoodHex;
                string actualDecryptedValue = "";
                string expectedDecryptedValue = "DecryptionTestString01";
                actualDecryptedValue = cryptoProcessor.Decrypt(testingString);
                System.Diagnostics.Debug.WriteLine($"Actual Decrypted Value: {actualDecryptedValue}");
            }

        }

        [TestClass]
        public class AESAlgorithmTests
        {

            private Skyhoshi.Configuration.InternalCrypto.Crypto cryptoProcessor { get; set; }

            private Stopwatch stopWatch { get; set; }

            [TestInitialize]
            public void Setup()
            {
                CryptoInformation cryptoInformation = new CryptoInformation();
                cryptoInformation.TypeNameString = "AES";
                cryptoInformation.UseCustomInitializationVector = false;
                cryptoInformation.UseCustomKey = false;
                cryptoProcessor = new Crypto(cryptoInformation);
                stopWatch = Stopwatch.StartNew();
                stopWatch.Stop();
                stopWatch.Reset();
            }

            [TestMethod()]
            public void EncryptStringAESTest()
            {
                //int threadSpinWaitIterations = 100000;
                //string testString = "DecryptionTestString01";
                //string actualEncryptedValue = "";
                //string actualDecryptedValue = "";
                //string expectedValue = "WeirdTestString";

                //actualEncryptedValue = cryptoProcessor.EncryptStringAES(testString);
                //System.Diagnostics.Debug.WriteLine($"Actual Encrypted Value: {actualEncryptedValue}");
                //stopWatch.Start();
                //System.Threading.Thread.SpinWait(threadSpinWaitIterations);
                //stopWatch.Stop();
                //TimeSpan elapsedTime = stopWatch.Elapsed;
                //System.Diagnostics.Debug.WriteLine(
                //    $"StopWatch Result for Thread SpinWait({threadSpinWaitIterations}) : Microseconds = {stopWatch.Elapsed.Microseconds()} | NanoSeconds = {stopWatch.Elapsed.Nanoseconds()} | Ticks = {stopWatch.ElapsedTicks} | Milliseconds = {stopWatch.ElapsedMilliseconds}");
                //actualDecryptedValue = cryptoProcessor.DecryptStringAES(actualEncryptedValue);
                //System.Diagnostics.Debug.WriteLine($"Actual Decrypted Value: {actualDecryptedValue}");
            }

            [TestMethod()]
            public void DecryptByteArrayAESTest()
            {
                string testByteArrayAsString = "137,112,14,82,32,193,50,42,49,60,104,164,219,155,93,36,43,171,78,6,172,33,161,11,204,175,139,136,217,226,215,123";
                int numberOfBytes = testByteArrayAsString.Split(',').Length;
                byte[] arrayBytes = new byte[numberOfBytes];
                int byteCounter = 0;
                foreach (string s in testByteArrayAsString.Split(','))
                {
                    arrayBytes[byteCounter] = byte.Parse(s);
                }
                string testingString = "tRPYZy8pbWVXKEN2PoMM+kfmAwCLLdqBWwh/0hTrlOE=";
                string actualDecryptedValue = "";
                string expectedDecryptedValue = "DecryptionTestString01";
                //actualDecryptedValue = cryptoProcessor.DecryptByteArrayAES(arrayBytes);
                System.Diagnostics.Debug.WriteLine($"Actual Decrypted Value: {actualDecryptedValue}");

            }
        }
    }
}