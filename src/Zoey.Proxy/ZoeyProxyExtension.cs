using System;
using System.Net;
using System.Net.Http;
using Zoey.Proxy.Abstractions;

namespace Zoey.Proxy
{
    public static class ZoeyProxyExtension
    {
        public static (string address, int port) SetProxy(this IZoeyProxy proxy, HttpClientHandler clientHandler, bool isOneChange)
        {
            var ipProxy = proxy.GetIpProxy();
            clientHandler.Proxy = new WebProxy(new Uri($"http://{ipProxy.address}:{ipProxy.port}"));
            clientHandler.UseProxy = true;
            if(isOneChange)
                proxy.DeleteProxyIp(ipProxy);
            return ipProxy;
        }
    }
}
