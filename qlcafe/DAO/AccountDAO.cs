using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance { get => instance == null ? new AccountDAO() : instance; private set => instance = value; }
        private AccountDAO() { }
        public bool Login(string usname, string passwd)
        {
            /*            string query = $"select * from Account where usname = '{usname}' and passwd = '{passwd}';";
                        DataTable result = DataProvider.Instance.executeQuery(query);
                        return result.Rows.Count > 0;*/
            string query = "exec USP_Login @usName , @passwd";
            object obj = DataProvider.Instance.executeScalar(query, new object[] { usname, passwd });
            return obj.ToString() == "1";
        }
        public Account GetAccountByUsername(string usname)
        {
            DataTable dt = DataProvider.Instance.executeQuery($"select * from account where usname = '{usname}'");
            foreach(DataRow row in dt.Rows)
            {
                return new Account(row);
            }
            return null;
        }
        public bool UpdateAccount(string usname, string dsplname, string passwd, string newpasswd)
        {
            return DataProvider.Instance.executeNonQuery("exec USP_UpdateAccount @usname , @dplname , @passwd , @newpasswd", new object[] { usname, dsplname, passwd, newpasswd }) > 0;
        }
        public DataTable GetAccounts()
        {
            return DataProvider.Instance.executeQuery("select usname as [tên đăng nhập], dplname as [tên hiển thị], type as [loại tài khoản] from Account");
        }
        public bool InsertAccount(string usname, string dsplname, int type)
        {
            return DataProvider.Instance.executeNonQuery($"insert into Account(usname, dplname, type) values(N'{usname}',N'{dsplname}',{type})") > 0;
        }
        public bool UpdateAccountInfo(string usname, string dsplname, int type)
        {
            return DataProvider.Instance.executeNonQuery($"update Account set dplname = N'{dsplname}', type = {type} where usname = N'{usname}'") > 0;
        }
        public bool DeleteAccount(string usname)
        {
            return DataProvider.Instance.executeNonQuery($"delete Account where usname = N'{usname}'") > 0;
        }
        public bool PasswordReset(string usname)
        {
            return DataProvider.Instance.executeNonQuery($"Update Account set passwd = N'0' where usname = N'{usname}'") > 0;
        }
    }
}
 