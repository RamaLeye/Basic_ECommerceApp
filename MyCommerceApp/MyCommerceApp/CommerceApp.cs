using BillSDK;
using RPC;
using StockSDK;
using System;
using System.Collections.Generic;
using UserSDK;

namespace MyCommerceApp
{
    class CommerceApp
    {
        List<ItemLine> shoppingCart = new List<ItemLine>();
        List<BillLine> billLines = new List<BillLine>();
        Bill bill ;
        User user;
        RpcUser userClient = new RpcUser();
        RcpStock stockClient = new RcpStock();


        static void Main(string[] args)
        {
            new CommerceApp();
        }


        public CommerceApp()
        {

            int choixMenu = 0;
            string numArticle = "";
            string quantite = "";
            user = new User();
            Authentification(user);
            getProducts(); 

            do
            {              
                switch (choixMenu)
                {
                    case 1:
                        choiceArticle(numArticle, quantite);
                        break;
                    case 2: DisplayCart();
                        break;
                    case 3: validateCart(user);
                        break;
                    case 4:
                        getProducts();
                        break;
                    case 5: Quit();
                        break;
                    default:
                        break;
                }
               
                menu();
                Console.WriteLine("\n Menu choice : ");
                choixMenu = int.Parse(Console.ReadLine());            
            } while (true);
        }


        public  void menu()
        {
            Console.WriteLine("\n [*] ______________ Menu ________________");
            Console.WriteLine(" 1. Choose an article ");
            Console.WriteLine(" 2. Display my cart ");
            Console.WriteLine(" 3. Get my bill ");
            Console.WriteLine(" 4. Display products ");
            Console.WriteLine(" 5. Quit ");
        }


        public  void Authentification(User user)
        {
            do
            {
                Console.WriteLine(" \n [*] _____________ Please enter a correct username. ______________ [*] ");
                string username = "";
                Console.WriteLine("[*] __________________Enter your username : ");
                username = Console.ReadLine();
                user = userClient.CallUser(username);

            } while (user.getUserName().Equals(""));

            Console.WriteLine(" \n [*] _________________ Bienvenue " + user.getFirstName() + " " + user.getLastName() + ".");
        }


        public  void choiceArticle(string numArticle, string quantite)
        {
            do
            {
                Console.WriteLine(" \n [*] ______________ Article number : ");
                numArticle = Console.ReadLine();
                Console.WriteLine(" \n [*] ______________ Quantity :  ");
                quantite = Console.ReadLine();
            } while (int.Parse(numArticle) < 0 || int.Parse(quantite) == 0);

           shoppingCart.Add(stockClient.CallItemLine(numArticle + " " + quantite));
 
        }


        public  void getProducts()
        {
            RcpStock stockClient = new RcpStock();
            string produits = stockClient.CallStock("products");
            Console.WriteLine("\n "  + produits);
        }



        public void validateCart(User user)
        {                    
            bill = new Bill(user, shoppingCart);
            Console.WriteLine(bill.ToString());

        }

        public  void DisplayCart()
        {
            Console.WriteLine("\n [*] _____________Your shopping cart : ");
            foreach ( ItemLine item in shoppingCart)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public  void Quit()
        {
            Console.WriteLine(" \n [*] _______________ Thank you bye _____________. ");
            userClient.Close();
            stockClient.Close();
            Environment.Exit(0);
        }




    }
}
