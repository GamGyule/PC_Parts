using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compuzoneWPF
{
    class AdminProduct
    {
        public AdminProduct(string name, string info, int price, string img, int count, int index)
        {
            Name = name;
            Info = info;
            Price = price;
            Img = img;
            Count = count;
            Index = index;
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

        public string Img
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }
    }
}
