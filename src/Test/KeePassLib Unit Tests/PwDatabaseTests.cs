using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeePassLib_Unit_Tests
{
    [TestClass]
    public class PwDatabaseTests
    {
        [TestMethod]
        public void PwDatabase_Open_Test_SearchForKey()
        {
            string secureKeyName = "Sample Entry #2";
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey(secureKeyName);
            System.Diagnostics.Debug.WriteLine(returnValue);
            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void PwDatabase_Open_Test_SearchForKeyEmptyString()
        {
            string secureKeyName = "";

           string returnValue =  Skyhoshi.Configuration.ConfigurationManager.GetSecureKey(secureKeyName);
            
            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }

        [TestMethod]
        public void PwDatabase_Open_Test_SearchForKeyDefaultParameter()
        {
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.GetSecureKey();

            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }


        [TestMethod]
        public void PwDatabase_Open_Test_AppSettings_System_ConfigurationManager()
        {
            string returnValue = Skyhoshi.Configuration.ConfigurationManager.AppSetting["MyAppSetting"];

            Assert.IsFalse(string.IsNullOrWhiteSpace(returnValue));
        }
    }
}
