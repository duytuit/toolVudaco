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
        private BindingSource bs = new BindingSource();
        private DataTable fullData;
        private int currentPage = 1;
        private int pageSize = 10;
        DataAccess ac = new DataAccess();
        public congNoChiTiet()
        {
            InitializeComponent();
            this.Dock = DockStyle.Top; // hoặc Fill nếu cần
                                       // Tạo bảng mẫu
            fullData = GetSampleTable();
            bs.DataSource = Paginate(fullData, currentPage, pageSize);

            // AdvancedDataGridView
            advancedDataGridView1.DataSource = bs;
            advancedDataGridView1.AutoGenerateColumns = true;
            advancedDataGridView1.FilterAndSortEnabled = true;
            advancedDataGridView1.Dock = DockStyle.Fill;
            advancedDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            advancedDataGridView1.SortStringChanged += AdvancedDataGridView1_SortStringChanged;
            advancedDataGridView1.FilterStringChanged += AdvancedDataGridView1_FilterStringChanged;

            // Nút chuyển trang
           // buttonNext.Click += (s, e) => { currentPage++; LoadPage(); };
           // buttonPrev.Click += (s, e) => { currentPage = Math.Max(1, currentPage - 1); LoadPage(); };
        }
        private void LoadPage()
        {
            bs.DataSource = Paginate(fullData, currentPage, pageSize);
        }

        private DataTable Paginate(DataTable dt, int page, int size)
        {
            var rows = dt.AsEnumerable().Skip((page - 1) * size).Take(size);
            return rows.Any() ? rows.CopyToDataTable() : dt.Clone(); // Trả về bảng rỗng nếu không còn dòng
        }

        private void AdvancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        {
            bs.Filter = advancedDataGridView1.FilterString;
        }

        private void AdvancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            bs.Sort = advancedDataGridView1.SortString;
        }

        private DataTable GetSampleTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STT");
            dt.Columns.Add("Ngày hạch toán");
            dt.Columns.Add("Loại xe NCC");
            dt.Columns.Add("Mã điều xe");
            dt.Columns.Add("Số tờ khai");
            dt.Columns.Add("Số bill");
            dt.Columns.Add("Nội dung");
            dt.Columns.Add("Số tiền", typeof(decimal));
            dt.Columns.Add("VAT", typeof(decimal));
            dt.Columns.Add("Tổng cộng", typeof(decimal));
            dt.Columns.Add("Chi hộ");
            dt.Columns.Add("Ứng trước");
            dt.Columns.Add("Biển số xe");
            dt.Columns.Add("Số cont");

            var ncc = ac.RunQuery("SELECT MaNhaCungCap, TenNhaCungCap FROM DanhSachNhaCungCap");

            // Thêm dữ liệu mẫu
            for (int i = 1; i <= 100; i++)
            {
                dt.Rows.Add(i, DateTime.Today.AddDays(-i).ToShortDateString(), "Container", $"MDX{i}",
                    $"TK{i}", $"BILL{i}", $"Nội dung {i}", 100000 + i, 10000 + i, 110000 + i,
                    "Có", "Không", $"51C-{i:0000}", $"CONT{i}");
            }

            return dt;
        }
        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void congNoChiTiet_Load(object sender, EventArgs e)
        {
           
            var dt = ac.RunQuery("SELECT MaNhaCungCap, TenNhaCungCap FROM DanhSachNhaCungCap");

            comboGridView1.PlaceholderText = "Tìm mã hoặc tên nhà cung cấp...";
            comboGridView1.SetDataSource(dt); // Không cần set DisplayMember/ValueMember
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetSampleTable();
            //if (comboGridView1.SelectedValue != null)
            //{
            //    MessageBox.Show($"Mã: {comboGridView1.SelectedValue} - Tên: {comboGridView1.SelectedText}");
            //}
            //else
            //{
            //    MessageBox.Show("Chưa chọn dòng nào.");
            //}
        }

    }
}
