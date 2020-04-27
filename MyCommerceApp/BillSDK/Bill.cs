using StockSDK;
using System;
using System.Collections.Generic;
using UserSDK;

namespace BillSDK
{
    public class Bill
    {
        User user;
        List<BillLine> billLines;
        double subtotal_HT;
        double total_T;
        const double TPS = 5;
        const double TVQ = 9.975;

        public void calculateTotal()
        {
            foreach (BillLine line in billLines)
            {
                subtotal_HT += line.getSubtotal();
            }

            Math.Round(subtotal_HT);
            total_T = subtotal_HT + Math.Round((subtotal_HT * (TPS / 100)), 2) + Math.Round((subtotal_HT * (TVQ / 100)), 2);
        }

        public Bill(User u, List<ItemLine> itemLines)
        {
            billLines = new List<BillLine>();
            foreach (ItemLine item in itemLines)
            {
                billLines.Add(new BillLine(item));
            }
            this.user = u;
            calculateTotal();
        }


        public Bill(User user, List<BillLine> billLines)
        {
            this.user = user;
            this.billLines = billLines;
            subtotal_HT = 0;
            calculateTotal();
        }


        public Bill(User user, List<BillLine> billLines, float subtotal, float total)
        {
            this.user = user;
            this.billLines = billLines;
            subtotal_HT = subtotal;
            total_T = total;
        }


        public static Bill CreateBill(User u, List<ItemLine> lines)
        {
            return new Bill(u, lines);
        }

        public User getUser()
        {
            return user;
        }

        public List<BillLine> getBillLines()
        {
            return billLines;
        }

        public double getSubtotal_HT()
        {
            return subtotal_HT;
        }

        public double getTotal_T()
        {
            return total_T;
        }

        public override string ToString()
        {
            string bill = "\n Facture : " + user.getFirstName() + "  " + user.getLastName() + "\n";
            foreach(BillLine line in billLines)
            {
                bill += line.ToString() + "\n";
            }
            bill += "Subtotal W/O Taxes : " + subtotal_HT +"\n";
            bill += "Total + Taxes : " + total_T + "\n";

            return bill;
        }
    }
}

