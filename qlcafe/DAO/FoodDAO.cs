using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance { get => instance == null ? new FoodDAO() : instance; private set => instance = value; }
        private FoodDAO() { }
        public List<Food> GetFoodListByCatergoryID(int id)
        {
            List<Food> food = new List<Food>();
            DataTable dt = DataProvider.Instance.executeQuery($"select * from Food where idCategory = {id} and onservice = 1");
            foreach (DataRow row in dt.Rows)
            {
                food.Add(new Food(row));
            }
            return food;
        }
        public DataTable GetFoods()
        {
            //List<Food> foods = new List<Food>();
            DataTable dt = DataProvider.Instance.executeQuery($"select Food.id as [mã], Food.name as [tên món], Food.idCategory as [mã category], FoodCategory.name as [category], Food.price as [giá] " +
                $"from Food, FoodCategory " +
                $"where Food.idCategory = FoodCategory.id and Food.onservice = 1");
            /* foreach (DataRow row in dt.Rows)
             {
                 foods.Add(new Food(row));
             }
             return foods;*/
            return dt;
        }
        public bool InsertFood(string name, int idCategory, float price)
        {
            return DataProvider.Instance.executeNonQuery($"insert into Food(name, idCategory, price) values(N'{name}',{idCategory},{price})") > 0;
        }
        public bool UpdateFood(int id, string name, int idCategory, float price)
        {
            return DataProvider.Instance.executeNonQuery($"update Food set name = N'{name}', idCategory = {idCategory}, price = {price} where id = {id}") > 0;
        }
        public bool DeleteFood(int id)
        {
            return DataProvider.Instance.executeNonQuery($"update Food set onservice = 0 where id = {id}") > 0;
        }

        public DataTable QueryByFoodNamePattern(string pattern)
        {
            return DataProvider.Instance.executeQuery($"select Food.id as [mã], Food.name as [tên món], Food.idCategory as [mã category], FoodCategory.name as [category], Food.price as [giá] " +
                $"from Food, FoodCategory " +
                $"where Food.idCategory = FoodCategory.id and Food.onservice = 1 " +
                $"and [dbo].[fuConvertToUnsign1](Food.name) like [dbo].[fuConvertToUnsign1](N'%{pattern}%')");
        }

    }
}
