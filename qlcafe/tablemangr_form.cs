using qlcafe.DAO;
using qlcafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace qlcafe
{
    public partial class tablemangr_form : Form
    {
        private Account session;

        public Account Session { get => session; set => session = value; }

        public tablemangr_form(Account session)
        {
            InitializeComponent();
            this.Session = session;
            ChangeView(session.Type);
            // chuẩn hóa đơn vị tiền tệ
            CultureInfo culture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentCulture = culture;
            LoadCbTables();
            LoadCategory();
            Reset();
        }

        #region Methods
        private void LoadCbTables()
        {
            cbSwitchTb.DataSource = TableDAO.Instance.LoadTablesList();
            cbSwitchTb.DisplayMember = "Name";
        }

        private void LoadTables()
        {
            flpTable.Controls.Clear();
            List<Table> tblist = TableDAO.Instance.LoadTablesList();
            foreach(Table tb in tblist)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = $"{tb.Name + Environment.NewLine}({tb.Status})";
                btn.Tag = tb;
                btn.Click += Btn_Click;
                if (tb.Status != "Trống")
                    btn.BackColor = Color.Gold;

                flpTable.Controls.Add(btn);
            }

        }
        private void LoadCategory()
        {
            cbFoodCategory.DataSource = CategoryDAO.Instance.GetCategories();
            cbFoodCategory.DisplayMember = "Name"; // trường sẽ hiển thị lên combobox
        }
        private void LoadFoodListByCategoryID(int id)
        {
            cbFood.DataSource = FoodDAO.Instance.GetFoodListByCatergoryID(id);
            cbFood.DisplayMember = "Name";
        }
        private void ShowBill(int id)
        {
            ltvBill.Items.Clear();
            float totalPrice = 0;
            List<DTO.Menu> menus = MenuDAO.Instance.GetListMenuByTableId(id);
            foreach(DTO.Menu menu in menus)
            {
                ListViewItem lvitem = new ListViewItem(menu.Name.ToString());
                lvitem.SubItems.Add(menu.Count.ToString());
                lvitem.SubItems.Add(menu.Price.ToString());
                lvitem.SubItems.Add(menu.TotalPrice.ToString());
                ltvBill.Items.Add(lvitem);
                totalPrice += menu.TotalPrice;
            }
            tbTotalPrice.Text = totalPrice.ToString("c");
            tbTotalPrice.Tag = totalPrice;
            
        }
        private void Reset()
        {
            LoadTables();
            Table tb = ltvBill.Tag as Table;
            if (tb != null)
                ShowBill(tb.Id);
            numDiscount.Value = 0;
            numFoodCount.Value = 1;
            cbSwitchTb.SelectedIndex = 0;
            cbFood.SelectedIndex = 0;
            cbFoodCategory.SelectedIndex = 0;
            numDiscount.Tag = 0.0f;
            tbTotalPrice.Tag = 0;
        }
        private void ChangeView(int account_type)
        {
            adminToolStripMenuItem.Enabled = account_type == 1;
            đăngXuấtToolStripMenuItem.Text += $" ({session.Dsplname})";
        }

        private void FinalCheck()
        {
            float oP = (float)tbTotalPrice.Tag;
            float nP = oP - (oP / 100) * (int)numDiscount.Value;
            tbTotalPrice.Text = nP.ToString("c");
            numDiscount.Tag = nP;
        }

        #endregion

        #region Events

        private void Btn_Click(object sender, EventArgs e)
        {
            ShowBill(((sender as Button).Tag as Table).Id);
            ltvBill.Tag = (sender as Button).Tag;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Table table = ltvBill.Tag as Table;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn!", "Lỗi");
                return;
            }
            int idBill = BillDAO.Instance.GetUncheckedBillIdByTableId(table.Id);
            if(idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.Id);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetLatestBillId(), (cbFood.SelectedItem as Food).Id, (int)numFoodCount.Value);

            } else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, (cbFood.SelectedItem as Food).Id, (int)numFoodCount.Value);
                
            }
            ShowBill(table.Id);
            LoadTables();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e) // chuyển bàn click
        {
            Table selectedTB = ltvBill.Tag as Table;
            if(MessageBox.Show($"Chuyển '{selectedTB.Name}' qua '{(cbSwitchTb.SelectedItem as Table).Name}'?", "Xác nhận", MessageBoxButtons.OKCancel) == DialogResult.OK)
                TableDAO.Instance.SwitchTables(selectedTB.Id, (cbSwitchTb.SelectedItem as Table).Id);
            ShowBill(selectedTB.Id);
            LoadTables();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            profile_form f = new profile_form(session);
            f.UpdateAccountEvt += F_UpdateAccountEvt;
            f.ShowDialog();
        }

        private void F_UpdateAccountEvt(object sender, AccountEvents e)
        {
            đăngXuấtToolStripMenuItem.Text = $"Đăng xuất ({e.Session.Dsplname})";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            admin_form f = new admin_form(session);
            f.InsertFood += F_InsertFood;
            f.UpdatFood += F_UpdatFood;
            f.DeleteFood += F_DeleteFood;
            f.ShowDialog();
        }

        private void F_DeleteFood(object sender, EventArgs e)
        {
            Table tb = ltvBill.Tag as Table;
            if (tb != null)
                ShowBill(tb.Id);
            LoadFoodListByCategoryID((cbFoodCategory.SelectedItem as Category).Id);
        }

        private void F_UpdatFood(object sender, EventArgs e)
        {
            Table tb = ltvBill.Tag as Table;
            if (tb != null)
                ShowBill(tb.Id);
            LoadFoodListByCategoryID((cbFoodCategory.SelectedItem as Category).Id);
        }

        private void F_InsertFood(object sender, EventArgs e)
        {
            Table tb = ltvBill.Tag as Table;
            if (tb != null)
                ShowBill(tb.Id);
            LoadFoodListByCategoryID((cbFoodCategory.SelectedItem as Category).Id);
        }

        private void cbFoodCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            id = (cb.SelectedItem as Category).Id;
            LoadFoodListByCategoryID(id);
        }


        private void btnCheckout_Click(object sender, EventArgs e)
        {
            Table table = ltvBill.Tag as Table;
            int billId = BillDAO.Instance.GetUncheckedBillIdByTableId(table.Id);
            float totalPrice = 0, paid = 0;
            FinalCheck();
            totalPrice = (float)tbTotalPrice.Tag;
            paid = (float)numDiscount.Tag;

            if (billId != -1)
            {
                if (MessageBox.Show($"Xác nhận thanh toán cho {table.Name}","Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(billId, (int)numDiscount.Value, totalPrice, paid);
                    ShowBill(table.Id);
                }
            }
            Reset();
        }


        

        private void btnDiscount_Click(object sender, EventArgs e)
        {
            FinalCheck();
        }

        private void tảiLạiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }
        #endregion
    }
}
