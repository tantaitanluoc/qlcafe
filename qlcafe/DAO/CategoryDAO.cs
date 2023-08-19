using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;

        public static CategoryDAO Instance { get => instance == null ? new CategoryDAO() : instance; private set => instance = value; }
        private CategoryDAO() { }    
        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
            DataTable dt = DataProvider.Instance.executeQuery("select * from FoodCategory;");
            foreach(DataRow row in dt.Rows)
            {
                categories.Add(new Category(row));
            }
            return categories; 
        }
/*        public Category GetCategoryById(int id)
        {
            Category c = null;

            DataTable dt = DataProvider.Instance.executeQuery($"select * from FoodCategory where id = {id}");
            foreach (DataRow row in dt.Rows)
            {
                c = new Category(row);
            }
            return c;
        }*/
        public List<int> GetListCategoryIDs()
        {
            List<int> l = new List<int>();
            DataTable dt = DataProvider.Instance.executeQuery($"select id from FoodCategory");
            foreach (DataRow row in dt.Rows)
            {
                l.Add((int)row["id"]);
            }
            return l;
        }
    }
}
