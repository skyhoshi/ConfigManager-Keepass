using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using KeePassLib.Delegates;

namespace KeePassLib
{
    // #pragma warning disable 1591 // Missing XML comments warning
    /// <summary>
    /// Search parameters for group and entry searches.
    /// </summary>
    public sealed class SearchParameters
    {
        private string m_strName = string.Empty;
        /// <summary>
        /// Identifies the Name of the Search Parameter object
        /// </summary>
        /// <remarks>This appears to be intended to allow you to Name Searches and save them for the future.</remarks>
        [DefaultValue("")]
        public string Name
        {
            get { return m_strName; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                m_strName = value;
            }
        }

        private string m_strText = string.Empty;
        [DefaultValue("")]
        public string SearchString
        {
            get { return m_strText; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                m_strText = value;
            }
        }

        private PwSearchMode m_sm = PwSearchMode.Simple;
        public PwSearchMode SearchMode
        {
            get { return m_sm; }
            set { m_sm = value; }
        }

        [DefaultValue(false)]
        [Obsolete]
        [XmlIgnore]
        public bool RegularExpression
        {
            get { return (m_sm == PwSearchMode.Regular); }
            set { m_sm = (value ? PwSearchMode.Regular : PwSearchMode.Simple); }
        }

        private bool m_bSearchInTitles = true;
        [DefaultValue(true)]
        public bool SearchInTitles
        {
            get { return m_bSearchInTitles; }
            set { m_bSearchInTitles = value; }
        }

        private bool m_bSearchInUserNames = true;
        [DefaultValue(true)]
        public bool SearchInUserNames
        {
            get { return m_bSearchInUserNames; }
            set { m_bSearchInUserNames = value; }
        }

        private bool m_bSearchInPasswords = false;
        [DefaultValue(false)]
        public bool SearchInPasswords
        {
            get { return m_bSearchInPasswords; }
            set { m_bSearchInPasswords = value; }
        }

        private bool m_bSearchInUrls = true;
        [DefaultValue(true)]
        public bool SearchInUrls
        {
            get { return m_bSearchInUrls; }
            set { m_bSearchInUrls = value; }
        }

        private bool m_bSearchInNotes = true;
        [DefaultValue(true)]
        public bool SearchInNotes
        {
            get { return m_bSearchInNotes; }
            set { m_bSearchInNotes = value; }
        }

        private bool m_bSearchInOther = true;
        [DefaultValue(true)]
        public bool SearchInOther
        {
            get { return m_bSearchInOther; }
            set { m_bSearchInOther = value; }
        }

        private bool m_bSearchInStringNames = false;
        [DefaultValue(false)]
        public bool SearchInStringNames
        {
            get { return m_bSearchInStringNames; }
            set { m_bSearchInStringNames = value; }
        }

        private bool m_bSearchInTags = true;
        [DefaultValue(true)]
        public bool SearchInTags
        {
            get { return m_bSearchInTags; }
            set { m_bSearchInTags = value; }
        }

        private bool m_bSearchInUuids = false;
        [DefaultValue(false)]
        public bool SearchInUuids
        {
            get { return m_bSearchInUuids; }
            set { m_bSearchInUuids = value; }
        }

        private bool m_bSearchInGroupPaths = false;
        [DefaultValue(false)]
        public bool SearchInGroupPaths
        {
            get { return m_bSearchInGroupPaths; }
            set { m_bSearchInGroupPaths = value; }
        }

        private bool m_bSearchInGroupNames = false;
        [DefaultValue(false)]
        public bool SearchInGroupNames
        {
            get { return m_bSearchInGroupNames; }
            set { m_bSearchInGroupNames = value; }
        }

        private bool m_bSearchInHistory = false;
        [DefaultValue(false)]
        public bool SearchInHistory
        {
            get { return m_bSearchInHistory; }
            set { m_bSearchInHistory = value; }
        }

#if KeePassUAP
		private StringComparison m_scType = StringComparison.OrdinalIgnoreCase;
#else
        private StringComparison m_scType = StringComparison.InvariantCultureIgnoreCase;
#endif
        /// <summary>
        /// String comparison type. Specifies the condition when the specified
        /// text matches a group/entry string.
        /// </summary>
        public StringComparison ComparisonMode
        {
            get { return m_scType; }
            set { m_scType = value; }
        }

        private bool m_bExcludeExpired = false;
        [DefaultValue(false)]
        public bool ExcludeExpired
        {
            get { return m_bExcludeExpired; }
            set { m_bExcludeExpired = value; }
        }

        private bool m_bRespectEntrySearchingDisabled = true;
        [DefaultValue(true)]
        public bool RespectEntrySearchingDisabled
        {
            get { return m_bRespectEntrySearchingDisabled; }
            set { m_bRespectEntrySearchingDisabled = value; }
        }

        private StrPwEntryDelegate m_fnDataTrf = null;
        [XmlIgnore]
        public StrPwEntryDelegate DataTransformationFn
        {
            get { return m_fnDataTrf; }
            set { m_fnDataTrf = value; }
        }

        private string m_strDataTrf = string.Empty;
        /// <summary>
        /// Only for serialization.
        /// </summary>
        [DefaultValue("")]
        public string DataTransformation
        {
            get { return m_strDataTrf; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                m_strDataTrf = value;
            }
        }

        [XmlIgnore]
        public static SearchParameters None
        {
            get
            {
                SearchParameters sp = new SearchParameters();

                Debug.Assert(sp.m_strName.Length == 0);
                Debug.Assert(sp.m_strText.Length == 0);
                Debug.Assert(sp.m_sm == PwSearchMode.Simple);
                sp.m_bSearchInTitles = false;
                sp.m_bSearchInUserNames = false;
                Debug.Assert(!sp.m_bSearchInPasswords);
                sp.m_bSearchInUrls = false;
                sp.m_bSearchInNotes = false;
                sp.m_bSearchInOther = false;
                Debug.Assert(!sp.m_bSearchInStringNames);
                sp.m_bSearchInTags = false;
                Debug.Assert(!sp.m_bSearchInUuids);
                Debug.Assert(!sp.m_bSearchInGroupPaths);
                Debug.Assert(!sp.m_bSearchInGroupNames);
                Debug.Assert(!sp.m_bSearchInHistory);
                // Debug.Assert(sp.m_scType == StringComparison.InvariantCultureIgnoreCase);
                Debug.Assert(!sp.m_bExcludeExpired);
                Debug.Assert(sp.m_bRespectEntrySearchingDisabled);

                return sp;
            }
        }

        /// <summary>
        /// Construct a new search parameters object.
        /// </summary>
        public SearchParameters()
        {
        }

        public SearchParameters Clone()
        {
            return (SearchParameters)this.MemberwiseClone();
        }
    }
    // #pragma warning restore 1591 // Missing XML comments warning
}