
namespace toolVudaco.NCC
{
    partial class nhaCungCap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.congNoChiTiet1 = new toolVudaco.NCC.congNoChiTiet();
            this.SuspendLayout();
            // 
            // congNoChiTiet1
            // 
            this.congNoChiTiet1.Location = new System.Drawing.Point(2, 2);
            this.congNoChiTiet1.Name = "congNoChiTiet1";
            this.congNoChiTiet1.Size = new System.Drawing.Size(1282, 550);
            this.congNoChiTiet1.TabIndex = 0;
            // 
            // nhaCungCap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1286, 555);
            this.Controls.Add(this.congNoChiTiet1);
            this.Name = "nhaCungCap";
            this.Text = "Nhà Cung Cấp";
            this.Load += new System.EventHandler(this.nhaCungCap_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private congNoChiTiet congNoChiTiet1;
    }
}