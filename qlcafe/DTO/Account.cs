using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace qlcafe.DTO
{
    public class Account
    {
        private string usname;
        private string dsplname;
        private string passwd;
        private int type;


        public Account(string usname, string dsplname, int type, string passwd = null)
        {
            this.Usname = usname;
            this.Dsplname = dsplname;
            this.Type = type;
            this.Passwd = passwd;
        }
        public Account(System.Data.DataRow row)
        {
            this.Usname = row["usname"].ToString();
            this.Dsplname = row["dplname"].ToString();
            this.Type = (int)row["type"];
            this.Passwd = row["passwd"].ToString();
        }
        public string Usname { get => usname; set => usname = value; }
        public string Dsplname { get => dsplname; set => dsplname = value; }
        public int Type { get => type; set => type = value; }
        public string Passwd { get => passwd; set => passwd = value; }
    }
}
