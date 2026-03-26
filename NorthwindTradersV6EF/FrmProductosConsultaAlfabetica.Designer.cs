namespace NorthwindTradersV6EF
{
    partial class FrmProductosConsultaAlfabetica
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
            this.GrbProductos = new System.Windows.Forms.GroupBox();
            this.Dgv = new System.Windows.Forms.DataGridView();
            this.GrbProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // GrbProductos
            // 
            this.GrbProductos.Controls.Add(this.Dgv);
            this.GrbProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbProductos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrbProductos.Location = new System.Drawing.Point(30, 30);
            this.GrbProductos.Margin = new System.Windows.Forms.Padding(4);
            this.GrbProductos.Name = "GrbProductos";
            this.GrbProductos.Padding = new System.Windows.Forms.Padding(30);
            this.GrbProductos.Size = new System.Drawing.Size(1252, 704);
            this.GrbProductos.TabIndex = 0;
            this.GrbProductos.TabStop = false;
            this.GrbProductos.Text = "» Consulta albabética de productos: «";
            this.GrbProductos.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // Dgv
            // 
            this.Dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Dgv.Location = new System.Drawing.Point(30, 49);
            this.Dgv.Margin = new System.Windows.Forms.Padding(4);
            this.Dgv.Name = "Dgv";
            this.Dgv.RowHeadersWidth = 51;
            this.Dgv.Size = new System.Drawing.Size(1192, 625);
            this.Dgv.TabIndex = 0;
            // 
            // FrmProductosConsultaAlfabetica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1312, 764);
            this.ControlBox = false;
            this.Controls.Add(this.GrbProductos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmProductosConsultaAlfabetica";
            this.Padding = new System.Windows.Forms.Padding(30);
            this.Text = "» Consulta alfabética de productos «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmProductosConsultaAlfabetica_FormClosed);
            this.Load += new System.EventHandler(this.FrmProductosConsultaAlfabetica_Load);
            this.GrbProductos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GrbProductos;
        private System.Windows.Forms.DataGridView Dgv;
    }
}