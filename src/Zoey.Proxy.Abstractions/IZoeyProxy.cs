using System;
using System.Collections.Generic;
using System.Text;

namespace Zoey.Proxy.Abstractions
{
    /// <summary>
    /// 设置代理
    /// </summary>
    public interface IZoeyProxy
    {
        /// <summary>
        /// 获取代理地址
        /// </summary>
        /// <returns></returns>
        (string address, int port) GetIpProxy();

        /// <summary>
        /// 删除代理Ip
        /// </summary>
        /// <param name="address"></param>
        void DeleteProxyIp((string, int) address);
        /// <summary>
        /// 添加代理Ip为有效Ip
        /// </summary>
        /// <param name="address"></param>
        void AddValidIp((string address, int port) address);
    }
}
