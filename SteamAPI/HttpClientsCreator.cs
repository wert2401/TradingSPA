using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TradingSite.SteamAPI
{
    public class HttpClientsCreator
    {
        private string[] Proxies;
        public HttpClientsCreator()
        {
            Proxies = new string []
            {
                "118.174.46.162:8080",
                "36.67.80.19:8080",
                "212.17.19.19:8080",
                "188.133.165.206:56145"
            };
        }

        public List<HttpClient> GetHttpClient()
        {
            List<HttpClient> clients = new List<HttpClient>();
            foreach (string ip in Proxies)
            {
                WebProxy wp = new WebProxy(ip);
                HttpClientHandler handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = wp
                };
                HttpClient hc = new HttpClient(handler);
                hc.Timeout = TimeSpan.FromSeconds(5);
                clients.Add(hc);
            }
            return clients;
        }
    }
}
