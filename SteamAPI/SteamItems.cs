using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TradingSite.Data;
using TradingSite.Database;

namespace TradingSite.SteamAPI
{
    public class SteamItems
    {
        public List<Item> GetItemsWithPrice(List<Item> items, AppDatabaseContext context, List<HttpClient> clients)
        {
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
                Item dbItem = context.Items.Where(c => c.Name == item.Name && c.IdSecond == item.IdSecond).FirstOrDefault();
                if (dbItem != null)
                {
                    if ((DateTime.Now - dbItem.UpTime).TotalMinutes < 30)
                    {
                        dbItem.Price = item.Price;
                        responseItems.Add(dbItem);
                        itemsToRemove.Add(item);
                        context.Attach(dbItem);
                    }
                }
            }

            foreach (Item rmItem in itemsToRemove)
            {
                items.Remove(rmItem);
            }

            //Медленный способ запросов на стим площадку, обновление базы данных.
            int temp = items.Count / clients.Count();
            for (int i = 0; i < temp + 1; i++)
            {
                for (int j = i * clients.Count(); j < (i + 1) * clients.Count(); j++)
                {
                    if (j >= items.Count())
                    {
                        break;
                    }
                    Item curItem = items[j];
                    HttpClient curClient = clients[j - i * clients.Count()];
                    taskPool.Add(Task.Run(() => Steam.GetItemPrice(curClient, curItem)));
                }
                Task.WaitAll(taskPool.ToArray());
                for (int y = 0; y < taskPool.Count(); y++)
                {
                    Item res = taskPool[y].Result;
                    Console.WriteLine("Not Loaded: " + res.Name);
                    if (res.SteamPrice != -100)
                    {
                        Item dbItem = context.Items.Where(c => c.Name == res.Name && c.IdSecond == res.IdSecond).FirstOrDefault();
                        if (dbItem != null)
                        {
                            dbItem.SteamPrice = res.SteamPrice;
                            dbItem.UpTime = res.UpTime;
                            responseItems.Add(dbItem);
                            context.Attach(dbItem);
                        }
                        else
                        {
                            responseItems.Add(res);
                            context.Items.Update(res);
                        }
                    }
                }
                taskPool.Clear();
            }
            context.SaveChanges();
            Console.WriteLine("Items delivered: " + responseItems.Count().ToString());
            return responseItems;
        }
    }
}
