using System;

namespace DCrawler
{
    /// <summary>
    /// 异常基类
    /// </summary>
    public class ZoeyProxyException : Exception
    {
        public ZoeyProxyException()
        {
        }

        public ZoeyProxyException(string message) : base(message)
        {
        }

        public ZoeyProxyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
