using Newtonsoft.Json;
using System;

namespace StockSDK
{
    public class Item
    {
        [JsonProperty("nom")]
        string name;
        [JsonProperty("prixHT")]
        float unitPrice;

        public Item(string name, float unitPrice)
        {
            this.name = name;
            this.unitPrice = unitPrice;
        }

        public string getName()
        {
            return name;
        }

        public float getUnitPrice()
        {
            return unitPrice;
        }

        public override string ToString()
        {
            string profil = name + "      " + unitPrice;
            return base.ToString();
        }
    }
}
