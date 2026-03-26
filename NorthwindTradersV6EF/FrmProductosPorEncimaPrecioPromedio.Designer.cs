namespace NorthwindTradersV6EF
{
    partial class FrmProductosPorEncimaPrecioPromedio
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
            this.Grb = new System.Windows.Forms.GroupBox();
            this.Dgv = new System.Windows.Forms.DataGridView();
            this.Grb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // Grb
            // 
            this.Grb.Controls.Add(this.Dgv);
            this.Grb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Grb.Location = new System.Drawing.Point(30, 30);
            this.Grb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Grb.Name = "Grb";
            this.Grb.Padding = new System.Windows.Forms.Padding(30);
            this.Grb.Size = new System.Drawing.Size(1007, 494);
            this.Grb.TabIndex = 0;
            this.Grb.TabStop = false;
            this.Grb.Text = "»   Listado de productos con el precio por encima del precio promedio:   «";
            this.Grb.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // Dgv
            // 
            this.Dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Dgv.Location = new System.Drawing.Point(30, 49);
            this.Dgv.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Dgv.Name = "Dgv";
            this.Dgv.RowHeadersWidth = 51;
            this.Dgv.Size = new System.Drawing.Size(947, 415);
            this.Dgv.TabIndex = 0;
            // 
            // FrmProductosPorEncimaPrecioPromedio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.ControlBox = false;
            this.Controls.Add(this.Grb);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmProductosPorEncimaPrecioPromedio";
            this.Padding = new System.Windows.Forms.Padding(30);
            this.Text = "» Productos por encima del precio promedio «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmProductosPorEncimaPrecioPromedio_FormClosed);
            this.Load += new System.EventHandler(this.FrmProductosPorEncimaPrecioPromedio_Load);
            this.Grb.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Grb;
        private System.Windows.Forms.DataGridView Dgv;
    }
}