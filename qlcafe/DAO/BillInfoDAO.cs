using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    public class BillInfoDAO
    {
        private static BillInfoDAO instance;

        public static BillInfoDAO Instance { get => instance == null ? new BillInfoDAO() : instance; private set => instance = value; }
        private BillInfoDAO() { }

        public List<BillInfo> GetListBillInfo(int id)
        {
            List<BillInfo> billInfoList = new List<BillInfo>();
            DataTable dt = DataProvider.Instance.executeQuery($"select * from billinfo where idBill = {id}");
            foreach(DataRow row in dt.Rows)
            {
                BillInfo billinfo = new BillInfo(row);
                billInfoList.Add(billinfo);
            }
            return billInfoList;
        }
        public bool InsertBillInfo(int idBill, int idFood, int count)
        {
            return DataProvider.Instance.executeNonQuery("USP_InsertBillInfo @idBill , @idFood , @count", new object[] { idBill, idFood, count }) > 0;
        }
    }
}
