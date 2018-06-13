using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Interfaces;
using KeePassLib.Keys;
using KeePassLib.Serialization;

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
            try
            {
                var ioConnInfo = new IOConnectionInfo() { Path = @"M:\ConfigManager-Keepass\data\ConfigurationManager.kdbx" };
                ioConnInfo = new IOConnectionInfo() { Path = @"M:\ConfigManager-Keepass\data\NewDatabase.kdbx" };

                var compKey = new CompositeKey();
                compKey.AddUserKey(new KcpKeyFile(@"M:\ConfigManager-Keepass\data\NewDatabase.key"));
                //(new MemoryStream(System.IO.File.ReadAllBytes($@"")), "MyFaceRecongnitionApplicationDB"));
                //compKey.AddUserKey(new KcpPassword("P@ss0wrd"));
                //This is a do nothing logger but the Interface should be implimented if you are going to make your application public, I recommend creating an applications insights account and using the logger to write trace events to that service.
                IStatusLogger nullStatusLogger = new NullStatusLogger();


                Database.Open(ioConnInfo, compKey, nullStatusLogger);
                IsDatabaseOpen = true;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public static string GetSecureKey(string secureKeyName = "Sample Entry")
        {
            if (!IsDatabaseOpen) OpenDatabase();
            return GetValueFromDB(ref db, secureKeyName);
        }

        static string GetValueFromDB(ref KeePassLib.PwDatabase pdb, string SearchString = "")
        {
            if (string.IsNullOrWhiteSpace(SearchString))
            {
                throw new ArgumentException("Search String Cannot be blank");
            }
            //byte[] keyBytes = System.Text.Encoding.Default.GetBytes(key); //new []{ System.Text.EncodingInfo}
            //Guid i = Guid.Parse(key);
            //var pwUuID = new PwUuid(i.ToByteArray());
            SearchParameters sp = new SearchParameters();
            sp.SearchInStringNames = true;
            sp.SearchString = SearchString;
            PwObjectList<PwEntry> pwl = new PwObjectList<PwEntry>();
            pdb.RootGroup.SearchEntries(sp, pwl);
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
    }
}
