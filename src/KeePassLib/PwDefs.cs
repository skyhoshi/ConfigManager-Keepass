/*
  KeePass Password Safe - The Open-Source Password Manager
  Copyright (C) 2003-2021 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KeePassLib
{
	/// <summary>
	/// Contains KeePassLib-global definitions and enums.
	/// </summary>
	public static class PwDefs
	{
		/// <summary>
		/// The product name.
		/// </summary>
		public static readonly string ProductName = "KeePass Password Safe";

		/// <summary>
		/// A short, simple string representing the product name. The string
		/// should contain no spaces, directory separator characters, etc.
		/// </summary>
		public static readonly string ShortProductName = "KeePass";

		internal const string UnixName = "keepass2";
		internal const string ResClass = "KeePass2"; // With initial capital

		/// <summary>
		/// Version, encoded as 32-bit unsigned integer.
		/// 2.00 = 0x02000000, 2.01 = 0x02000100, ..., 2.18 = 0x02010800.
		/// As of 2.19, the version is encoded component-wise per byte,
		/// e.g. 2.19 = 0x02130000.
		/// It is highly recommended to use <c>FileVersion64</c> instead.
		/// </summary>
		public static readonly uint Version32 = 0x02310000;

		/// <summary>
		/// Version, encoded as 64-bit unsigned integer
		/// (component-wise, 16 bits per component).
		/// </summary>
		public static readonly ulong FileVersion64 = 0x0002003100000000UL;

		/// <summary>
		/// Version, encoded as string.
		/// </summary>
		public static readonly string VersionString = "2.49";

		public static readonly string Copyright = @"Copyright © 2003-2021 Dominik Reichl";

		/// <summary>
		/// Product website URL. Terminated by a forward slash.
		/// </summary>
		public static readonly string HomepageUrl = "https://keepass.info/";

		/// <summary>
		/// URL to the online translations page.
		/// </summary>
		public static readonly string TranslationsUrl = "https://keepass.info/translations.html";

		/// <summary>
		/// URL to the online plugins page.
		/// </summary>
		public static readonly string PluginsUrl = "https://keepass.info/plugins.html";

		/// <summary>
		/// Product donations URL.
		/// </summary>
		public static readonly string DonationsUrl = "https://keepass.info/donate.html";

		/// <summary>
		/// URL to the root path of the online KeePass help. Terminated by
		/// a forward slash.
		/// </summary>
		public static readonly string HelpUrl = "https://keepass.info/help/";

		/// <summary>
		/// URL to a TXT file (eventually compressed) that contains information
		/// about the latest KeePass version available on the website.
		/// </summary>
		public static readonly string VersionUrl = "https://www.dominik-reichl.de/update/version2x.txt.gz";

		/// <summary>
		/// A <c>DateTime</c> object that represents the time when the assembly
		/// was loaded.
		/// </summary>
		public static readonly DateTime DtDefaultNow = DateTime.UtcNow;

		/// <summary>
		/// Default number of master key encryption/transformation rounds
		/// (making dictionary attacks harder).
		/// </summary>
		public static readonly ulong DefaultKeyEncryptionRounds = 60000;

		/// <summary>
		/// Default identifier string for the title field.
		/// Should not contain spaces, tabs or other whitespace.
		/// </summary>
		public const string TitleField = "Title";
		// Const instead of static readonly for backward compatibility with plugins

		/// <summary>
		/// Default identifier string for the user name field.
		/// Should not contain spaces, tabs or other whitespace.
		/// </summary>
		public const string UserNameField = "UserName";
		// Const instead of static readonly for backward compatibility with plugins

		/// <summary>
		/// Default identifier string for the password field.
		/// Should not contain spaces, tabs or other whitespace.
		/// </summary>
		public const string PasswordField = "Password";
		// Const instead of static readonly for backward compatibility with plugins

		/// <summary>
		/// Default identifier string for the URL field.
		/// Should not contain spaces, tabs or other whitespace.
		/// </summary>
		public const string UrlField = "URL";
		// Const instead of static readonly for backward compatibility with plugins

		/// <summary>
		/// Default identifier string for the notes field.
		/// Should not contain spaces, tabs or other whitespace.
		/// </summary>
		public const string NotesField = "Notes";
		// Const instead of static readonly for backward compatibility with plugins

		/// <summary>
		/// Default identifier string for the field which will contain TAN indices.
		/// </summary>
		public static readonly string TanIndexField = UserNameField;

		/// <summary>
		/// Default title of an entry that is really a TAN entry.
		/// </summary>
		public static readonly string TanTitle = @"<TAN>";

		/// <summary>
		/// Prefix of a custom auto-type string field.
		/// </summary>
		public static readonly string AutoTypeStringPrefix = "S:";

		/// <summary>
		/// Default string representing a hidden password.
		/// </summary>
		public static readonly string HiddenPassword = "********";

		/// <summary>
		/// Default auto-type keystroke sequence. If no custom sequence is
		/// specified, this sequence is used.
		/// </summary>
		public static readonly string DefaultAutoTypeSequence = @"{USERNAME}{TAB}{PASSWORD}{ENTER}";

		/// <summary>
		/// Default auto-type keystroke sequence for TAN entries. If no custom
		/// sequence is specified, this sequence is used.
		/// </summary>
		public static readonly string DefaultAutoTypeSequenceTan = @"{PASSWORD}";

		/// <summary>
		/// Maximum time (in milliseconds) after which the user interface
		/// should be updated.
		/// </summary>
		internal const int UIUpdateDelay = 50;

		internal const uint QualityBitsWeak = 79;

		internal const string FavoriteTag = "Favorite";

		/// <summary>
		/// Check if a name is a standard field name.
		/// </summary>
		/// <param name="strFieldName">Input field name.</param>
		/// <returns>Returns <c>true</c>, if the field name is a standard
		/// field name (title, user name, password, ...), otherwise <c>false</c>.</returns>
		public static bool IsStandardField(string strFieldName)
		{
			Debug.Assert(strFieldName != null); if(strFieldName == null) return false;

			if(strFieldName.Equals(TitleField)) return true;
			if(strFieldName.Equals(UserNameField)) return true;
			if(strFieldName.Equals(PasswordField)) return true;
			if(strFieldName.Equals(UrlField)) return true;
			if(strFieldName.Equals(NotesField)) return true;

			return false;
		}

		public static List<string> GetStandardFields()
		{
			List<string> l = new List<string>();

			l.Add(TitleField);
			l.Add(UserNameField);
			l.Add(PasswordField);
			l.Add(UrlField);
			l.Add(NotesField);

			return l;
		}

		/// <summary>
		/// Check whether an entry is a TAN entry.
		/// </summary>
		public static bool IsTanEntry(PwEntry pe)
		{
			if(pe == null) { Debug.Assert(false); return false; }

			return (pe.Strings.ReadSafe(PwDefs.TitleField) == TanTitle);
		}

		internal static string GetTranslationDisplayVersion(string strFileVersion)
		{
			if(strFileVersion == null) { Debug.Assert(false); return string.Empty; }

			if(strFileVersion == "2.39") return "2.39.1 / 2.39";
			if(strFileVersion == "2.42") return "2.42.1 / 2.42";
			if(strFileVersion == "2.48") return "2.48.1 / 2.48";

			return strFileVersion;
		}

		internal static PwIcon GroupIconToEntryIcon(PwIcon i)
		{
			PwIcon r = i; // Inherit by default

			switch(i)
			{
				case PwIcon.Folder:
				case PwIcon.FolderOpen:
				case PwIcon.FolderPackage:
					Debug.Assert((new PwEntry(false, false)).IconId == PwIcon.Key);
					r = PwIcon.Key;
					break;

				case PwIcon.EMailBox:
					r = PwIcon.EMail;
					break;

				default: break;
			}

			return r;
		}
	}
}
