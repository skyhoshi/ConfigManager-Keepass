using System;
using System.Data.Common;
using System.Security;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skyhoshi.Configuration;
using Skyhoshi.Configuration.Database;
using Skyhoshi.Configuration.Database.Search;
using Skyhoshi.Configuration.InternalCrypto;

namespace KeePassLib_Unit_Tests
{
    [TestClass]
    public class PwDatabaseTests
    {
        [TestInitialize]
        public void TestStartup()
        {
            ConfigurationManager.CryptoProcessor = new Crypto();
            ConfigurationManager.DatabaseInfo = new DatabaseInformation();
        }

        private void SetupSearchOptions()
        {
            SearchOptions options = new SearchOptions();
            options.Options = SearchParameterOptions.Title & SearchParameterOptions.Username;
            ConfigurationManager.SearchInformation.Options = options;
        }


        public void SetupDefaultDatabase()
        {
            ResetForAllTests();
            string databaseStorageLocation = $@"K:\By Services\github\Skyhoshi\ConfigManager-Keepass\data\";
            string databaseDatabaseFileName = "NewDatabaseByPasswordForDataRetrievalTests";
            string databaseKeyFileName = "";
            string databasePassword = "0987654321";
            ConfigurationManager.SetDatabaseFileInformation(databaseStorageLocation, databaseDatabaseFileName, databaseKeyFileName, databasePassword);
        }

        public void ResetDatabaseInformation()
        {
            ConfigurationManager.DatabaseInfo = new DatabaseInformation();
        }

        public void ResetCryptoInformation()
        {
            ConfigurationManager.CryptoProcessor = new Crypto();
        }

        public void ResetForAllTests()
        {
            ResetDatabaseInformation();
            ResetCryptoInformation();
        }

        [TestMethod]
        public void PwDatabase_Open_By_KeyFile_Test_SearchForKey()
        {
            string secureKeyName = "Sample Entry #2";
            string databaseStorageLocation = $@"K:\By Services\github\Skyhoshi\ConfigManager-Keepass\data";
            string databaseDatabaseFileName = "NewDatabaseByKey";
            string databaseKeyFileName = "NewDatabaseByKey";
            ConfigurationManager.SetDatabaseFileInformation(databaseStorageLocation, databaseDatabaseFileName, databaseKeyFileName);
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey(secureKeyName);
            System.Diagnostics.Debug.WriteLine($"DEBUG: {nameof(returnValue)}={returnValue}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }

        [TestMethod]
        public void PwDatabase_Open_By_Password_Test_SearchForKey()
        {
            string secureKeyName = "Sample Entry #2";
            string databaseStorageLocation = $@"K:\By Services\github\Skyhoshi\ConfigManager-Keepass\data\";
            string databaseDatabaseFileName = "NewDatabaseByPassword";
            string databaseKeyFileName = "";
            string databasePassword = "0987654321";
            ConfigurationManager.SetDatabaseFileInformation(databaseStorageLocation, databaseDatabaseFileName, databaseKeyFileName, databasePassword);
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey(secureKeyName);
            System.Diagnostics.Debug.WriteLine($"DEBUG: {nameof(returnValue)}={returnValue}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }

        [TestMethod]
        public void PwDatabase_Open_By_SecureString_Password_Test_SearchForKey()
        {
            string secureKeyName = "Sample Entry #2";
            string databaseStorageLocation = $@"K:\By Services\github\Skyhoshi\ConfigManager-Keepass\data\";
            string databaseDatabaseFileName = "NewDatabaseByPassword";
            string databaseKeyFileName = "";
            string databasePassword = "0987654321";
            SecureString password = new SecureString();
            foreach (char c in databasePassword)
            {
                password.AppendChar(c);
            }
            ConfigurationManager.SetDatabaseMasterPassword(password);
            ConfigurationManager.SetDatabaseFileInformation(databaseStorageLocation, databaseDatabaseFileName, databaseKeyFileName, databasePassword);
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey(secureKeyName);
            System.Diagnostics.Debug.WriteLine($"DEBUG: {nameof(returnValue)}={returnValue}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void PwDatabase_Open_Test_SearchForKeyEmptyString()
        {
            string secureKeyName = "";
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey(secureKeyName);

            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception), AllowDerivedTypes = true)]
        public void PwDatabase_Open_Test_SearchForKeyDefaultParameter_ThrowsException()
        {
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey("");

            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }

        [TestMethod]
        public void PwDatabase_Open_Test_SearchForKeySampleEntryString()
        {
            SetupSearchOptions();
            string expectedValue = "Password";
            SetupDefaultDatabase();
            string secureKeyName = "Sample Entry";
            string actualValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey(secureKeyName);

            System.Diagnostics.Debug.WriteLine($"actualValue: {actualValue}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actualValue), "string.IsNullOrWhiteSpace(actualValue)");
            Assert.AreEqual(expectedValue, actualValue,$"Ensure Search Settings are set correctly. Expected Search Options : Search Title,Username and Return Field Password");
        }


        [TestMethod]
        public void PwDatabase_Open_Test_AppSettings_System_ConfigurationManager()
        {
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.AppSetting["MyAppSetting"];

            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }
    }
}
