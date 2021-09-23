using KeePassLib.Interfaces;

namespace KeePassLib
{
    // #pragma warning disable 1591 // Missing XML comments warning
    /// <summary>
    /// Memory protection configuration structure (for default fields).
    /// </summary>
    public sealed class MemoryProtectionConfig : IDeepCloneable<MemoryProtectionConfig>
    {
        public bool ProtectTitle = false;
        public bool ProtectUserName = false;
        public bool ProtectPassword = true;
        public bool ProtectUrl = false;
        public bool ProtectNotes = false;

        // public bool AutoEnableVisualHiding = false;

        public MemoryProtectionConfig CloneDeep()
        {
            return (MemoryProtectionConfig)this.MemberwiseClone();
        }

        public bool GetProtection(string strField)
        {
            if(strField == PwDefs.TitleField) return this.ProtectTitle;
            if(strField == PwDefs.UserNameField) return this.ProtectUserName;
            if(strField == PwDefs.PasswordField) return this.ProtectPassword;
            if(strField == PwDefs.UrlField) return this.ProtectUrl;
            if(strField == PwDefs.NotesField) return this.ProtectNotes;

            return false;
        }
    }
    // #pragma warning restore 1591 // Missing XML comments warning
}