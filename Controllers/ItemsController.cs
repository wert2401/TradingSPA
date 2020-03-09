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

        private AppDatabaseContext _context;
        public ItemsController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("{types}")]
        public List<Item> Get(string types)
        {
            try
            {
                //Получение листа предметов с одной страницы с маркета
                List<Item> items = DotaMarket.GetItems(types);

                //Создание разных клиентов с разными прокси серверами\
                HttpClientsCreator hcc = new HttpClientsCreator();
                List<HttpClient> htppClients = hcc.GetHttpClient();

                //Получением листа Айтемов с ценами со стима
                SteamItems si = new SteamItems();
                List<Item> responseItems = si.GetItemsWithPrice(items, _context, htppClients);

                //Диспоузим все клиенты
                foreach (HttpClient client in htppClients)
                {
                    client.Dispose();
                }

                return responseItems;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}