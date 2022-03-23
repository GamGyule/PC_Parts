using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compuzoneWPF
{
    public class Product
    {
        
        public Product()
        {

        }

        public Product(string name, string info, int price, int pid, string co)
        {
            this.Name = name;
            this.Info = info;
            this.Price = price;
            this.Pid = pid;
            this.Co = co;
        }

        public string Name
        {
            get;
            set;
        }

        public string Info
        {
            get;
            set;
        }

        public int Price
        {
            get;
            set;
        }

        public int Pid
        {
            get;
            set;
        }

        public string Co
        {
            get;
            set;
        }

        public static List<Product> GetProducts()
        {
            DBManager db = new DBManager();
            List<Product> products = new List<Product>();

            products = db.ProductShow();

            if(products.Count <= 0)
            {
                products.Add(new Product("해당 제품이 없습니다", "해당 제품이 없습니다", 0,1541,"null"));
            }
            return products;
        }


    }
}
