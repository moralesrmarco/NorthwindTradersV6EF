using NorthwindTradersV6EF;
using System.Windows.Forms;

namespace NorthwindTradersV6EF
{
    partial class FrmVentasCrud
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.GrbOperaciones = new System.Windows.Forms.GroupBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnNota = new System.Windows.Forms.Button();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.grbVentas = new System.Windows.Forms.GroupBox();
            this.dgvVentas = new System.Windows.Forms.DataGridView();
            this.tabcOperacion = new System.Windows.Forms.TabControl();
            this.tabpConsultar = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabpRegistrar = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.tabpModificar = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.tabpEliminar = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.grbVenta = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.grbTransportista = new System.Windows.Forms.GroupBox();
            this.nudFlete = new Utilities.NudNoWheel();
            this.txtCP = new System.Windows.Forms.TextBox();
            this.txtCiudad = new System.Windows.Forms.TextBox();
            this.txtDirigidoa = new System.Windows.Forms.TextBox();
            this.LblCargo = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.txtPais = new System.Windows.Forms.TextBox();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.txtDomicilio = new System.Windows.Forms.TextBox();
            this.cboTransportista = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dtpHoraEnvio = new System.Windows.Forms.DateTimePicker();
            this.dtpHoraRequerido = new System.Windows.Forms.DateTimePicker();
            this.dtpHoraVenta = new System.Windows.Forms.DateTimePicker();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.dtpEnvio = new System.Windows.Forms.DateTimePicker();
            this.dtpRequerido = new System.Windows.Forms.DateTimePicker();
            this.dtpVenta = new System.Windows.Forms.DateTimePicker();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.cboEmpleado = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.controlAgregarProducto = new NorthwindTradersV6EF.ControlAgregarProducto();
            this.headerOperacion = new NorthwindTradersV6EF.Controles.CustomTabHeader();
            this.controlBuscarVenta = new NorthwindTradersV6EF.ControlBuscarVenta();
            this.controlDetalleDeLaVenta = new NorthwindTradersV6EF.ControlDetalleDeLaVenta();
            this.controlTotalesDeLaVenta = new NorthwindTradersV6EF.ControlTotalesDeLaVenta();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.GrbOperaciones.SuspendLayout();
            this.grbVentas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).BeginInit();
            this.tabcOperacion.SuspendLayout();
            this.tabpConsultar.SuspendLayout();
            this.tabpRegistrar.SuspendLayout();
            this.tabpModificar.SuspendLayout();
            this.tabpEliminar.SuspendLayout();
            this.grbVenta.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grbTransportista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFlete)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(12, 12, 25, 12);
            this.panel1.Size = new System.Drawing.Size(1907, 927);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 405F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.GrbOperaciones, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.grbVentas, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tabcOperacion, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.grbVenta, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.headerOperacion, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.controlBuscarVenta, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.controlDetalleDeLaVenta, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.controlTotalesDeLaVenta, 1, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tableLayoutPanel1.RowCount = 12;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1849, 1956);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // GrbOperaciones
            // 
            this.GrbOperaciones.Controls.Add(this.btnNuevo);
            this.GrbOperaciones.Controls.Add(this.btnNota);
            this.GrbOperaciones.Controls.Add(this.btnGenerar);
            this.GrbOperaciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrbOperaciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrbOperaciones.Location = new System.Drawing.Point(436, 1480);
            this.GrbOperaciones.Margin = new System.Windows.Forms.Padding(25, 2, 2, 2);
            this.GrbOperaciones.Name = "GrbOperaciones";
            this.GrbOperaciones.Padding = new System.Windows.Forms.Padding(2);
            this.GrbOperaciones.Size = new System.Drawing.Size(1405, 71);
            this.GrbOperaciones.TabIndex = 7;
            this.GrbOperaciones.TabStop = false;
            this.GrbOperaciones.Text = "»   Operaciones:   «";
            // 
            // btnNuevo
            // 
            this.btnNuevo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevo.Location = new System.Drawing.Point(475, 21);
            this.btnNuevo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(180, 38);
            this.btnNuevo.TabIndex = 3;
            this.btnNuevo.Text = "Nueva venta";
            this.btnNuevo.UseVisualStyleBackColor = true;
            // 
            // btnNota
            // 
            this.btnNota.AutoSize = true;
            this.btnNota.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNota.Location = new System.Drawing.Point(672, 21);
            this.btnNota.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnNota.Name = "btnNota";
            this.btnNota.Size = new System.Drawing.Size(216, 38);
            this.btnNota.TabIndex = 2;
            this.btnNota.Text = "Nota de remisión";
            this.btnNota.UseVisualStyleBackColor = true;
            // 
            // btnGenerar
            // 
            this.btnGenerar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerar.Location = new System.Drawing.Point(908, 21);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(180, 38);
            this.btnGenerar.TabIndex = 1;
            this.btnGenerar.Text = "Generar venta";
            this.btnGenerar.UseVisualStyleBackColor = true;
            // 
            // grbVentas
            // 
            this.grbVentas.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.grbVentas, 2);
            this.grbVentas.Controls.Add(this.dgvVentas);
            this.grbVentas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbVentas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbVentas.Location = new System.Drawing.Point(10, 102);
            this.grbVentas.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbVentas.Name = "grbVentas";
            this.grbVentas.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.grbVentas.Size = new System.Drawing.Size(1829, 300);
            this.grbVentas.TabIndex = 2;
            this.grbVentas.TabStop = false;
            this.grbVentas.Text = "»   Ventas:   «";
            // 
            // dgvVentas
            // 
            this.dgvVentas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVentas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVentas.Location = new System.Drawing.Point(12, 28);
            this.dgvVentas.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvVentas.Name = "dgvVentas";
            this.dgvVentas.RowHeadersWidth = 51;
            this.dgvVentas.Size = new System.Drawing.Size(1805, 260);
            this.dgvVentas.TabIndex = 0;
            this.dgvVentas.TabStop = false;
            // 
            // tabcOperacion
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabcOperacion, 2);
            this.tabcOperacion.Controls.Add(this.tabpConsultar);
            this.tabcOperacion.Controls.Add(this.tabpRegistrar);
            this.tabcOperacion.Controls.Add(this.tabpModificar);
            this.tabcOperacion.Controls.Add(this.tabpEliminar);
            this.tabcOperacion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabcOperacion.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabcOperacion.Location = new System.Drawing.Point(10, 32);
            this.tabcOperacion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabcOperacion.Name = "tabcOperacion";
            this.tabcOperacion.SelectedIndex = 0;
            this.tabcOperacion.Size = new System.Drawing.Size(1829, 52);
            this.tabcOperacion.TabIndex = 1;
            // 
            // tabpConsultar
            // 
            this.tabpConsultar.Controls.Add(this.label1);
            this.tabpConsultar.Location = new System.Drawing.Point(4, 25);
            this.tabpConsultar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpConsultar.Name = "tabpConsultar";
            this.tabpConsultar.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpConsultar.Size = new System.Drawing.Size(1821, 23);
            this.tabpConsultar.TabIndex = 0;
            this.tabpConsultar.Tag = 117;
            this.tabpConsultar.Text = "   Consultar venta   ";
            this.tabpConsultar.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(461, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Busque la venta y seleccionela en la lista que se muestra para ver su detalle";
            // 
            // tabpRegistrar
            // 
            this.tabpRegistrar.Controls.Add(this.label2);
            this.tabpRegistrar.Location = new System.Drawing.Point(4, 25);
            this.tabpRegistrar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpRegistrar.Name = "tabpRegistrar";
            this.tabpRegistrar.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpRegistrar.Size = new System.Drawing.Size(1841, 23);
            this.tabpRegistrar.TabIndex = 1;
            this.tabpRegistrar.Tag = 116;
            this.tabpRegistrar.Text = "   Registrar venta   ";
            this.tabpRegistrar.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(271, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Proporcione los datos de la venta a registrar";
            // 
            // tabpModificar
            // 
            this.tabpModificar.Controls.Add(this.label3);
            this.tabpModificar.Location = new System.Drawing.Point(4, 25);
            this.tabpModificar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpModificar.Name = "tabpModificar";
            this.tabpModificar.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpModificar.Size = new System.Drawing.Size(1841, 23);
            this.tabpModificar.TabIndex = 2;
            this.tabpModificar.Tag = 116;
            this.tabpModificar.Text = "   Modificar venta   ";
            this.tabpModificar.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(565, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Busque la venta y seleccionela en la lista que se muestra para que pueda modifica" +
    "r sus datos";
            // 
            // tabpEliminar
            // 
            this.tabpEliminar.Controls.Add(this.label4);
            this.tabpEliminar.Location = new System.Drawing.Point(4, 25);
            this.tabpEliminar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpEliminar.Name = "tabpEliminar";
            this.tabpEliminar.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabpEliminar.Size = new System.Drawing.Size(1841, 23);
            this.tabpEliminar.TabIndex = 3;
            this.tabpEliminar.Tag = 109;
            this.tabpEliminar.Text = "   Eliminar venta   ";
            this.tabpEliminar.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(408, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Busque la venta a eliminar y seleccionela en la lista que se muestra";
            // 
            // grbVenta
            // 
            this.grbVenta.AutoSize = true;
            this.grbVenta.Controls.Add(this.tableLayoutPanel2);
            this.grbVenta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbVenta.Location = new System.Drawing.Point(436, 420);
            this.grbVenta.Margin = new System.Windows.Forms.Padding(25, 4, 4, 4);
            this.grbVenta.Name = "grbVenta";
            this.grbVenta.Padding = new System.Windows.Forms.Padding(12, 0, 12, 12);
            this.grbVenta.Size = new System.Drawing.Size(1403, 820);
            this.grbVenta.TabIndex = 6;
            this.grbVenta.TabStop = false;
            this.grbVenta.Text = "»   Venta:   «";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.grbTransportista, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.controlAgregarProducto, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 16);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1379, 792);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // grbTransportista
            // 
            this.grbTransportista.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbTransportista.Controls.Add(this.nudFlete);
            this.grbTransportista.Controls.Add(this.txtCP);
            this.grbTransportista.Controls.Add(this.txtCiudad);
            this.grbTransportista.Controls.Add(this.txtDirigidoa);
            this.grbTransportista.Controls.Add(this.LblCargo);
            this.grbTransportista.Controls.Add(this.label35);
            this.grbTransportista.Controls.Add(this.label34);
            this.grbTransportista.Controls.Add(this.label33);
            this.grbTransportista.Controls.Add(this.txtPais);
            this.grbTransportista.Controls.Add(this.txtRegion);
            this.grbTransportista.Controls.Add(this.txtDomicilio);
            this.grbTransportista.Controls.Add(this.cboTransportista);
            this.grbTransportista.Controls.Add(this.label32);
            this.grbTransportista.Controls.Add(this.label31);
            this.grbTransportista.Controls.Add(this.label30);
            this.grbTransportista.Controls.Add(this.label29);
            this.grbTransportista.Location = new System.Drawing.Point(4, 120);
            this.grbTransportista.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbTransportista.Name = "grbTransportista";
            this.grbTransportista.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbTransportista.Size = new System.Drawing.Size(1371, 152);
            this.grbTransportista.TabIndex = 1;
            this.grbTransportista.TabStop = false;
            this.grbTransportista.Text = "»   Forma de envío:   «";
            // 
            // nudFlete
            // 
            this.nudFlete.DecimalPlaces = 2;
            this.nudFlete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudFlete.Location = new System.Drawing.Point(629, 110);
            this.nudFlete.Margin = new System.Windows.Forms.Padding(2);
            this.nudFlete.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudFlete.Name = "nudFlete";
            this.nudFlete.Size = new System.Drawing.Size(120, 23);
            this.nudFlete.TabIndex = 7;
            this.nudFlete.ThousandsSeparator = true;
            this.nudFlete.WheelEnabled = true;
            // 
            // txtCP
            // 
            this.txtCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCP.Location = new System.Drawing.Point(629, 79);
            this.txtCP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCP.MaxLength = 10;
            this.txtCP.Name = "txtCP";
            this.txtCP.Size = new System.Drawing.Size(132, 23);
            this.txtCP.TabIndex = 5;
            // 
            // txtCiudad
            // 
            this.txtCiudad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCiudad.Location = new System.Drawing.Point(629, 49);
            this.txtCiudad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCiudad.MaxLength = 15;
            this.txtCiudad.Name = "txtCiudad";
            this.txtCiudad.Size = new System.Drawing.Size(169, 23);
            this.txtCiudad.TabIndex = 3;
            // 
            // txtDirigidoa
            // 
            this.txtDirigidoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDirigidoa.Location = new System.Drawing.Point(629, 20);
            this.txtDirigidoa.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDirigidoa.MaxLength = 40;
            this.txtDirigidoa.Name = "txtDirigidoa";
            this.txtDirigidoa.Size = new System.Drawing.Size(404, 23);
            this.txtDirigidoa.TabIndex = 1;
            // 
            // LblCargo
            // 
            this.LblCargo.AutoSize = true;
            this.LblCargo.Location = new System.Drawing.Point(471, 112);
            this.LblCargo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblCargo.Name = "LblCargo";
            this.LblCargo.Size = new System.Drawing.Size(143, 17);
            this.LblCargo.TabIndex = 9;
            this.LblCargo.Text = "Cargo por envío $:";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(502, 84);
            this.label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(112, 17);
            this.label35.TabIndex = 8;
            this.label35.Text = "Código postal:";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(552, 54);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(63, 17);
            this.label34.TabIndex = 7;
            this.label34.Text = "Ciudad:";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(542, 25);
            this.label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(73, 17);
            this.label33.TabIndex = 6;
            this.label33.Text = "Enviar a:";
            // 
            // txtPais
            // 
            this.txtPais.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPais.Location = new System.Drawing.Point(138, 108);
            this.txtPais.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPais.MaxLength = 15;
            this.txtPais.Name = "txtPais";
            this.txtPais.Size = new System.Drawing.Size(136, 23);
            this.txtPais.TabIndex = 6;
            // 
            // txtRegion
            // 
            this.txtRegion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRegion.Location = new System.Drawing.Point(138, 79);
            this.txtRegion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtRegion.MaxLength = 15;
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(136, 23);
            this.txtRegion.TabIndex = 4;
            // 
            // txtDomicilio
            // 
            this.txtDomicilio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDomicilio.Location = new System.Drawing.Point(138, 49);
            this.txtDomicilio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDomicilio.MaxLength = 60;
            this.txtDomicilio.Name = "txtDomicilio";
            this.txtDomicilio.Size = new System.Drawing.Size(332, 23);
            this.txtDomicilio.TabIndex = 2;
            // 
            // cboTransportista
            // 
            this.cboTransportista.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTransportista.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTransportista.FormattingEnabled = true;
            this.cboTransportista.Location = new System.Drawing.Point(138, 20);
            this.cboTransportista.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboTransportista.Name = "cboTransportista";
            this.cboTransportista.Size = new System.Drawing.Size(332, 25);
            this.cboTransportista.TabIndex = 0;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(82, 112);
            this.label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(44, 17);
            this.label32.TabIndex = 3;
            this.label32.Text = "País:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(65, 84);
            this.label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(64, 17);
            this.label31.TabIndex = 2;
            this.label31.Text = "Región:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(50, 54);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(78, 17);
            this.label30.TabIndex = 1;
            this.label30.Text = "Domicilio:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(42, 25);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(84, 17);
            this.label29.TabIndex = 0;
            this.label29.Text = "Compañía:";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.dtpHoraEnvio);
            this.panel2.Controls.Add(this.dtpHoraRequerido);
            this.panel2.Controls.Add(this.dtpHoraVenta);
            this.panel2.Controls.Add(this.label28);
            this.panel2.Controls.Add(this.label27);
            this.panel2.Controls.Add(this.label26);
            this.panel2.Controls.Add(this.dtpEnvio);
            this.panel2.Controls.Add(this.dtpRequerido);
            this.panel2.Controls.Add(this.dtpVenta);
            this.panel2.Controls.Add(this.label25);
            this.panel2.Controls.Add(this.label24);
            this.panel2.Controls.Add(this.label23);
            this.panel2.Controls.Add(this.cboEmpleado);
            this.panel2.Controls.Add(this.label22);
            this.panel2.Controls.Add(this.cboCliente);
            this.panel2.Controls.Add(this.label21);
            this.panel2.Controls.Add(this.txtId);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 4);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1371, 98);
            this.panel2.TabIndex = 0;
            // 
            // dtpHoraEnvio
            // 
            this.dtpHoraEnvio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpHoraEnvio.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraEnvio.Location = new System.Drawing.Point(872, 69);
            this.dtpHoraEnvio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpHoraEnvio.Name = "dtpHoraEnvio";
            this.dtpHoraEnvio.ShowUpDown = true;
            this.dtpHoraEnvio.Size = new System.Drawing.Size(152, 23);
            this.dtpHoraEnvio.TabIndex = 27;
            // 
            // dtpHoraRequerido
            // 
            this.dtpHoraRequerido.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpHoraRequerido.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraRequerido.Location = new System.Drawing.Point(872, 39);
            this.dtpHoraRequerido.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpHoraRequerido.Name = "dtpHoraRequerido";
            this.dtpHoraRequerido.ShowUpDown = true;
            this.dtpHoraRequerido.Size = new System.Drawing.Size(152, 23);
            this.dtpHoraRequerido.TabIndex = 23;
            // 
            // dtpHoraVenta
            // 
            this.dtpHoraVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpHoraVenta.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraVenta.Location = new System.Drawing.Point(872, 10);
            this.dtpHoraVenta.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpHoraVenta.Name = "dtpHoraVenta";
            this.dtpHoraVenta.ShowUpDown = true;
            this.dtpHoraVenta.Size = new System.Drawing.Size(152, 23);
            this.dtpHoraVenta.TabIndex = 20;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(819, 74);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(48, 17);
            this.label28.TabIndex = 32;
            this.label28.Text = "Hora:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(819, 44);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(48, 17);
            this.label27.TabIndex = 31;
            this.label27.Text = "Hora:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(819, 15);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(48, 17);
            this.label26.TabIndex = 30;
            this.label26.Text = "Hora:";
            // 
            // dtpEnvio
            // 
            this.dtpEnvio.Checked = false;
            this.dtpEnvio.CustomFormat = "dd/MMM/yyyy";
            this.dtpEnvio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpEnvio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnvio.Location = new System.Drawing.Point(626, 69);
            this.dtpEnvio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpEnvio.Name = "dtpEnvio";
            this.dtpEnvio.ShowCheckBox = true;
            this.dtpEnvio.Size = new System.Drawing.Size(176, 23);
            this.dtpEnvio.TabIndex = 24;
            // 
            // dtpRequerido
            // 
            this.dtpRequerido.Checked = false;
            this.dtpRequerido.CustomFormat = "dd/MMM/yyyy";
            this.dtpRequerido.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpRequerido.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpRequerido.Location = new System.Drawing.Point(626, 39);
            this.dtpRequerido.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpRequerido.Name = "dtpRequerido";
            this.dtpRequerido.ShowCheckBox = true;
            this.dtpRequerido.Size = new System.Drawing.Size(176, 23);
            this.dtpRequerido.TabIndex = 21;
            // 
            // dtpVenta
            // 
            this.dtpVenta.CustomFormat = "dd/MMM/yyyy";
            this.dtpVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpVenta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpVenta.Location = new System.Drawing.Point(626, 10);
            this.dtpVenta.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpVenta.Name = "dtpVenta";
            this.dtpVenta.ShowCheckBox = true;
            this.dtpVenta.Size = new System.Drawing.Size(176, 23);
            this.dtpVenta.TabIndex = 19;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(490, 74);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(124, 17);
            this.label25.TabIndex = 29;
            this.label25.Text = "Fecha de envío:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(478, 44);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(132, 17);
            this.label24.TabIndex = 26;
            this.label24.Text = "Fecha requerido:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(485, 15);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(125, 17);
            this.label23.TabIndex = 25;
            this.label23.Text = "Fecha de venta:";
            // 
            // cboEmpleado
            // 
            this.cboEmpleado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEmpleado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboEmpleado.FormattingEnabled = true;
            this.cboEmpleado.Location = new System.Drawing.Point(136, 69);
            this.cboEmpleado.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboEmpleado.Name = "cboEmpleado";
            this.cboEmpleado.Size = new System.Drawing.Size(332, 25);
            this.cboEmpleado.TabIndex = 17;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(45, 74);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(83, 17);
            this.label22.TabIndex = 22;
            this.label22.Text = "Vendedor:";
            // 
            // cboCliente
            // 
            this.cboCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboCliente.FormattingEnabled = true;
            this.cboCliente.Location = new System.Drawing.Point(136, 39);
            this.cboCliente.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboCliente.Name = "cboCliente";
            this.cboCliente.Size = new System.Drawing.Size(332, 25);
            this.cboCliente.TabIndex = 16;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(66, 44);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(63, 17);
            this.label21.TabIndex = 18;
            this.label21.Text = "Cliente:";
            // 
            // txtId
            // 
            this.txtId.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtId.Location = new System.Drawing.Point(136, 4);
            this.txtId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtId.MaxLength = 10;
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(150, 26);
            this.txtId.TabIndex = 28;
            this.txtId.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(105, 9);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(26, 17);
            this.label20.TabIndex = 15;
            this.label20.Text = "Id:";
            // 
            // controlAgregarProducto
            // 
            this.controlAgregarProducto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlAgregarProducto.Location = new System.Drawing.Point(5, 290);
            this.controlAgregarProducto.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.controlAgregarProducto.Name = "controlAgregarProducto";
            this.controlAgregarProducto.Size = new System.Drawing.Size(1369, 498);
            this.controlAgregarProducto.TabIndex = 2;
            // 
            // headerOperacion
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.headerOperacion, 2);
            this.headerOperacion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerOperacion.IconOff = null;
            this.headerOperacion.IconOn = null;
            this.headerOperacion.Location = new System.Drawing.Point(6, 0);
            this.headerOperacion.Margin = new System.Windows.Forms.Padding(0);
            this.headerOperacion.Name = "headerOperacion";
            this.headerOperacion.Size = new System.Drawing.Size(1837, 28);
            this.headerOperacion.TabControl = null;
            this.headerOperacion.TabIndex = 10;
            this.headerOperacion.WrapContents = false;
            // 
            // controlBuscarVenta
            // 
            this.controlBuscarVenta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlBuscarVenta.Location = new System.Drawing.Point(11, 421);
            this.controlBuscarVenta.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.controlBuscarVenta.Name = "controlBuscarVenta";
            this.tableLayoutPanel1.SetRowSpan(this.controlBuscarVenta, 7);
            this.controlBuscarVenta.Size = new System.Drawing.Size(395, 1530);
            this.controlBuscarVenta.TabIndex = 11;
            // 
            // controlDetalleDeLaVenta
            // 
            this.controlDetalleDeLaVenta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlDetalleDeLaVenta.Location = new System.Drawing.Point(436, 1567);
            this.controlDetalleDeLaVenta.Margin = new System.Windows.Forms.Padding(25, 4, 4, 4);
            this.controlDetalleDeLaVenta.Name = "controlDetalleDeLaVenta";
            this.controlDetalleDeLaVenta.Size = new System.Drawing.Size(1403, 385);
            this.controlDetalleDeLaVenta.TabIndex = 12;
            // 
            // controlTotalesDeLaVenta
            // 
            this.controlTotalesDeLaVenta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlTotalesDeLaVenta.Location = new System.Drawing.Point(436, 1258);
            this.controlTotalesDeLaVenta.Margin = new System.Windows.Forms.Padding(25, 4, 4, 4);
            this.controlTotalesDeLaVenta.Name = "controlTotalesDeLaVenta";
            this.controlTotalesDeLaVenta.Size = new System.Drawing.Size(1403, 206);
            this.controlTotalesDeLaVenta.TabIndex = 13;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FrmVentasCrud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1919, 951);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmVentasCrud";
            this.Padding = new System.Windows.Forms.Padding(12, 12, 0, 12);
            this.Text = "» Mantenimiento de Ventas «";
            this.Load += new System.EventHandler(this.FrmVentasCrud_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.GrbOperaciones.ResumeLayout(false);
            this.GrbOperaciones.PerformLayout();
            this.grbVentas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).EndInit();
            this.tabcOperacion.ResumeLayout(false);
            this.tabpConsultar.ResumeLayout(false);
            this.tabpConsultar.PerformLayout();
            this.tabpRegistrar.ResumeLayout(false);
            this.tabpRegistrar.PerformLayout();
            this.tabpModificar.ResumeLayout(false);
            this.tabpModificar.PerformLayout();
            this.tabpEliminar.ResumeLayout(false);
            this.tabpEliminar.PerformLayout();
            this.grbVenta.ResumeLayout(false);
            this.grbVenta.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.grbTransportista.ResumeLayout(false);
            this.grbTransportista.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFlete)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grbVentas;
        private System.Windows.Forms.DataGridView dgvVentas;
        private System.Windows.Forms.TabControl tabcOperacion;
        private System.Windows.Forms.TabPage tabpConsultar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabpRegistrar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabpModificar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabpEliminar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grbVenta;
        private System.Windows.Forms.GroupBox GrbOperaciones;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnNota;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dtpHoraEnvio;
        private System.Windows.Forms.DateTimePicker dtpHoraRequerido;
        private System.Windows.Forms.DateTimePicker dtpHoraVenta;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.DateTimePicker dtpEnvio;
        private System.Windows.Forms.DateTimePicker dtpRequerido;
        private System.Windows.Forms.DateTimePicker dtpVenta;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox cboEmpleado;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.GroupBox grbTransportista;
        private Utilities.NudNoWheel nudFlete;
        private System.Windows.Forms.TextBox txtCP;
        private System.Windows.Forms.TextBox txtCiudad;
        private System.Windows.Forms.TextBox txtDirigidoa;
        private System.Windows.Forms.Label LblCargo;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox txtPais;
        private System.Windows.Forms.TextBox txtRegion;
        private System.Windows.Forms.TextBox txtDomicilio;
        private System.Windows.Forms.ComboBox cboTransportista;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.ToolTip toolTip1;
        private Controles.CustomTabHeader headerOperacion;
        private ControlBuscarVenta controlBuscarVenta;
        private ControlAgregarProducto controlAgregarProducto;
        private ControlDetalleDeLaVenta controlDetalleDeLaVenta;
        private ControlTotalesDeLaVenta controlTotalesDeLaVenta;
        internal System.Windows.Forms.ErrorProvider errorProvider1;
    }
}