using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace qlcafe.DTO
{
    public class Bill
    {
        private int id;
        private DateTime? datecheckin;
        private DateTime? datecheckout;
        private int status;
        private int discount;

        public Bill(int  id, DateTime? datecheckin, DateTime datecheckout, int discount = 0)
        {
            this.Datecheckin = datecheckin;
            this.Datecheckout = datecheckout;
            this.Id = id;
            this.Status = status;
            this.Discount = discount;
        }
        public Bill(DataRow row)
        {
            this.Datecheckin = (DateTime?)row["datecheckin"];
            if(row["datecheckout"].ToString() != "")
                this.Datecheckout = (DateTime?)row["datecheckout"];
            this.Id = (int)row["id"];
            this.Status = (int)row["status"];
            this.Discount = (int)row["discount"];
        }

        public int Id { get => id; set => id = value; }
        public DateTime? Datecheckin { get => datecheckin; set => datecheckin = value; }
        public DateTime? Datecheckout { get => datecheckout; set => datecheckout = value; }
        public int Status { get => status; set => status = value; }
        public int Discount { get => discount; set => discount = value; }
    }
}
