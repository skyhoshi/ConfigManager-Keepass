using System;

namespace KeePassLib
{
    public sealed class ObjectTouchedEventArgs : EventArgs
    {
        private readonly object m_o;
        public object Object { get { return m_o; } }

        private readonly bool m_bModified;
        public bool Modified { get { return m_bModified; } }

        private readonly bool m_bParentsTouched;
        public bool ParentsTouched { get { return m_bParentsTouched; } }

        public ObjectTouchedEventArgs(object o, bool bModified,
            bool bParentsTouched)
        {
            m_o = o;
            m_bModified = bModified;
            m_bParentsTouched = bParentsTouched;
        }
    }
}