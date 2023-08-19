using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;
        public static int TableWidth = 110;
        public static int TableHeight = 110;

        public static TableDAO Instance { get => instance == null ? new TableDAO() : instance; private set => instance = value; }
        private TableDAO() { }
        public List<Table> LoadTablesList()
        {
            List<Table> tblist = new List<Table>();
            DataTable dt = DataProvider.Instance.executeQuery("USP_GetTablesList;");
            foreach(DataRow item in dt.Rows)
            {
                tblist.Add(new Table(item));
            }
            return tblist;
        }
        public void SwitchTables(int id1, int id2)
        {
            if (id1 == id2)
                return;
            DataProvider.Instance.executeQuery("USP_SwitchTables @id1 , @id2", new object[] { id1, id2});

        }
    }
}
