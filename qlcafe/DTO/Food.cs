using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DTO
{
    public class Food
    {
        private int id;
        private string name;
        private int categoryId;
        private float price;
        public Food(int id, string name, int categoryId, float price = 0)
        {
            this.Id = id;
            this.Name = name;
            this.CategoryId = categoryId;
            this.Price = price;
        }
        public Food(DataRow row)
        {
            this.Id = (int)row["id"];
            this.Name = row["name"].ToString();
            this.CategoryId = (int)row["idCategory"];
            this.Price = (float)Convert.ToDouble(row["price"].ToString());
        }
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int CategoryId { get => categoryId; set => categoryId = value; }
        public float Price { get => price; set => price = value; }
    }
}
