using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using toolVudaco.NCC;

namespace toolVudaco
{
    public partial class bangDieuKhien : Form
    {
        public bangDieuKhien()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           // DataAccess ac = new DataAccess();
           // var dt = ac.RunQuery("select * from DanhSachNhaCungCap");
            // Kiểm tra xem Form2 đã mở chưa
            nhaCungCap existingForm = Application.OpenForms.OfType<nhaCungCap>().FirstOrDefault();

            if (existingForm == null)
            {
                // Chưa mở, tạo mới
                nhaCungCap f2 = new nhaCungCap();
                f2.Show();
            }
            else
            {
                // Đã mở rồi → đưa lên trước
                existingForm.BringToFront();
            }
        }
    }
}
