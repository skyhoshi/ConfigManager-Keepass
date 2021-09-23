using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Interfaces;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using Skyhoshi.Configuration.Database;
using Skyhoshi.Configuration.Database.Search;
using Skyhoshi.Configuration.InternalCrypto;
using Skyhoshi.Configuration.Logging;

namespace Skyhoshi.Configuration
{
    public static class ConfigurationManager
    {
        private static bool IsDatabaseOpen { get; set; }
        private static KeePassLib.PwDatabase db = new PwDatabase();


        private static PwDatabase Database
        {
            get
            {
                //if (!IsDatabaseOpen) OpenDatabase();
                return db;
            }
            set
            {
                IsDatabaseOpen = db.IsOpen;
                if (!IsDatabaseOpen) OpenDatabase();
                db = value;
            }
        }

        public static InternalCrypto.Crypto CryptoProcessor { get; set; } = new Crypto();
        public static Database.DatabaseInformation DatabaseInfo { get; set; } = new DatabaseInformation();

        public static SearchInformation SearchInformation { get; set; } = new SearchInformation();

        public static NameValueCollection AppSetting
        {
            get { return System.Configuration.ConfigurationManager.AppSettings; }
        }

        public static ConnectionStringSettingsCollection ConnectionString
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings; }
        }

        private static void CloseDatabase()
        {
            db.Close();
            IsDatabaseOpen = db.IsOpen;
        }

        private static void OpenDatabase()
        {
            if (!string.IsNullOrWhiteSpace(DatabaseInfo.DatabaseFileName))
            {
                if (!string.IsNullOrWhiteSpace(DatabaseInfo.DatabaseFileFullLocation))
                {
                    if (!File.Exists(DatabaseInfo.DatabaseFileFullLocation))
                    {
                        throw new FileNotFoundException($"DATABASE FILE ERROR{DatabaseInfo.DatabaseFileFullLocation} does not exists. Please check the locations and try again.");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(DatabaseInfo.DatabaseKeyFileName))
            {
                if (!string.IsNullOrWhiteSpace(DatabaseInfo.DatabaseKeyFileFullLocation))
                {
                    if (!File.Exists(DatabaseInfo.DatabaseKeyFileFullLocation))
                    {
                        throw new FileNotFoundException($"KEY FILE ERROR: {DatabaseInfo.DatabaseKeyFileFullLocation} does not exists. Please check the locations and try again.");
                    }
                }
            }
            
            try
            {
                var ioConnInfo = new IOConnectionInfo() { Path = DatabaseInfo.DatabaseFileFullLocation };

                var compKey = CreateCompositeKey();
#if DEBUG
                IStatusLogger statusLogger = new DebugStatusLogger();
                Database.Open(ioConnInfo, compKey, statusLogger);

#else
                //This is a do nothing logger but the Interface should be implemented if you are going to make your application public, I recommend creating an applications insights account and using the logger to write trace events to that service.
                IStatusLogger nullStatusLogger = new NullStatusLogger();
                Database.Open(ioConnInfo, compKey, nullStatusLogger);
#endif

                IsDatabaseOpen = true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static CompositeKey CreateCompositeKey()
        {
            var compKey = new CompositeKey();
            if (!string.IsNullOrWhiteSpace(DatabaseInfo.DatabaseKeyFileName))
            {
                compKey.AddUserKey(new KcpKeyFile(DatabaseInfo.DatabaseKeyFileFullLocation));
            }

            if (DatabaseInfo.DatabaseMasterPassword != null)
            {
                compKey.AddUserKey(new KcpPassword(GetDatabaseMasterPassword()));
            }

            if (DatabaseInfo.DatabaseMasterCustomKey != null)
            {
                if (string.IsNullOrWhiteSpace(DatabaseInfo.DatabaseMasterCustomKeyName))
                {
                    throw new ArgumentException("A Master Custom Key cannot be without a name");
                }

                compKey.AddUserKey(new KcpCustomKey(DatabaseInfo.DatabaseMasterCustomKeyName, Encoding.Default.GetBytes(DatabaseInfo.DatabaseMasterCustomKey), true));
            }

            if (DatabaseInfo.UseMasterKeyUserAccount)
            {
                compKey.AddUserKey(new KcpUserAccount());
            }

            if (compKey.UserKeyCount == 0)
            {
                throw new AccessViolationException("Your database MUST have some sort of key to access the database itself. blank key or passwords are not allowed.");
            }
            return compKey;
        }


        public static string GetSecureKey(string secureKeyName)
        {
            if (string.IsNullOrWhiteSpace(secureKeyName))
            {
                throw new ArgumentNullException("You must specify the name of the key you wish to retrieve");
            }

            
            CryptoProcessor = new Crypto();

            if (!IsDatabaseOpen) OpenDatabase();
            return GetValueFromDB(ref db, secureKeyName);
        }

       private static string GetValueFromDB(ref KeePassLib.PwDatabase pdb, string searchString = "")
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                throw new ArgumentException("Search String Cannot be blank");
            }

            SearchProcessor processor = new SearchProcessor();

            processor.Database = pdb;
            //byte[] keyBytes = System.Text.Encoding.Default.GetBytes(key); //new []{ System.Text.EncodingInfo}
            //Guid i = Guid.Parse(key);
            //var pwUuID = new PwUuid(i.ToByteArray());

            //SearchParameters sp = new SearchParameters();
            //sp.SearchInStringNames = true;
            //sp.SearchString = searchString;
            //PwObjectList<PwEntry> pwl = new PwObjectList<PwEntry>();
            //pdb.RootGroup.SearchEntries(sp, pwl);

            PwObjectList<PwEntry> pwl = processor.GetListOfEntriesBySearchByString(searchString);
            System.Diagnostics.Debug.WriteLine($"Entries Found: {pwl.Count()}");
            string k = string.Empty;
            foreach (var entry in pwl.CloneShallowToList())
            {
                foreach (var l in entry.Strings.ToList())
                {
                    if (l.Key == "Password")
                    {
                        k = l.Value.ReadString();
                    }
                }
            }
            return k;
        }



        private static string GetDatabaseMasterPassword()
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(DatabaseInfo.DatabaseMasterPassword);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static void SetDatabaseMasterPassword(SecureString password = null, string encryptedTextPassword = "")
        {
            if (password != null)
            {
                DatabaseInfo.DatabaseMasterPassword = password;
            }

            if (!string.IsNullOrWhiteSpace(encryptedTextPassword))
            {
                string unencryptedPassword = CryptoProcessor.Decrypt(encryptedTextPassword);
                if (string.IsNullOrWhiteSpace(unencryptedPassword))
                {
                    throw new ArgumentException("Encrypted Text Failed to decrypt. Please Reset the Password and update storage");
                }
                SecureString secureStringPassword = new SecureString();
                foreach (char c in unencryptedPassword)
                {
                    secureStringPassword.AppendChar(c);
                }

                if (password != null)
                {
                    if (password.GetHashCode() != secureStringPassword.GetHashCode())
                    {
                        throw new ArgumentException("You can only pass one password type, please remove one password from the method call and try again.");
                    }
                }
                else
                {
                    DatabaseInfo.DatabaseMasterPassword = secureStringPassword;
                }
            }
        }
        public static void SetDatabaseMasterPassword(SecureString password = null)
        {
            if (password != null)
            {
                DatabaseInfo.DatabaseMasterPassword = password;
            }

        }
        public static void SetDatabaseMasterPassword(string encryptedTextPassword = "")
        {
            if (!string.IsNullOrWhiteSpace(encryptedTextPassword))
            {
                string unencryptedPassword = CryptoProcessor.Decrypt(encryptedTextPassword);
                if (string.IsNullOrWhiteSpace(unencryptedPassword))
                {
                    throw new ArgumentException("Encrypted Text Failed to decrypt. Please Reset the Password and update storage");
                }
                SecureString secureStringPassword = new SecureString();
                foreach (char c in unencryptedPassword)
                {
                    secureStringPassword.AppendChar(c);
                }
                DatabaseInfo.DatabaseMasterPassword = secureStringPassword;
                DatabaseInfo.DatabaseMasterPassword.MakeReadOnly();
            }
        }

        public static void SetDatabaseFileInformation(string databaseStorageLocation, string databaseDatabaseFileName, string databaseKeyFileName)
        {
            //TODO: Add Advanced File Location and Validation ... validation at Set Time to prevent unwanted memory use and leakage.
            DatabaseInfo.DatabaseStoragePath = databaseStorageLocation;
            DatabaseInfo.DatabaseFileName = databaseDatabaseFileName;
            DatabaseInfo.DatabaseKeyFileName = databaseKeyFileName;
        }
        public static void SetDatabaseFileInformation(string databaseStorageLocation, string databaseDatabaseFileName, string databaseKeyFileName, string databasePassword)
        {
            SetDatabaseFileInformation(databaseStorageLocation, databaseDatabaseFileName, databaseKeyFileName);
            string encryptedPassword = CryptoProcessor.Encrypt(databasePassword);
            SetDatabaseMasterPassword(null, encryptedPassword);
        }
    }
}
