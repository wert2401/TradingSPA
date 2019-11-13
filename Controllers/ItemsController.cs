using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using TradingSite.Data;
using TradingSite.SteamAPI;
using TradingSite.MarketAPI;

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
        public List<Item> Get()
        {
            Random rnd = new Random();
            try
            {
                //Получение листа предметов с одной страницы с маркета
                List<Item> items = DotaMarket.GetItems();
                
                //Создание разных клиентов с разными прокси серверами
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

                //Получение цены каждого предмета с торговой площадки
                List<Item> responseItems = new List<Item>();
                List<Task<Item>> taskPool = new List<Task<Item>>();
                int index = 0;
                while (items.Count > 0)
                {
                    while (taskPool.Count <= 10)
                    {
                        Item curItem = items[0];
                        HttpClient tempClient = clients[index];
                        taskPool.Add(Task.Run(() => Steam.GetItemPrice(tempClient, curItem))); //Почему нельзя передават сразу параметры, а только через переменные-посредники?
                        items.Remove(curItem);
                        index++;
                        if (index >= Proxies.Count())
                        {
                            index = 0;
                        }
                    }
                    int taskIndex = Task.WaitAny(taskPool.ToArray());
                    Item item = taskPool[taskIndex].Result;
                    taskPool.Remove(taskPool[taskIndex]);
                    responseItems.Add(item);
                }

                foreach (HttpClient client in clients)
                {
                    client.Dispose();
                }

                return responseItems;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}