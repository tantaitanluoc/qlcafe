using qlcafe.DAO;
using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace qlcafe
{
    public partial class admin_form : Form
    {
        private Account session;
        BindingSource foodbs = new BindingSource();
        BindingSource accountbs = new BindingSource();

        public Account Session { get => session; set => session = value; }

        public admin_form(Account session)
        {
            InitializeComponent();
            this.Session = session;
            Init();
        }
        private void Init()
        {
            dgvFood.DataSource = foodbs;
            dgvAccount.DataSource = accountbs;
            InitDateTime();
            GetFoodList();
            GetAccountList();
            DataBinding();
            LoadCategoryComboboxData(cbFoodCategory);
        }
        private void InitDateTime()
        {
            DateTime today = DateTime.Now;
            dtpFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpToDate.Value = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            LoadDataFromDateRange(dtpFromDate.Value, dtpToDate.Value);

        }
        private void LoadDataFromDateRange(DateTime from, DateTime to)
        {
            dgvBill.DataSource = BillDAO.Instance.GetListBillByDateRange(from, to);
        }

        private void GetFoodList()
        {
            foodbs.DataSource = FoodDAO.Instance.GetFoods();
        }
        private void LoadCategoryComboboxData(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetCategories();
            cb.DisplayMember = "name";
        }
        private void DataBinding()
        {
            tbFoodName.DataBindings.Add(new Binding("Text", dgvFood.DataSource, "tên món", true, DataSourceUpdateMode.Never));
            tbFoodID.DataBindings.Add(new Binding("Text", dgvFood.DataSource, "mã", true, DataSourceUpdateMode.Never));
            numFoodPrice.DataBindings.Add(new Binding("Value", dgvFood.DataSource, "giá", true, DataSourceUpdateMode.Never));

            tbAccountUsname.DataBindings.Add(new Binding("Text", dgvAccount.DataSource, "tên đăng nhập", true, DataSourceUpdateMode.Never));
            tbAccountDplname.DataBindings.Add(new Binding("Text", dgvAccount.DataSource, "tên hiển thị", true, DataSourceUpdateMode.Never));
            numAccountType.DataBindings.Add(new Binding("Value", dgvAccount.DataSource, "loại tài khoản", true, DataSourceUpdateMode.Never));
           

        }

        private void SearchFoods(string pattern)
        {
            foodbs.DataSource = FoodDAO.Instance.QueryByFoodNamePattern(pattern);
        }

        private void GetAccountList()
        {
            accountbs.DataSource = AccountDAO.Instance.GetAccounts();
        }
        private void AddAccount()
        {
            string usname = tbAccountUsname.Text;
            string dsplname = tbAccountDplname.Text;
            int type = (int)numAccountType.Value;
            if (AccountDAO.Instance.InsertAccount(usname, dsplname, type))
                MessageBox.Show($"Thêm tài khoản '{usname}' thành công", "Thông báo");
            else
                MessageBox.Show($"Không thể thêm tài khoản '{usname}'", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void UpdateAccountInfo()
        {
            string usname = tbAccountUsname.Text;
            string dsplname = tbAccountDplname.Text;
            int type = (int)numAccountType.Value;
            if (AccountDAO.Instance.UpdateAccountInfo(usname, dsplname, type))
                MessageBox.Show($"Cập nhật tài khoản '{usname}' thành công", "Thông báo");
            else
                MessageBox.Show($"Không thể cập nhật tài khoản '{usname}'", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void DeleteAccount()
        {
            string usname = tbAccountUsname.Text;
            if(session.Usname.Equals(usname))
            {
                MessageBox.Show($"Không thể xóa tài khoản '{usname}' đang đăng nhập với quyền quản trị", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(usname))
                MessageBox.Show($"Đã xóa tài khoản '{usname}'", "Thông báo");
            else
                MessageBox.Show($"Không thể xóa tài khoản '{usname}'", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ResetPassword()
        {
            string usname = tbAccountUsname.Text;
            if (AccountDAO.Instance.PasswordReset(usname))
                MessageBox.Show($"Đã đặt lại mật khẩu cho tài khoản '{usname}'", "Thông báo");
            else
                MessageBox.Show($"Không thể đặt lại mật khẩu tài khoản '{usname}'", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region Events
        private void btnViewFood_Click(object sender, EventArgs e)
        {
            GetFoodList();
        }
        private void btnView_Click(object sender, EventArgs e)
        {
            LoadDataFromDateRange(dtpFromDate.Value, dtpToDate.Value);
        }
        private void tbFoodID_TextChanged(object sender, EventArgs e)
        {
            int id = -1;
            if(dgvFood.SelectedCells[0].OwningRow.Cells["mã category"].Value != null)
            {
                id = (int)dgvFood.SelectedCells[0].OwningRow.Cells["mã category"].Value;
                cbFoodCategory.SelectedIndex = CategoryDAO.Instance.GetListCategoryIDs().IndexOf(id);
            }

        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = tbFoodName.Text;
            int idCategory = (cbFoodCategory.SelectedItem as Category).Id;
            float price = (float) numFoodPrice.Value;
            if (FoodDAO.Instance.InsertFood(name, idCategory, price))
            {
                MessageBox.Show($"Đã thêm món '{name}' vào danh sách thức ăn.");
                GetFoodList();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
                MessageBox.Show($"Có lỗi xảy ra, không thể thêm món '{name}'.");
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(tbFoodID.Text);
            string name = tbFoodName.Text;
            int idCategory = (cbFoodCategory.SelectedItem as Category).Id;
            float price = (float)numFoodPrice.Value;
            if (FoodDAO.Instance.UpdateFood(id, name, idCategory, price))
            {
                MessageBox.Show($"Cập nhật cho mã món '{id}' thành công.");
                GetFoodList();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
                MessageBox.Show($"Có lỗi xảy ra, không thể cập nhật cho món với mã '{id}'.");
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(tbFoodID.Text);
            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show($"Xóa món với mã '{id}' thành công.");
                GetFoodList();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
                MessageBox.Show($"Có lỗi xảy ra, không thể xóa món với mã '{id}'.");
        }

        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            SearchFoods(tbFoodSearchPattern.Text);
        }

        private void btnViewAccount_Click(object sender, EventArgs e)
        {
            GetAccountList();
        }



        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }
        private event EventHandler updateFood;
        public event EventHandler UpdatFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }
        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        #endregion

        private void tbAccountUsname_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            AddAccount();
            GetAccountList();
        }
        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
            GetAccountList();
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            DeleteAccount();
            GetAccountList();
        }

        private void btnAccountResetPasswd_Click(object sender, EventArgs e)
        {
            ResetPassword();
        }
    }
}
