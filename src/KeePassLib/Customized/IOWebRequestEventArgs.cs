using System;
using System.Net;
using KeePassLib.Serialization;

namespace KeePassLib
{
    public sealed class IOWebRequestEventArgs : EventArgs
    {
        private readonly WebRequest m_wr;
        public WebRequest Request { get { return m_wr; } }

        private readonly IOConnectionInfo m_ioc;
        public IOConnectionInfo IOConnectionInfo { get { return m_ioc; } }

        public IOWebRequestEventArgs(WebRequest r, IOConnectionInfo ioc)
        {
            m_wr = r;
            m_ioc = ioc;
        }
    }
}