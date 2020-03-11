using System;
using System.Collections.Generic;
using System.Text;

namespace Zoey.Proxy
{
    public class ZoeyProxyOptions
    {
        /// <summary>
        /// 默认存放Ip文件路径
        /// </summary>
        private const string DefaultIpFilePath = "/ProxyIp/ProxyIp.txt";
        /// <summary>
        /// 默认存放有效Ip文件路径
        /// </summary>
        private const string DefaultValidIpFilePath = "/ProxyIp/ValidProxyIp.txt";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isOneChange">是否每次请求换一次Ip</param>
        /// <param name="ipFilePath">存放Ip的文件路径 默认在根目录的ProxyIp文件夹下</param>
        /// <param name="responseCallBack">获取代理ip方法</param>
        public ZoeyProxyOptions(bool isOneChange, string ipFilePath,
            Func<string> responseCallBack, int requestFailRetryCount)
        {
            IsOneChange = isOneChange;
            IpFilePath = ipFilePath;
            ResponseCallBack = responseCallBack;
            CacheName = nameof(ZoeyProxyOptions);
            RequestFailRetryCount = requestFailRetryCount;
        }

        public ZoeyProxyOptions() : this(false, DefaultIpFilePath, null, 5)
        {
        }
        public ZoeyProxyOptions(bool isOneChange, Func<string> responseCallBack)
            : this(isOneChange, DefaultIpFilePath, responseCallBack, 5)
        {
        }

        public ZoeyProxyOptions(string ipFilePath, Func<string> responseCallBack)
            : this(true, ipFilePath, responseCallBack, 5)
        {

        }

        public ZoeyProxyOptions(Func<string> responseCallBack)
            : this(true, DefaultIpFilePath, responseCallBack, 5)
        {

        }

        /// <summary>
        /// 是否每次请求换一次Ip
        /// 默认:false
        /// </summary>
        public bool IsOneChange { get; private set; }
        /// <summary>
        /// 存放Ip的文件路径
        /// 默认在根目录的ProxyIp文件夹下
        /// </summary>
        public string IpFilePath { get; private set; }
        /// <summary>
        /// 存放有效Ip文件路径
        /// 默认在根目录的ValidProxyIp文件夹下
        /// </summary>
        public string ValidIpFilePath { get; }
        /// <summary>
        /// 获取代理ip方法
        /// </summary>
        public Func<string> ResponseCallBack { get; private set; }

        /// <summary>
        /// 缓存名称
        /// </summary>
        public string CacheName { get; private set; }

        /// <summary>
        /// 错误重试次数
        /// </summary>
        public int RequestFailRetryCount { get; set; }
    }
}
