using System;
using TradingSite.Data;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace TradingSite.SteamAPI
{
    public static class Steam
    {
        public static Item GetItemPrice(HttpClient client, Item item)
        {
            try
            {
                var response = client.GetAsync("https://steamcommunity.com/market/search/render/?norender=1&query=" + item.Name.Replace("'", "%27").Replace(" ", "%20")).Result;
                string respString = response.Content.ReadAsStringAsync().Result;
                string price = Regex.Match(respString, @"sell_price..(\d+),").Groups[1].Value;
                item.SteamPrice = double.Parse(price) / 100;
                return item;
            }
            catch (Exception e)
            {
                return item;
            }
        }
    }
}
