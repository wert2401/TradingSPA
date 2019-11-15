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
using Microsoft.EntityFrameworkCore;
using TradingSite.Database;

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

        private AppDatabaseContext _context;
        public ItemsController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Item> Get()
        {
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

                #region FastWay
                //int index = 0;
                //while (items.Count > 0)
                //{
                //    while (taskPool.Count <= 10)
                //    {
                //        Item curItem = items[0];
                //        HttpClient tempClient = clients[index];
                //        taskPool.Add(Task.Run(() => Steam.GetItemPrice(tempClient, curItem))); //Почему нельзя передават сразу параметры, а только через переменные-посредники?
                //        items.Remove(curItem);
                //        index++;
                //        if (index >= Proxies.Count())
                //        {
                //            index = 0;
                //        }
                //    }
                //    int taskIndex = Task.WaitAny(taskPool.ToArray());
                //    Item item = taskPool[taskIndex].Result;
                //    taskPool.Remove(taskPool[taskIndex]);
                //    responseItems.Add(item);
                //}
                #endregion

                //Поиск по базе данных, и если Айтем нашелся и он был обновлен меньше чем 30 минут назад, то добавляем его в список возвращяемых Айтемов
                List<Item> itemsToRemove = new List<Item>();
                foreach (Item item in items)
                {
                    Item dbItem = _context.Items.Where(c => c.Name == item.Name && c.IdSecond == item.IdSecond).FirstOrDefault();
                    if (dbItem != null)
                    {
                        if ((DateTime.Now - dbItem.UpTime).TotalMinutes < 30)
                        {
                            responseItems.Add(dbItem);
                            itemsToRemove.Add(item);
                            Console.WriteLine("1");
                        }
                        item.Id = dbItem.Id;
                    }
                }
                foreach (Item rmItem in itemsToRemove)
                {
                    items.Remove(rmItem);
                }
                //Медленный способ запросов на стим площадку, обновление базы данных.
                Console.WriteLine("\nSearch Count: " + items.Count);
                int temp = items.Count / Proxies.Count();
                for (int i = 0; i < temp + 1; i++)
                {
                    for (int j = i * Proxies.Count(); j < (i + 1) * Proxies.Count(); j++)
                    {
                        if (j >= items.Count())
                        {
                            break;
                        }
                        Item curItem = items[j];
                        HttpClient curClient = clients[j - i * Proxies.Count()];
                        taskPool.Add(Task.Run(() => Steam.GetItemPrice(curClient, curItem)));
                    }
                    Task.WaitAll(taskPool.ToArray());
                    for (int y = 0; y < taskPool.Count(); y++)
                    {
                        Item res = taskPool[y].Result;
                        Console.WriteLine(res.Name + " : " + res.SteamPrice);
                        if (res.SteamPrice != -100)
                        {
                            responseItems.Add(res);
                            //Item dbItem = _context.Items.Where(c => c.Name == res.Name).FirstOrDefault();
                            //if (dbItem != null)
                            //{
                            //    dbItem.SteamPrice = res.SteamPrice;
                            //    dbItem.UpTime = res.UpTime;
                            //    _context.Attach(dbItem);

                            //}
                            //else
                            //{
                                _context.Items.Update(res);
                            //}
                        }
                    }
                    taskPool.Clear();
                }

                //Диспоузим все клиенты
                foreach (HttpClient client in clients)
                {
                    client.Dispose();
                }
                _context.SaveChanges();
                Console.WriteLine("Response Items: " + responseItems.Count() + "\n");
                return responseItems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}