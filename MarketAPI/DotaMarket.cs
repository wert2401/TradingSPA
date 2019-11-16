using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TradingSite.Data;

namespace TradingSite.MarketAPI
{
    public static class DotaMarket
    {
        public static List<Item> GetItems(string rareTypes)
        {
            List<Item> items = new List<Item>();
            using (HttpClient cl = new HttpClient())
            {
                string request = "https://market.dota2.net/ajax/i_popularity/"+rareTypes+"//all/1";
                var response = cl.GetAsync(request).Result;
                string responseText = response.Content.ReadAsStringAsync().Result;
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
            }
            return items;
        }
    }
}
