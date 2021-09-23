using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using KeePassLib.Collections;

namespace Skyhoshi.Configuration.Database.Search
{
    public class SearchProcessor
    {
        public static PwDatabase SearchDatabase { get; set; }

        public PwDatabase Database
        {
            get => SearchDatabase;
            set => SearchDatabase = value;
        }

        public string GetValueOfFieldByFieldNameAndSearchText(string fieldName, string searchText, SearchOptions searchOptions = null)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ArgumentException("Search String Cannot be blank");
            }

            SearchProcessor processor = new SearchProcessor();

            //byte[] keyBytes = System.Text.Encoding.Default.GetBytes(key); //new []{ System.Text.EncodingInfo}
            //Guid i = Guid.Parse(key);
            //var pwUuID = new PwUuid(i.ToByteArray());

            //SearchParameters sp = new SearchParameters();
            //sp.SearchInStringNames = true;
            //sp.SearchString = searchString;
            //PwObjectList<PwEntry> pwl = new PwObjectList<PwEntry>();
            //pdb.RootGroup.SearchEntries(sp, pwl);
            PwObjectList<PwEntry> pwl = this.GetListOfEntriesBySearchByString(searchText);
            string k = string.Empty;
            foreach (var entry in pwl.CloneShallowToList())
            {
                foreach (var l in entry.Strings.ToList())
                {
                    if (l.Key == fieldName)
                    {
                        k = l.Value.ReadString();
                    }
                }
            }
            return k;
        }

        public PwObjectList<PwEntry> GetListOfEntriesBySearchByString(string searchString)
        {
            SearchOptions options = new SearchOptions();
            options.ExactValueMatch = true;
            return GetListOfEntriesBySearchByString(searchString, options);
        }

        public PwObjectList<PwEntry> GetListOfEntriesBySearchByString(string searchString, SearchOptions searchOptions)
        {
            return GetListOfEntriesBySearchByString(SearchDatabase, searchString, searchOptions);
        }

        public PwObjectList<PwEntry> GetListOfEntriesBySearchByString(KeePassLib.PwDatabase pdb, string searchString, SearchOptions searchOptions)
        {
            if (pdb == null)
            {
                pdb = SearchDatabase;
            }

            if (pdb.GetHashCode() != SearchDatabase.GetHashCode())
            {
                SearchDatabase = pdb;
            }
            if (searchOptions == null)
            {
                searchOptions = new SearchOptions();
            }


            SearchParameters sp = new SearchParameters();
            sp.ExcludeExpired = true;
            //Named for Save.
            sp.Name = $"Search: {searchString}";
            sp.ComparisonMode = StringComparison.InvariantCultureIgnoreCase;
            PwObjectList<PwEntry> pwl = new PwObjectList<PwEntry>();


            sp.SearchMode = searchOptions.Mode;
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Search Mode: {searchOptions.Mode}");

            switch (searchOptions.Mode)
            {
                case PwSearchMode.None:
                    break;
                case PwSearchMode.Simple:
                    break;
                case PwSearchMode.Regular:
                    break;
                case PwSearchMode.XPath:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#endif


            sp.SearchInTitles = false;
            sp.SearchInUserNames = false;
            sp.SearchInStringNames = false;
            sp.SearchInGroupNames = false;
            sp.SearchInGroupPaths = false;
            sp.SearchInHistory = false;
            sp.SearchInNotes = false;
            sp.SearchInPasswords = false;
            sp.SearchInTags = false;
            sp.SearchInUrls = false;
            sp.SearchInUuids = false;
            sp.SearchInOther = false;
            sp.SearchInUserNames = false;
            switch (searchOptions.Options)
            {
                case SearchParameterOptions.Title:
                    sp.SearchInTitles = true;
                    break;
                case SearchParameterOptions.Username:
                    sp.SearchInUserNames = true;
                    break;
                case SearchParameterOptions.ProtectedString:
                    sp.SearchInStringNames = true;
                    break;
                case SearchParameterOptions.GroupNames:
                    sp.SearchInGroupNames = true;
                    break;
                case SearchParameterOptions.GroupPaths:
                    sp.SearchInGroupPaths = true;
                    break;
                case SearchParameterOptions.History:
                    sp.SearchInHistory = true;
                    break;
                case SearchParameterOptions.Notes:
                    sp.SearchInNotes = true;
                    break;
                case SearchParameterOptions.Passwords:
                    sp.SearchInPasswords = true;
                    break;
                case SearchParameterOptions.Tags:
                    sp.SearchInTags = true;
                    break;
                case SearchParameterOptions.Urls:
                    sp.SearchInUrls = true;
                    break;
                case SearchParameterOptions.UniqueIdentifier:
                    sp.SearchInUuids = true;
                    break;
                case SearchParameterOptions.OtherDataField:
                    sp.SearchInOther = true;
                    break;
                case SearchParameterOptions.Default:
                default:
                    sp.SearchInTitles = true;
                    //sp.SearchInUserNames = true;
                    //sp.SearchInStringNames = true;
                    break;
            }

            sp.SearchString = searchString;

            pdb.RootGroup.SearchEntries(sp, pwl);
            PwObjectList<PwEntry> pwObjectExactMatch = pwl.CloneDeep();
            System.Diagnostics.Debug.WriteLine($"Password Object List: Count: {pwl.Count()}");
            foreach (PwEntry pwEntry in pwl)
            {
                System.Diagnostics.Debug.WriteLine($"Entry Found: {pwEntry.ToString()}");
                Debug.WriteLine($"Title: {pwEntry.Strings.Get("Title").ReadString()}");
                if (pwEntry.Strings.Get("Title").ReadString() != searchString)
                {
                    Debug.WriteLine($"Removing : {pwEntry.ToString()}");
                    PwEntry entryToRemove = pwObjectExactMatch.Single(entry => entry.Strings.Get("Title").ReadString() == pwEntry.Strings.Get("Title").ReadString());
                    if (!pwObjectExactMatch.Remove(entryToRemove))
                    {
                        if (searchOptions.ExactValueMatch)
                        {
                            throw new Exception("Unable to match and limit to one entry.");
                        }
                    }
                }

            }
            return pwObjectExactMatch;
        }

    }
    [Flags]
    public enum SearchParameterOptions
    {
        Default,
        Title,
        Username,
        GroupNames,
        ProtectedString,
        GroupPaths,
        History,
        Notes,
        Passwords,
        Tags,
        Urls,
        UniqueIdentifier,
        OtherDataField,

    }

    public class SearchOptions
    {
        /// <summary>
        /// this set to true will exclude all Expired Entries from the search for the parameter.
        /// </summary>
        public bool DoNotSearchInExpired { get; set; }
        /// <summary>
        /// This set to true will set <seealso cref="SearchParameters.RespectEntrySearchingDisabled"/> to true. <br/> In Other words <br/>
        /// <see cref="DoNotSearchSetExcludeFromSearch"/> == <seealso cref="SearchParameters.RespectEntrySearchingDisabled"/>
        /// </summary>
        public bool DoNotSearchSetExcludeFromSearch { get; set; }

        public bool ExactValueMatch { get; set; } = false;

        public SearchParameterOptions Options { get; set; } = SearchParameterOptions.Default;
        public PwSearchMode Mode { get; set; } = PwSearchMode.Regular;
    }
}
