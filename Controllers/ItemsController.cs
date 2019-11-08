using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace TradingSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        //https://steamcommunity.com/market/search/render/?norender=1&query=ItemName
        //https://market.dota2.net/api/v2/prices/class_instance/RUB.json
        //https://market.dota2.net/ajax/i_popularity/common//all/1
        public static string[] Proxies { get; private set; } =
        {
            "118.174.46.162:8080",
            "36.67.80.19:8080",
            "212.17.19.19:8080",
            "188.133.165.206:56145"
        };



        [HttpGet]
        public async Task<List<Item>> Get()
        {
            Random rnd = new Random();
            try
            {
                List<Item> items = new List<Item>();
                using (HttpClient cl = new HttpClient())
                {
                    var response = await cl.GetAsync("https://market.dota2.net/ajax/i_popularity/common//all/1");
                    string responseText = await response.Content.ReadAsStringAsync();
                    responseText = Regex.Replace(responseText, @"\[*,\d+]", "").Substring(1).Replace(",[]", "");
                    List<string[]> strings = JsonConvert.DeserializeObject<List<string[]>>(responseText);
                    foreach (string[] stringObj in strings)
                    {
                        Item item = new Item
                        {
                            IdFirst = stringObj[0],
                            IdSecond = stringObj[1],
                            Name = stringObj[2],
                            Price = double.Parse(stringObj[3].Replace(".", ",")) / 62,
                            IsGood = bool.Parse(stringObj[5])
                        };
                        items.Add(item);
                    }

                    List<HttpClient> clients = new List<HttpClient>();

                    foreach (string ip in Proxies)
                    {
                        WebProxy wp = new WebProxy(ip);
                        HttpClientHandler handler = new HttpClientHandler()
                        {
                            UseProxy = true,
                            Proxy = wp
                        };
                        clients.Add(new HttpClient(handler));
                    }

                    //Почему нельзя передават сразу параметры, а только через переменные-посредники?
                    List<Task<double>> pool = new List<Task<double>>();
                    int countOfTasks = clients.Count;
                    int temp = (items.Count / countOfTasks);
                    for (int i = 0; i <= temp; i++)
                    {
                        for (int j = i * countOfTasks; j < countOfTasks * (i + 1); j++)
                        {
                            if (j >= items.Count)
                            {
                                break;
                            }
                            int index = j - (i * countOfTasks);
                            Item it = items[j];
                            pool.Add(Task.Run(() => GetItemPrice(clients[index], it)));
                        }

                        Task.WaitAll(pool.ToArray());

                        for (int j = 0; j < pool.Count; j++)
                        {
                            items[(i * countOfTasks) + j].SteamPrice = pool[j].Result;
                        }

                        pool.Clear();
                        //Console.WriteLine($"-----------------------------------------------------------");
                    }
                    foreach (HttpClient client in clients)
                    {
                        client.Dispose();
                    }
                }
                return items;
            }
            catch (Exception e)
            {
                return new List<Item> { new Item { Name = e.Message } };
                throw;
            }
        }

        private double GetItemPrice(HttpClient client, Item item)
        {
            try
            {
                var response = client.GetAsync("https://steamcommunity.com/market/search/render/?norender=1&query=" + item.Name.Replace("'", "%27").Replace(" ", "%20")).Result;
                string respString = response.Content.ReadAsStringAsync().Result;
                string price = Regex.Match(respString, @"sell_price..(\d+),").Groups[1].Value;
                //Console.WriteLine("Thread ID:" + Thread.GetCurrentProcessorId());
                //Console.WriteLine("Name:" + item.Name);
                //Console.WriteLine("Price:" + price);
                //Console.WriteLine("Response" + respString + "\n");
                return double.Parse(price) / 100;
                //return -10;
            }
            catch (Exception e)
            {
                //Console.WriteLine("Error:" + e.Message + "\n");
                return -1;
            }
        }
    }

    public class Item
    {
        public string IdFirst { get; set; }
        public string IdSecond { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsGood { get; set; }
        public double SteamPrice { get; set; } = -100;
    }
}

//    public class MyResponse
//    {
//        public bool Success { get; set; }
//        public int Time { get; set; }
//        public string Currency { get; set; }
//        public Dictionary<string, Item> Items { get; set; }

//        public void SortItems()
//        {
//            List<string> keysToRemove = new List<string>();

//            foreach (KeyValuePair<string, Item> item in Items)
//            {
//                if (Convert.ToDouble(item.Value.Price) > 500)
//                {
//                    keysToRemove.Add(item.Key);
//                }

//            }

//            foreach (string key in keysToRemove)
//            {
//                Items.Remove(key);
//            }
//        }
//    }

//    public class MyRequest
//    {
//        public Dictionary<string, Item> Items { get; set; }
//    }

//    public class Item
//    {
//        public string Price { get; set; }
//        public string Buy_order { get; set; }
//        public string Avg_price { get; set; }
//        public string Popularity_7d { get; set; }
//        public string Market_hash_name { get; set; }
//    }
//}
