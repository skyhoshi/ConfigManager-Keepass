using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Skyhoshi.Configuration.Database
{
    public class DatabaseInformation
    {
        public DatabaseInformation()
        {

        }

        public DatabaseInformation(string storageLocationWithoutFileName, string databaseFileName, string password)
        {

        }
        public DatabaseInformation(string storageLocationWithoutFileName, string databaseFileName, string keyFileName, bool usesStandardExtensions)
        {

        }
        public DatabaseInformation(string storageLocationWithoutFileName, string databaseFileName, string keyFileName, string password)
        {

        }
        public DatabaseInformation(string storageLocationWithoutFileName, string databaseFileName, bool useUserAccount)
        {

        }
        public DatabaseInformation(string storageLocationWithoutFileName, string databaseFileName, bool isCustomKey, string customKeyName, string customKey)
        {

        }
        public string DatabaseNameExtension = ".kdbx";
        public string DatabaseKeyNameExtension = ".key";
        public string DatabaseStoragePath { get; set; } = "";
        public string DatabaseFileName { get; set; } = "ConfigurationManager";
        public string DatabaseKeyFileName { get; set; } = "ConfigurationManager";

        public string DatabaseFileFullLocation => $@"{DatabaseStoragePath}\{DatabaseFileName}{DatabaseNameExtension}";

        public string DatabaseKeyFileFullLocation => $@"{DatabaseStoragePath}\{DatabaseKeyFileName}{DatabaseKeyNameExtension}";

        public SecureString DatabaseMasterPassword { get; set; }

        public bool UseMasterKeyUserAccount { get; set; } = false;
        public string DatabaseMasterCustomKeyName { get; set; }
        public string DatabaseMasterCustomKey { get; set; }
    }
}
