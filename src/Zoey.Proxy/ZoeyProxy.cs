using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Zoey.Proxy.Abstractions;

namespace Zoey.Proxy
{
    public class ZoeyProxy : IZoeyProxy
    {
        private static readonly string CacheKey = $"{nameof(ZoeyProxy)}_CacheKey";
        private readonly ZoeyProxyOptions _configuration;
        private static readonly ConcurrentQueue<(string, int)> ExcludeIp = new ConcurrentQueue<(string, int)>();

        private static readonly object _lock = new object();


        public ZoeyProxy(IOptions<ZoeyProxyOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _configuration = options.Value;
        }

        /// <summary>
        /// 获取代理地址
        /// </summary>
        /// <returns></returns>
        public (string address, int port) GetIpProxy()
        {
            lock (_lock)
            {
                var ipList = GetAllIpProxy();
                if (ExcludeIp.Any())
                    ipList = ipList.Except(ExcludeIp).ToList();
                if (ipList.Count == 0)
                {
                    UpdateIp();
                    //BUG:如果依然没获取到会报错
                    ipList = GetAllIpProxy();
                }
                return ipList[new Random().Next(0, ipList.Count - 1)];
            }
        }

        /// <summary>
        /// 添加代理Ip为有效Ip
        /// </summary>
        /// <param name="address"></param>
        public void AddValidIp((string address, int port) address)
        {
            lock (_lock)
            {
                var file = $"{Environment.CurrentDirectory}/{_configuration.ProxyConfiguration.ValidIpFilePath}";
                File.WriteAllText(file, $"{address.address}:{address.port}");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 删除代理Ip
        /// </summary>
        /// <param name="address"></param>
        public void DeleteProxyIp((string, int) address)
        {
            ExcludeIp.Enqueue(address);
        }

        #region 私有方法
        /// <summary>
        /// 获取代理Ip
        /// </summary>
        async void UpdateIp()
        {
            var policyWrap = SetPolicy();
            await policyWrap.ExecuteAsync(() =>
            {
                var ipStr = _configuration.ProxyConfiguration.ResponseCallBack();
                var file = $"{Environment.CurrentDirectory}/{_configuration.ProxyConfiguration.IpFilePath}";
                File.WriteAllText(file, ipStr);
                _cacheManager.GetCache(_configuration.ProxyConfiguration.CacheName).Remove(CacheKey);
                Logger.Info($"更新Ip成功,时间:{DateTime.Now}");
                return Task.CompletedTask;
            });

            AsyncPolicyWrap SetPolicy()
            {
                var p1 = Policy
                    .Handle<WebException>()
                    .Or<HttpRequestException>()
                    .Or<TimeoutRejectedException>()
                    .Or<TimeoutException>()
                    .Or<DCrawlerException>()
                    .Or<TaskCanceledException>()
                    .WaitAndRetryAsync(_configuration.ProxyConfiguration.RequestFailRetryCount,
                        retryAttempt => TimeSpan.FromSeconds(1),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            Logger.Debug($"获取代理IP：第{retryCount}次获取代理Ip错误");
                        });
                var p2 = Policy.Handle<Exception>()
                    .FallbackAsync(cancellationToken =>
                    {
                        Logger.Error($"获取代理IP错误：{JsonConvert.SerializeObject(cancellationToken)}");
                        return Task.CompletedTask;
                    });
                var mixedPolicy = Policy.WrapAsync(p2, p1);
                return mixedPolicy;
            }
        }

        /// <summary>
        /// 从文件获取IP
        /// </summary>
        /// <returns></returns>
        List<(string address, int port)> GetAllIpProxy()
        {
            return _cacheManager.GetCache(_configuration.ProxyConfiguration.CacheName).Get(CacheKey, () =>
            {
                var result = new List<(string address, int port)>();
                var file = $"{Environment.CurrentDirectory}/{_configuration.ProxyConfiguration.IpFilePath}";
                if (!File.Exists(file))
                    return new List<(string, int)>();
                using (var sr = new StreamReader(file, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        result.Add((line.Split(':')[0], int.Parse(line.Split(':')[1])));
                    }
                }
                return result;
            });
        }
        #endregion
    }
}
