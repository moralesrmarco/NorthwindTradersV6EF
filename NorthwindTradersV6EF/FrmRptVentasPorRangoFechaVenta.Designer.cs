namespace NorthwindTradersV6EF
{
    partial class FrmRptVentasPorRangoFechaVenta
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnImprimir = new System.Windows.Forms.Button();
            this.DtpVentaFin = new System.Windows.Forms.DateTimePicker();
            this.DtpVentaIni = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(38, 25);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 375F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1024, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnImprimir);
            this.panel1.Controls.Add(this.DtpVentaFin);
            this.panel1.Controls.Add(this.DtpVentaIni);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 41);
            this.panel1.TabIndex = 0;
            // 
            // BtnImprimir
            // 
            this.BtnImprimir.AutoSize = true;
            this.BtnImprimir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnImprimir.Location = new System.Drawing.Point(788, 0);
            this.BtnImprimir.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.BtnImprimir.Name = "BtnImprimir";
            this.BtnImprimir.Size = new System.Drawing.Size(219, 41);
            this.BtnImprimir.TabIndex = 9;
            this.BtnImprimir.Text = "Mostrar reporte";
            this.BtnImprimir.UseVisualStyleBackColor = true;
            this.BtnImprimir.Click += new System.EventHandler(this.BtnImprimir_Click);
            // 
            // DtpVentaFin
            // 
            this.DtpVentaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtpVentaFin.Location = new System.Drawing.Point(575, 8);
            this.DtpVentaFin.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.DtpVentaFin.Name = "DtpVentaFin";
            this.DtpVentaFin.ShowCheckBox = true;
            this.DtpVentaFin.Size = new System.Drawing.Size(155, 22);
            this.DtpVentaFin.TabIndex = 8;
            this.DtpVentaFin.ValueChanged += new System.EventHandler(this.DtpVentaFin_ValueChanged);
            this.DtpVentaFin.Leave += new System.EventHandler(this.DtpVentaFin_Leave);
            // 
            // DtpVentaIni
            // 
            this.DtpVentaIni.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtpVentaIni.Location = new System.Drawing.Point(202, 8);
            this.DtpVentaIni.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.DtpVentaIni.Name = "DtpVentaIni";
            this.DtpVentaIni.ShowCheckBox = true;
            this.DtpVentaIni.Size = new System.Drawing.Size(155, 22);
            this.DtpVentaIni.TabIndex = 7;
            this.DtpVentaIni.ValueChanged += new System.EventHandler(this.DtpVentaIni_ValueChanged);
            this.DtpVentaIni.Leave += new System.EventHandler(this.DtpVentaIni_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(398, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "Fecha de venta final:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "Fecha de venta inicial:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.reportViewer1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(0, 53);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(25, 25, 25, 25);
            this.groupBox1.Size = new System.Drawing.Size(1024, 397);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "» Reporte de ventas por rango de fecha de venta «";
            this.groupBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // reportViewer1
            // 
            this.reportViewer1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "NorthwindTradersV6EF.RptVentasPorRangoFechaVenta.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(25, 44);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(974, 328);
            this.reportViewer1.TabIndex = 0;
            // 
            // FrmRptVentasPorRangoFechaVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1100, 500);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmRptVentasPorRangoFechaVenta";
            this.Padding = new System.Windows.Forms.Padding(38, 25, 38, 25);
            this.Text = "» Reporte de ventas por rango de fecha de venta «";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmRptVentasPorRangoFechaVenta_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnImprimir;
        private System.Windows.Forms.DateTimePicker DtpVentaFin;
        private System.Windows.Forms.DateTimePicker DtpVentaIni;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}