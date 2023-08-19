using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace qlcafe.DTO
{
    public class Table
    {
        private int id;
        private string name;
        private string status;
        public Table(int id, string name, string status)
        {
            this.Id = id;
            this.Name = name;
            this.Status = status;
        }
        public Table(DataRow row)
        {
            this.Id = (int)row["id"];
            this.Status = row["status"].ToString();
            this.Name = row["name"].ToString();
        }
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Status { get => status; set => status = value; }

        public static implicit operator Table(ListView v)
        {
            throw new NotImplementedException();
        }
    }
}
