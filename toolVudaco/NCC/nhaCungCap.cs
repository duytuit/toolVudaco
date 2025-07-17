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
    public partial class nhaCungCap : Form
    {
        public nhaCungCap()
        {
            InitializeComponent();
            this.Dock = DockStyle.Top; // hoặc Fill nếu cần
        }

        private void nhaCungCap_Load(object sender, EventArgs e)
        {
            congNoChiTiet1.Dock = DockStyle.Fill;
        }
    }
}
