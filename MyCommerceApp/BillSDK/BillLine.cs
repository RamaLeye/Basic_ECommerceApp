using StockSDK;
using System;
using System.Collections.Generic;
using System.Text;


namespace BillSDK
{
    public class BillLine
    {
        ItemLine itemLine;
        double subtotal;


        public BillLine(ItemLine itemLine)
        {
            this.itemLine = itemLine;
            subtotal = itemLine.getQuantiy() * itemLine.getUnitPrice();
        }
        public BillLine(ItemLine itemLine, double subtotal)
        {
            this.itemLine  = itemLine;
            this.subtotal = subtotal;
        }

        public ItemLine getItemLine()
        {
            return itemLine;
        }


        public double getSubtotal()
        {
            return subtotal;
        }

        public override string ToString()
        {
            string line = itemLine.ToString() + "                 Subtotal = " + subtotal;
            return line;
        }

    }
}
