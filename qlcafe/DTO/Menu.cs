using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DTO
{
    public class Menu
    {
        private string name;
        private int count;
        private float price;
        private float totalPrice;

        public Menu(string name, int count, float price, float totalPrice = 0)
        {
            this.Name = name;
            this.Count = count;
            this.Price = price;
            this.TotalPrice = totalPrice;
        }
        public Menu(DataRow row)
        {
            this.Name = (string)row["name"];
            this.Count = (int)row["count"];
            this.Price = (float)Convert.ToDouble(row["price"].ToString());
            this.TotalPrice = (float)Convert.ToDouble(row["total"]);
        }
        public string Name { get => name; set => name = value; }
        public int Count { get => count; set => count = value; }
        public float Price { get => price; set => price = value; }
        public float TotalPrice { get => totalPrice; set => totalPrice = value; }
    }
}
