namespace NorthwindTradersV6EF
{
    partial class FrmProductosListado
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
            this.tabcOperacion = new System.Windows.Forms.TabControl();
            this.tabpBuscarProducto = new System.Windows.Forms.TabPage();
            this.nudBIdFin = new System.Windows.Forms.NumericUpDown();
            this.nudBIdIni = new System.Windows.Forms.NumericUpDown();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.cboBProveedor = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboBCategoria = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBProducto = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Dgv = new System.Windows.Forms.DataGridView();
            this.tabcOperacion.SuspendLayout();
            this.tabpBuscarProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBIdFin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBIdIni)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // tabcOperacion
            // 
            this.tabcOperacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabcOperacion.Controls.Add(this.tabpBuscarProducto);
            this.tabcOperacion.Location = new System.Drawing.Point(20, 20);
            this.tabcOperacion.Margin = new System.Windows.Forms.Padding(4);
            this.tabcOperacion.Name = "tabcOperacion";
            this.tabcOperacion.SelectedIndex = 0;
            this.tabcOperacion.Size = new System.Drawing.Size(1410, 73);
            this.tabcOperacion.TabIndex = 0;
            // 
            // tabpBuscarProducto
            // 
            this.tabpBuscarProducto.Controls.Add(this.nudBIdFin);
            this.tabpBuscarProducto.Controls.Add(this.nudBIdIni);
            this.tabpBuscarProducto.Controls.Add(this.btnLimpiar);
            this.tabpBuscarProducto.Controls.Add(this.btnBuscar);
            this.tabpBuscarProducto.Controls.Add(this.cboBProveedor);
            this.tabpBuscarProducto.Controls.Add(this.label6);
            this.tabpBuscarProducto.Controls.Add(this.cboBCategoria);
            this.tabpBuscarProducto.Controls.Add(this.label5);
            this.tabpBuscarProducto.Controls.Add(this.txtBProducto);
            this.tabpBuscarProducto.Controls.Add(this.label4);
            this.tabpBuscarProducto.Controls.Add(this.label3);
            this.tabpBuscarProducto.Controls.Add(this.label2);
            this.tabpBuscarProducto.Controls.Add(this.label1);
            this.tabpBuscarProducto.Location = new System.Drawing.Point(4, 25);
            this.tabpBuscarProducto.Margin = new System.Windows.Forms.Padding(4);
            this.tabpBuscarProducto.Name = "tabpBuscarProducto";
            this.tabpBuscarProducto.Padding = new System.Windows.Forms.Padding(4);
            this.tabpBuscarProducto.Size = new System.Drawing.Size(1402, 44);
            this.tabpBuscarProducto.TabIndex = 1;
            this.tabpBuscarProducto.Text = "   Buscar un producto   ";
            this.tabpBuscarProducto.UseVisualStyleBackColor = true;
            // 
            // nudBIdFin
            // 
            this.nudBIdFin.Location = new System.Drawing.Point(395, 9);
            this.nudBIdFin.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudBIdFin.Name = "nudBIdFin";
            this.nudBIdFin.Size = new System.Drawing.Size(110, 22);
            this.nudBIdFin.TabIndex = 13;
            this.nudBIdFin.Enter += new System.EventHandler(this.Nud_Enter);
            this.nudBIdFin.Leave += new System.EventHandler(this.nudBIdFin_Leave);
            // 
            // nudBIdIni
            // 
            this.nudBIdIni.Location = new System.Drawing.Point(228, 9);
            this.nudBIdIni.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudBIdIni.Name = "nudBIdIni";
            this.nudBIdIni.Size = new System.Drawing.Size(110, 22);
            this.nudBIdIni.TabIndex = 13;
            this.nudBIdIni.Enter += new System.EventHandler(this.Nud_Enter);
            this.nudBIdIni.Leave += new System.EventHandler(this.nudBIdIni_Leave);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(1298, 6);
            this.btnLimpiar.Margin = new System.Windows.Forms.Padding(4);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(100, 28);
            this.btnLimpiar.TabIndex = 12;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(1190, 6);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(100, 28);
            this.btnBuscar.TabIndex = 11;
            this.btnBuscar.Tag = "Buscar";
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // cboBProveedor
            // 
            this.cboBProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBProveedor.FormattingEnabled = true;
            this.cboBProveedor.Location = new System.Drawing.Point(1022, 8);
            this.cboBProveedor.Margin = new System.Windows.Forms.Padding(4);
            this.cboBProveedor.Name = "cboBProveedor";
            this.cboBProveedor.Size = new System.Drawing.Size(160, 24);
            this.cboBProveedor.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(944, 12);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Proveedor:";
            // 
            // cboBCategoria
            // 
            this.cboBCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBCategoria.FormattingEnabled = true;
            this.cboBCategoria.Location = new System.Drawing.Point(778, 8);
            this.cboBCategoria.Margin = new System.Windows.Forms.Padding(4);
            this.cboBCategoria.Name = "cboBCategoria";
            this.cboBCategoria.Size = new System.Drawing.Size(160, 24);
            this.cboBCategoria.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(703, 12);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Categoría:";
            // 
            // txtBProducto
            // 
            this.txtBProducto.Location = new System.Drawing.Point(570, 9);
            this.txtBProducto.Margin = new System.Windows.Forms.Padding(4);
            this.txtBProducto.MaxLength = 40;
            this.txtBProducto.Name = "txtBProducto";
            this.txtBProducto.Size = new System.Drawing.Size(132, 22);
            this.txtBProducto.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(508, 12);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "Producto:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(343, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Id final:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Id inicial:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Buscar un producto:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.Dgv);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(20, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(30);
            this.groupBox1.Size = new System.Drawing.Size(1406, 620);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "» Listado de productos «";
            // 
            // Dgv
            // 
            this.Dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Dgv.Location = new System.Drawing.Point(30, 49);
            this.Dgv.Name = "Dgv";
            this.Dgv.RowHeadersWidth = 51;
            this.Dgv.RowTemplate.Height = 24;
            this.Dgv.Size = new System.Drawing.Size(1346, 541);
            this.Dgv.TabIndex = 0;
            // 
            // FrmProductosListado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1452, 753);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabcOperacion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmProductosListado";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Text = "» Listado de productos «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmProductosListado_FormClosed);
            this.Load += new System.EventHandler(this.FrmProductosListado_Load);
            this.tabcOperacion.ResumeLayout(false);
            this.tabpBuscarProducto.ResumeLayout(false);
            this.tabpBuscarProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBIdFin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBIdIni)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabcOperacion;
        private System.Windows.Forms.TabPage tabpBuscarProducto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBProducto;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.ComboBox cboBProveedor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboBCategoria;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView Dgv;
        private System.Windows.Forms.NumericUpDown nudBIdFin;
        private System.Windows.Forms.NumericUpDown nudBIdIni;
    }
}