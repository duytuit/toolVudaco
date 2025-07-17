using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace toolVudaco.NCC
{
    public partial class congNoChiTiet : UserControl
    {
        public congNoChiTiet()
        {
            InitializeComponent();
            this.Dock = DockStyle.Top; // hoặc Fill nếu cần
        }

        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void congNoChiTiet_Load(object sender, EventArgs e)
        {
             DataAccess ac = new DataAccess();
             var dt = ac.RunQuery("SELECT MaNhaCungCap, TenNhaCungCap FROM DanhSachNhaCungCap");
             multiColumnComboBox1.PlaceholderText = "Tìm mã hoặc tên ncc...";
             multiColumnComboBox1.SetData(dt);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(multiColumnComboBox1.SelectedValue);
        }
    }
}
