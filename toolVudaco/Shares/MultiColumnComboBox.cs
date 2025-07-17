using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace toolVudaco.Shares
{
    public partial class MultiColumnComboBox : UserControl
    {
        private DataTable data = new DataTable();
        private DropDownForm dropdown;
        private string placeholderText = "Nhập để tìm kiếm...";
        private bool isPlaceholderActive = true;
        private bool isFormClosingSubscribed = false;

        public string SelectedValue { get; private set; }
        public string SelectedText { get; private set; }
        public DataRow SelectedItem { get; private set; }

        public string ValueMember { get; private set; }
        public string DisplayMember { get; private set; }

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

        public MultiColumnComboBox()
        {
            InitializeComponent();
            InitControl();
        }

        private void InitControl()
        {
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyDown += TxtSearch_KeyDown;
            txtSearch.KeyPress += TxtSearch_KeyPress;

            SetPlaceholder();

            dropdown = new DropDownForm();
            dropdown.DataGridView.CellClick += DataGridView_CellClick;
            dropdown.DataGridView.KeyDown += Dropdown_KeyDown;
        }

        public void SetData(DataTable dt)
        {
            data = dt;
            dropdown.DataGridView.DataSource = dt;

            // Tự động dò cột
            ValueMember = GetBestMatchColumn(dt, "id") ?? dt.Columns[0].ColumnName;
            DisplayMember = GetBestMatchColumn(dt, "name") ??
                            (dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName);
        }

        private string GetBestMatchColumn(DataTable dt, string keyword)
        {
            return dt.Columns.Cast<DataColumn>()
                     .FirstOrDefault(c => c.ColumnName.ToLower().Contains(keyword))?.ColumnName;
        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isPlaceholderActive)
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
                isPlaceholderActive = false;

                txtSearch.Text = e.KeyChar.ToString();
                txtSearch.SelectionStart = txtSearch.Text.Length;
                e.Handled = true;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (isPlaceholderActive || data == null) return;

            string filter = txtSearch.Text.Trim().Replace("'", "''");

            if (string.IsNullOrEmpty(filter))
            {
                dropdown.Hide();
                SetPlaceholder();
                return;
            }

            int cursorPos = txtSearch.SelectionStart;

            string expression = string.Join(" OR ",
                data.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}] LIKE '%{filter}%'"));
            DataRow[] rows = data.Select(expression);

            if (rows.Length > 0)
            {
                dropdown.DataGridView.DataSource = rows.CopyToDataTable();
                ShowDropdown();
            }
            else
            {
                dropdown.Hide();
            }

            txtSearch.Focus();
            txtSearch.SelectionStart = cursorPos;
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
            {
                SetPlaceholder();
                dropdown.Hide();
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && dropdown.Visible)
            {
                dropdown.DataGridView.Focus();
                if (dropdown.DataGridView.Rows.Count > 0)
                    dropdown.DataGridView.CurrentCell = dropdown.DataGridView.Rows[0].Cells[0];
            }
        }

        private void Dropdown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dropdown.DataGridView.CurrentRow != null)
            {
                int rowIndex = dropdown.DataGridView.CurrentRow.Index;
                DataGridView_CellClick(dropdown.DataGridView, new DataGridViewCellEventArgs(0, rowIndex));
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                dropdown.Hide();
            }
        }

        private void SetPlaceholder()
        {
            isPlaceholderActive = true;
            txtSearch.Text = placeholderText;
            txtSearch.ForeColor = Color.Gray;
            txtSearch.SelectionStart = 0;
        }

        private void ShowDropdown()
        {
            if (!dropdown.Visible)
            {
                var location = txtSearch.PointToScreen(Point.Empty);
                dropdown.Size = new Size(this.Width, 150);
                dropdown.Location = new Point(location.X, location.Y + txtSearch.Height);
                dropdown.Show();
            }
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dropdown.DataGridView.Rows[e.RowIndex];
                var rowView = row.DataBoundItem as DataRowView;
                var dataRow = rowView?.Row;

                if (dataRow != null)
                {
                    SelectedItem = dataRow;

                    txtSearch.Text = dataRow.Table.Columns.Contains(DisplayMember)
                        ? dataRow[DisplayMember]?.ToString()
                        : dataRow[0]?.ToString();

                    SelectedValue = dataRow.Table.Columns.Contains(ValueMember)
                        ? dataRow[ValueMember]?.ToString()
                        : dataRow[0]?.ToString();

                    SelectedText = txtSearch.Text;

                    OnSelectedValueChanged();
                }

                dropdown.Hide();
                txtSearch.SelectionStart = txtSearch.Text.Length;
                isPlaceholderActive = false;
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void OnSelectedValueChanged()
        {
            SelectedValueChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (this.ParentForm != null && !isFormClosingSubscribed)
            {
                this.ParentForm.FormClosing += (s, ev) => dropdown?.Hide();
                isFormClosingSubscribed = true;
            }
        }
    }
}
