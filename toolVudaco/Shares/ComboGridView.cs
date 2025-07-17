using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace toolVudaco.Shares
{
    public partial class ComboGridView : UserControl
    {
        private TextBox txtSearch;
        private DataGridView dgvResult;
        private string placeholderText = "Nhập để tìm kiếm...";
        private bool isPlaceholderActive = true;
        private DataTable originalData = new DataTable();

        public string ValueMember { get; set; }
        public string DisplayMember { get; set; }
        public string SelectedValue { get; private set; }
        public string SelectedText { get; private set; }
        public DataRow SelectedItem { get; private set; }

        public event EventHandler SelectedValueChanged;

        [Category("Custom")]
        public string PlaceholderText
        {
            get => placeholderText;
            set
            {
                placeholderText = value;
                if (isPlaceholderActive)
                    SetPlaceholder();
            }
        }

        public ComboGridView()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.Height = 30;

            txtSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Height = 25
            };
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyDown += TxtSearch_KeyDown;

            this.Controls.Add(txtSearch);

            dgvResult = new DataGridView
            {
                Visible = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false // Ẩn cột chọn
            };
            dgvResult.CellClick += DgvResult_CellClick;

            SetPlaceholder();
        }

        public void SetDataSource(DataTable dt)
        {
            originalData = dt;
            dgvResult.DataSource = dt;

            ValueMember = GetBestMatchColumn(dt, "id") ?? dt.Columns[0].ColumnName;
            DisplayMember = GetBestMatchColumn(dt, "name") ??
                            (dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName);
        }

        private string GetBestMatchColumn(DataTable dt, string keyword)
        {
            return dt.Columns.Cast<DataColumn>()
                     .FirstOrDefault(c => c.ColumnName.ToLower().Contains(keyword))?.ColumnName;
        }

        private void SetPlaceholder()
        {
            isPlaceholderActive = true;
            txtSearch.Text = placeholderText;
            txtSearch.ForeColor = Color.Gray;
        }

        private void TxtSearch_GotFocus(object sender, EventArgs e)
        {
            if (isPlaceholderActive)
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
                isPlaceholderActive = false;
            }
        }

        private void TxtSearch_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                SetPlaceholder();

            Timer t = new Timer();
            t.Interval = 200;
            t.Tick += (s, ev) =>
            {
                if (!dgvResult.Focused)
                    HideDropdown();
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (isPlaceholderActive || originalData == null) return;

            string keyword = txtSearch.Text.Trim().Replace("'", "''");

            if (string.IsNullOrEmpty(keyword))
            {
                HideDropdown();
                return;
            }

            string filter = string.Join(" OR ", originalData.Columns.Cast<DataColumn>()
                .Select(c => $"[{c.ColumnName}] LIKE '%{keyword}%'"));

            DataRow[] rows = originalData.Select(filter);

            if (rows.Any())
            {
                dgvResult.DataSource = rows.CopyToDataTable();
                ShowDropdown();
            }
            else
            {
                HideDropdown();
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && dgvResult.Visible && dgvResult.Rows.Count > 0)
            {
                dgvResult.Focus();
                dgvResult.CurrentCell = dgvResult.Rows[0].Cells[0];
            }
        }

        private void DgvResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var rowView = dgvResult.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;

            SelectedItem = rowView.Row;

            SelectedText = DisplayMember != null && SelectedItem.Table.Columns.Contains(DisplayMember)
                ? SelectedItem[DisplayMember].ToString()
                : SelectedItem[0].ToString();

            SelectedValue = ValueMember != null && SelectedItem.Table.Columns.Contains(ValueMember)
                ? SelectedItem[ValueMember].ToString()
                : SelectedItem[0].ToString();

            txtSearch.Text = SelectedText;
            txtSearch.ForeColor = Color.Black;
            isPlaceholderActive = false;

            HideDropdown();

            SelectedValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ShowDropdown()
        {
            if (this.TopLevelControl == null) return;

            if (dgvResult.Parent != this.TopLevelControl)
            {
                this.TopLevelControl.Controls.Add(dgvResult);
                dgvResult.BringToFront();
            }

            Point screenPos = txtSearch.PointToScreen(Point.Empty);
            Point parentPos = this.TopLevelControl.PointToClient(screenPos);

            dgvResult.Location = new Point(parentPos.X, parentPos.Y + txtSearch.Height);
            dgvResult.Width = this.Width;
            dgvResult.Height = 150;
            dgvResult.Visible = true;
        }

        private void HideDropdown()
        {
            dgvResult.Visible = false;
        }
    }
}
