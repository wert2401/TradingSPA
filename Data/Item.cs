using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradingSite.Data
{
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
