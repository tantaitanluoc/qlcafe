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
    public partial class profile_form : Form
    {
        private Account session;
        public profile_form(Account session)
        {
            InitializeComponent();
            this.Session = session;
            ChangeView();
        }

        public Account Session { get => session; set => session = value; }

        private void profile_form_Load(object sender, EventArgs e)
        {

        }
        private void ChangeView()
        {
            tbUsname.Text = session.Usname;
            tbDplname.Text = session.Dsplname;
        
        }

        private void UpdateAccount()
        {
            string dsplname = tbDplname.Text;
            string passwd = tbPasswd.Text;
            string newpasswd = tbNewPasswd.Text;
            string reennewpasswd = tbReEnNewPasswd.Text;
            if (!newpasswd.Equals(reennewpasswd))
                MessageBox.Show("Mật khẩu mới nhập lại không khớp", "Lỗi", MessageBoxButtons.OK);
            else
            {
                if (AccountDAO.Instance.UpdateAccount(session.Usname, dsplname, passwd, newpasswd))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    if (updateAccountEvt != null)
                        updateAccountEvt(this, new AccountEvents(AccountDAO.Instance.GetAccountByUsername(session.Usname)));
                }
                else
                    MessageBox.Show("Không thể cập nhật! Vui lòng kiểm tra lại thông tin.");

            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccount();
        }

        private event EventHandler<AccountEvents> updateAccountEvt;
        public event EventHandler<AccountEvents> UpdateAccountEvt
        {
            add { updateAccountEvt += value; }
            remove { updateAccountEvt -= value; }
        }
    }
    public class AccountEvents : EventArgs
    {
        private Account session;
        public AccountEvents(Account session)
        {
            this.Session = session;
        }

        public Account Session { get => session; set => session = value; }
    }
}
