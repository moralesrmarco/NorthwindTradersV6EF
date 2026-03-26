namespace NorthwindTradersV6EF
{
    partial class FrmGraficaVentasPorVendedores
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.GroupBox = new System.Windows.Forms.GroupBox();
            this.ChartVentasPorVendedores = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChartVentasPorVendedores)).BeginInit();
            this.SuspendLayout();
            // 
            // GroupBox
            // 
            this.GroupBox.Controls.Add(this.ChartVentasPorVendedores);
            this.GroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox.Location = new System.Drawing.Point(30, 30);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Padding = new System.Windows.Forms.Padding(20);
            this.GroupBox.Size = new System.Drawing.Size(740, 390);
            this.GroupBox.TabIndex = 0;
            this.GroupBox.TabStop = false;
            this.GroupBox.Text = "» Gráfica de ventas por vendedores de todos los años «";
            this.GroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.GrbPaint);
            // 
            // ChartVentasPorVendedores
            // 
            chartArea1.Area3DStyle.Enable3D = true;
            chartArea1.Area3DStyle.Inclination = 45;
            chartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea1.Name = "ChartArea1";
            this.ChartVentasPorVendedores.ChartAreas.Add(chartArea1);
            this.ChartVentasPorVendedores.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.ChartVentasPorVendedores.Legends.Add(legend1);
            this.ChartVentasPorVendedores.Location = new System.Drawing.Point(20, 39);
            this.ChartVentasPorVendedores.Name = "ChartVentasPorVendedores";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series1.Legend = "Legend1";
            series1.Name = "Series";
            this.ChartVentasPorVendedores.Series.Add(series1);
            this.ChartVentasPorVendedores.Size = new System.Drawing.Size(700, 331);
            this.ChartVentasPorVendedores.TabIndex = 0;
            this.ChartVentasPorVendedores.Text = "chart1";
            // 
            // FrmGraficaVentasPorVendedores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.GroupBox);
            this.Name = "FrmGraficaVentasPorVendedores";
            this.Padding = new System.Windows.Forms.Padding(30);
            this.Text = "» Gráfica ventas por vendedores de todos los años «";
            this.Load += new System.EventHandler(this.FrmGraficaVentasPorVendedores_Load);
            this.GroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChartVentasPorVendedores)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartVentasPorVendedores;
    }
}