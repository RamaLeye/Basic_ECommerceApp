using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockSDK
{
    [Serializable]
    public class ItemLine
    {
        [JsonProperty("name")]
        string name;
        [JsonProperty("PU")]
        double unitPrice;
        [JsonProperty("quantite")]
        int quantity { get; set; }

       public ItemLine(string name, double prix, int quantity)
        {
            this.name = name;
            this.unitPrice = prix;

            if(quantity > 0)
            {
                this.quantity = quantity;
            }
            else
            {
                this.quantity = 0 ;
            }         
        }



        public ItemLine()
        {
            name = "";
            unitPrice = 0;
            quantity = 0;
        }

        public string getName()
        {
            return name;
        }

        public double getUnitPrice()
        {
            return unitPrice;
        }

        public int getQuantiy()
        {
            return quantity;
        }

        public void setQuantity(int quantity)
        {
            this.quantity = quantity;
        }

        public override string ToString()
        {
            string profil = String.Format("|{0,30}|{1,10}|{2,10}|", name, unitPrice, quantity); 
            return profil;
        }
    }
}
