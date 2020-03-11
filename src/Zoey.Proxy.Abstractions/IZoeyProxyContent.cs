using System;
using System.Collections.Generic;
using System.Text;

namespace Zoey.Proxy.Abstractions
{
    public interface IZoeyProxyContent
    {
        /// <summary>
        /// 获取所有代理IP
        /// </summary>
        /// <returns></returns>
        List<(string address, int port)> GetAllIpProxy();
        /// <summary>
        /// 获取所有代理IP
        /// </summary>
        /// <returns></returns>
        List<(string address, int port)> GetAllIpProxy();
    }
}
