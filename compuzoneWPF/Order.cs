using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compuzoneWPF
{
    public class Order
    {
        public Order()
        {

        }

        public Order(string name, int price, int count, int pid, int result)
        {
            this.Name = name;
            this.Price = price;
            this.Count = count;
            this.Pid = pid;
            this.Result = price;
        }

        public string Name
        {
            get;
            set;
        }

        public int Price
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public int Pid
        {
            get;
            set;
        }

        public int Result
        {
            get;
            set;
        }
    }
}
