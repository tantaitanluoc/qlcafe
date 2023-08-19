using qlcafe.DAO;
using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace qlcafe
{
    public partial class login_form : Form
    {
        public login_form()
        {
            InitializeComponent();
        }

        private void login_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát?","Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //string usname = tbUsname.Text, passwd = tbPasswd.Text;
            string usname = "admin", passwd = "admin"; // dev mode
            if (Authenticate(usname,passwd))
            {
                Account session = AccountDAO.Instance.GetAccountByUsername(usname);
                tablemangr_form f = new tablemangr_form(session);
                this.Hide();
                f.ShowDialog();
                this.Show();
            } else
            {
                MessageBox.Show("Thông tin đăng nhập không hợp lệ!");
            }
        }

        private bool Authenticate(string usname, string passwd)
        {
           // return true;
            return AccountDAO.Instance.Login(usname, passwd);
        }
    }
}
