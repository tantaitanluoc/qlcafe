using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance { get => instance == null ? new BillDAO() : instance; private set => instance = value; }
        private BillDAO() { }
        public int GetUncheckedBillIdByTableId(int tableId)
        {
            DataTable dt = DataProvider.Instance.executeQuery($"select * from Bill where idTable = ${tableId} and status = 0");
            if (dt.Rows.Count > 0)
            {
                //Bill bill = new Bill(dt.Rows[0]);
                return (int)dt.Rows[0]["id"];
            }
            else return -1;
            
        }
        public bool InsertBill(int id)
        {
            return DataProvider.Instance.executeNonQuery("USP_InsertBill @idTable", new object[] { id }) > 0;
        }
        public int GetLatestBillId()
        {
            try
            {
                return (int)DataProvider.Instance.executeScalar("select MAX(id) from Bill;");
            }
            catch
            {
                return 1;
            }
        }
        public bool CheckOut(int id, int discount, float totalPrice, float paid)
        {
           return DataProvider.Instance.executeNonQuery($"update Bill set status = 1, datecheckout = GETDATE()," +
               $" discount = {discount}, totalPrice = {totalPrice}, paid = {paid} " +
               $"where id = {id}") > 0;
        }
        public DataTable GetListBillByDateRange(DateTime from, DateTime to)
        {
            return DataProvider.Instance.executeQuery("USP_GetListBillByDateRange @from , @to", new object[] { from, to });
        }
    }
}
