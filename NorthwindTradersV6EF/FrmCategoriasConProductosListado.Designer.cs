namespace NorthwindTradersV6EF
{
    partial class FrmCategoriasConProductosListado
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
            this.GrbListado = new System.Windows.Forms.GroupBox();
            this.DgvListado = new System.Windows.Forms.DataGridView();
            this.GrbListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvListado)).BeginInit();
            this.SuspendLayout();
            // 
            // GrbListado
            // 
            this.GrbListado.Controls.Add(this.DgvListado);
            this.GrbListado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbListado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrbListado.Location = new System.Drawing.Point(30, 30);
            this.GrbListado.Margin = new System.Windows.Forms.Padding(4);
            this.GrbListado.Name = "GrbListado";
            this.GrbListado.Padding = new System.Windows.Forms.Padding(30);
            this.GrbListado.Size = new System.Drawing.Size(1252, 704);
            this.GrbListado.TabIndex = 0;
            this.GrbListado.TabStop = false;
            this.GrbListado.Text = "» Listado de categorías con productos «";
            this.GrbListado.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // DgvListado
            // 
            this.DgvListado.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvListado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvListado.Location = new System.Drawing.Point(30, 49);
            this.DgvListado.Margin = new System.Windows.Forms.Padding(4);
            this.DgvListado.Name = "DgvListado";
            this.DgvListado.RowHeadersWidth = 51;
            this.DgvListado.Size = new System.Drawing.Size(1192, 625);
            this.DgvListado.TabIndex = 0;
            // 
            // FrmCategoriasConProductosListado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1312, 764);
            this.ControlBox = false;
            this.Controls.Add(this.GrbListado);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmCategoriasConProductosListado";
            this.Padding = new System.Windows.Forms.Padding(30);
            this.Text = "» Listado de categorías con productos «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCategoriaConProductos_FormClosed);
            this.Load += new System.EventHandler(this.FrmCategoriasConProductosListado_Load);
            this.GrbListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvListado)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GrbListado;
        private System.Windows.Forms.DataGridView DgvListado;
    }
}