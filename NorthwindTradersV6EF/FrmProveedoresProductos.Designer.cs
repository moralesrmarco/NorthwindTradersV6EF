namespace NorthwindTradersV6EF
{
    partial class FrmProveedoresProductos
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grbProveedores = new System.Windows.Forms.GroupBox();
            this.DgvProveedores = new System.Windows.Forms.DataGridView();
            this.grbProductos = new System.Windows.Forms.GroupBox();
            this.DgvProductos = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grbProveedores.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvProveedores)).BeginInit();
            this.grbProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvProductos)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(20, 20);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grbProveedores);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(13, 4, 13, 4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grbProductos);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(13, 4, 13, 4);
            this.splitContainer1.Size = new System.Drawing.Size(1590, 905);
            this.splitContainer1.SplitterDistance = 451;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // grbProveedores
            // 
            this.grbProveedores.Controls.Add(this.DgvProveedores);
            this.grbProveedores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbProveedores.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbProveedores.Location = new System.Drawing.Point(13, 4);
            this.grbProveedores.Margin = new System.Windows.Forms.Padding(4);
            this.grbProveedores.Name = "grbProveedores";
            this.grbProveedores.Padding = new System.Windows.Forms.Padding(20);
            this.grbProveedores.Size = new System.Drawing.Size(1564, 443);
            this.grbProveedores.TabIndex = 0;
            this.grbProveedores.TabStop = false;
            this.grbProveedores.Text = "»   Proveedores:   «";
            this.grbProveedores.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // DgvProveedores
            // 
            this.DgvProveedores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvProveedores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvProveedores.Location = new System.Drawing.Point(20, 39);
            this.DgvProveedores.Margin = new System.Windows.Forms.Padding(4);
            this.DgvProveedores.Name = "DgvProveedores";
            this.DgvProveedores.RowHeadersWidth = 51;
            this.DgvProveedores.Size = new System.Drawing.Size(1524, 384);
            this.DgvProveedores.TabIndex = 0;
            this.DgvProveedores.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.DgvProveedores_DataBindingComplete);
            this.DgvProveedores.SelectionChanged += new System.EventHandler(this.DgvProveedores_SelectionChanged);
            // 
            // grbProductos
            // 
            this.grbProductos.Controls.Add(this.DgvProductos);
            this.grbProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbProductos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbProductos.Location = new System.Drawing.Point(13, 4);
            this.grbProductos.Margin = new System.Windows.Forms.Padding(4);
            this.grbProductos.Name = "grbProductos";
            this.grbProductos.Padding = new System.Windows.Forms.Padding(20);
            this.grbProductos.Size = new System.Drawing.Size(1564, 441);
            this.grbProductos.TabIndex = 0;
            this.grbProductos.TabStop = false;
            this.grbProductos.Text = "»   Productos:   «";
            this.grbProductos.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // DgvProductos
            // 
            this.DgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvProductos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvProductos.Location = new System.Drawing.Point(20, 39);
            this.DgvProductos.Margin = new System.Windows.Forms.Padding(4);
            this.DgvProductos.Name = "DgvProductos";
            this.DgvProductos.RowHeadersWidth = 51;
            this.DgvProductos.Size = new System.Drawing.Size(1524, 382);
            this.DgvProductos.TabIndex = 0;
            // 
            // FrmProveedoresProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1312, 764);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmProveedoresProductos";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Text = "» Consulta de productos por proveedor «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmProveedoresProductos_FormClosed);
            this.Load += new System.EventHandler(this.FrmProveedoresProductos_Load);
            this.Shown += new System.EventHandler(this.FrmProveedoresProductos_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grbProveedores.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvProveedores)).EndInit();
            this.grbProductos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvProductos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grbProveedores;
        private System.Windows.Forms.GroupBox grbProductos;
        private System.Windows.Forms.DataGridView DgvProveedores;
        private System.Windows.Forms.DataGridView DgvProductos;
    }
}