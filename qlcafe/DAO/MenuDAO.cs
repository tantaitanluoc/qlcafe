using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;

        public static MenuDAO Instance { get => instance == null ? new MenuDAO() : instance; private set => instance = value; }

        private MenuDAO() { }

        public List<Menu> GetListMenuByTableId(int id)
        {
            List<Menu> menus = new List<Menu>();
            DataTable dt = DataProvider.Instance.executeQuery($"select f.name, bi.count, f.price, (f.price * bi.count) as total " +
                $"from billinfo as bi, bill as b, Food as f where bi.idBill = b.id and bi.idFood = f.id and b.status = 0" +
                $"and b.idTable = { id} ");

            foreach(DataRow row in dt.Rows)
            {
                menus.Add(new Menu(row));
            }
            return menus;
        }
    }
}
