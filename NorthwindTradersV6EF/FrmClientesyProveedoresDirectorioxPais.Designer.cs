namespace NorthwindTradersV6EF
{
    partial class FrmClientesyProveedoresDirectorioxPais
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
            this.checkBoxProveedores = new System.Windows.Forms.CheckBox();
            this.checkBoxClientes = new System.Windows.Forms.CheckBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Grb = new System.Windows.Forms.GroupBox();
            this.Dgv = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.Grb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(20, 20);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxProveedores);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxClientes);
            this.splitContainer1.Panel1.Controls.Add(this.btnBuscar);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(15);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Grb);
            this.splitContainer1.Size = new System.Drawing.Size(1027, 514);
            this.splitContainer1.SplitterDistance = 58;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 0;
            // 
            // checkBoxProveedores
            // 
            this.checkBoxProveedores.AutoSize = true;
            this.checkBoxProveedores.Checked = true;
            this.checkBoxProveedores.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProveedores.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxProveedores.Location = new System.Drawing.Point(725, 14);
            this.checkBoxProveedores.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxProveedores.Name = "checkBoxProveedores";
            this.checkBoxProveedores.Size = new System.Drawing.Size(136, 24);
            this.checkBoxProveedores.TabIndex = 26;
            this.checkBoxProveedores.Text = "Proveedores";
            this.checkBoxProveedores.UseVisualStyleBackColor = true;
            // 
            // checkBoxClientes
            // 
            this.checkBoxClientes.AutoSize = true;
            this.checkBoxClientes.Checked = true;
            this.checkBoxClientes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxClientes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxClientes.Location = new System.Drawing.Point(583, 14);
            this.checkBoxClientes.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxClientes.Name = "checkBoxClientes";
            this.checkBoxClientes.Size = new System.Drawing.Size(100, 24);
            this.checkBoxClientes.TabIndex = 25;
            this.checkBoxClientes.Text = "Clientes";
            this.checkBoxClientes.UseVisualStyleBackColor = true;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(890, 12);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(100, 28);
            this.btnBuscar.TabIndex = 24;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // comboBox
            // 
            this.comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(188, 12);
            this.comboBox.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(344, 28);
            this.comboBox.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 20);
            this.label1.TabIndex = 22;
            this.label1.Text = "Buscar por país:";
            // 
            // Grb
            // 
            this.Grb.Controls.Add(this.Dgv);
            this.Grb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Grb.Location = new System.Drawing.Point(0, 0);
            this.Grb.Name = "Grb";
            this.Grb.Padding = new System.Windows.Forms.Padding(20);
            this.Grb.Size = new System.Drawing.Size(1027, 446);
            this.Grb.TabIndex = 0;
            this.Grb.TabStop = false;
            this.Grb.Text = "» Directorio de clientes y proveedores por país «";
            this.Grb.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // Dgv
            // 
            this.Dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Dgv.Location = new System.Drawing.Point(20, 39);
            this.Dgv.Name = "Dgv";
            this.Dgv.RowHeadersWidth = 51;
            this.Dgv.RowTemplate.Height = 24;
            this.Dgv.Size = new System.Drawing.Size(987, 387);
            this.Dgv.TabIndex = 0;
            // 
            // FrmClientesyProveedoresDirectorioxPais
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmClientesyProveedoresDirectorioxPais";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Text = "» Directorio de clientes y proveedores por país «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmClientesyProveedoresDirectorioxPais_FormClosed);
            this.Load += new System.EventHandler(this.FrmClientesyProveedoresDirectorioxPais_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.Grb.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox checkBoxProveedores;
        private System.Windows.Forms.CheckBox checkBoxClientes;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox Grb;
        private System.Windows.Forms.DataGridView Dgv;
    }
}