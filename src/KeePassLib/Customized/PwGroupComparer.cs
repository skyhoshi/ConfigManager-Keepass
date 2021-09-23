using System.Collections.Generic;
using KeePassLib.Utility;

namespace KeePassLib
{
    public sealed class PwGroupComparer : IComparer<PwGroup>
    {
        public PwGroupComparer()
        {
        }

        public int Compare(PwGroup a, PwGroup b)
        {
            return StrUtil.CompareNaturally(a.Name, b.Name);
        }
    }
}