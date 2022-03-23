using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compuzoneWPF
{
    class AdminOrder
    {
        public AdminOrder(int index, string name, string phone, string date, int pid, int count, int price)
        {
            Index = index;
            Name = name;
            Phone = phone;
            Date = date;
            Pid = pid;
            Count = count;
            Price = price;
        }

        public int Index
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public string Date
        {
            get;
            set;
        }

        public int Pid
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public int Price
        {
            get;
            set;
        }
    }
}
