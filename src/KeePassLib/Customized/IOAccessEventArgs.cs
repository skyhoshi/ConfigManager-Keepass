using System;
using KeePassLib.Serialization;

namespace KeePassLib
{
    public sealed class IOAccessEventArgs : EventArgs
    {
        private readonly IOConnectionInfo m_ioc;
        public IOConnectionInfo IOConnectionInfo { get { return m_ioc; } }

        private readonly IOConnectionInfo m_ioc2;
        public IOConnectionInfo IOConnectionInfo2 { get { return m_ioc2; } }

        private readonly IOAccessType m_t;
        public IOAccessType Type { get { return m_t; } }

        public IOAccessEventArgs(IOConnectionInfo ioc, IOConnectionInfo ioc2,
            IOAccessType t)
        {
            m_ioc = ioc;
            m_ioc2 = ioc2;
            m_t = t;
        }
    }
}