namespace NorthwindTradersV6EF
{
    partial class ControlDetalleDeLaVenta
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.grbDetalle = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Producto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Precio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImporteDelDescuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImporteConDescuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TasaIVA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImporteSinIVA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImporteDelIVA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Subtotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Modificar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Eliminar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ProductoId = new System.Windows.Forms.DataGridViewButtonColumn();
            this.RowVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            // 
            // grbDetalle
            // 
            this.grbDetalle.Controls.Add(this.dgvDetalle);
            this.grbDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbDetalle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbDetalle.Location = new System.Drawing.Point(0, 0);
            this.grbDetalle.Name = "grbDetalle";
            this.grbDetalle.Padding = new System.Windows.Forms.Padding(10);
            this.grbDetalle.Size = new System.Drawing.Size(1138, 302);
            this.grbDetalle.TabIndex = 10;
            this.grbDetalle.TabStop = false;
            this.grbDetalle.Text = "»   Detalle de la venta:   «";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Producto,
            this.Precio,
            this.Cantidad,
            this.Importe,
            this.Descuento,
            this.ImporteDelDescuento,
            this.ImporteConDescuento,
            this.TasaIVA,
            this.ImporteSinIVA,
            this.ImporteDelIVA,
            this.Subtotal,
            this.Modificar,
            this.Eliminar,
            this.ProductoId,
            this.RowVersion});
            this.dgvDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalle.Location = new System.Drawing.Point(10, 23);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.RowHeadersWidth = 51;
            this.dgvDetalle.Size = new System.Drawing.Size(1118, 269);
            this.dgvDetalle.TabIndex = 0;
            this.dgvDetalle.TabStop = false;
            // 
            // Id
            // 
            this.Id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Id.HeaderText = "N°";
            this.Id.MinimumWidth = 6;
            this.Id.Name = "Id";
            this.Id.Width = 46;
            // 
            // Producto
            // 
            this.Producto.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Producto.HeaderText = "Producto";
            this.Producto.MinimumWidth = 6;
            this.Producto.Name = "Producto";
            // 
            // Precio
            // 
            this.Precio.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Precio.HeaderText = "Precio con IVA incluido";
            this.Precio.MinimumWidth = 6;
            this.Precio.Name = "Precio";
            this.Precio.Width = 111;
            // 
            // Cantidad
            // 
            this.Cantidad.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cantidad.HeaderText = "Cantidad";
            this.Cantidad.MinimumWidth = 6;
            this.Cantidad.Name = "Cantidad";
            this.Cantidad.Width = 82;
            // 
            // Importe
            // 
            this.Importe.HeaderText = "Importe";
            this.Importe.MinimumWidth = 6;
            this.Importe.Name = "Importe";
            this.Importe.Width = 125;
            // 
            // Descuento
            // 
            this.Descuento.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Descuento.HeaderText = "Descuento";
            this.Descuento.MinimumWidth = 6;
            this.Descuento.Name = "Descuento";
            this.Descuento.Width = 93;
            // 
            // ImporteDelDescuento
            // 
            this.ImporteDelDescuento.HeaderText = "Importe del descuento";
            this.ImporteDelDescuento.MinimumWidth = 6;
            this.ImporteDelDescuento.Name = "ImporteDelDescuento";
            this.ImporteDelDescuento.Width = 125;
            // 
            // ImporteConDescuento
            // 
            this.ImporteConDescuento.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ImporteConDescuento.HeaderText = "Importe con descuento";
            this.ImporteConDescuento.MinimumWidth = 6;
            this.ImporteConDescuento.Name = "ImporteConDescuento";
            this.ImporteConDescuento.ToolTipText = "(con IVA incluido)";
            this.ImporteConDescuento.Width = 148;
            // 
            // TasaIVA
            // 
            this.TasaIVA.HeaderText = "Tasa IVA";
            this.TasaIVA.MinimumWidth = 6;
            this.TasaIVA.Name = "TasaIVA";
            this.TasaIVA.Width = 125;
            // 
            // ImporteSinIVA
            // 
            this.ImporteSinIVA.HeaderText = "Importe sin IVA";
            this.ImporteSinIVA.MinimumWidth = 6;
            this.ImporteSinIVA.Name = "ImporteSinIVA";
            this.ImporteSinIVA.ToolTipText = "(despues del descuento)";
            this.ImporteSinIVA.Width = 125;
            // 
            // ImporteDelIVA
            // 
            this.ImporteDelIVA.HeaderText = "Importe del IVA (Incluido)";
            this.ImporteDelIVA.MinimumWidth = 6;
            this.ImporteDelIVA.Name = "ImporteDelIVA";
            this.ImporteDelIVA.Width = 125;
            // 
            // Subtotal
            // 
            this.Subtotal.HeaderText = "Subtotal";
            this.Subtotal.MinimumWidth = 6;
            this.Subtotal.Name = "Subtotal";
            this.Subtotal.Width = 125;
            // 
            // Modificar
            // 
            this.Modificar.HeaderText = "Modificar producto";
            this.Modificar.Name = "Modificar";
            this.Modificar.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Modificar.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Eliminar
            // 
            this.Eliminar.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Eliminar.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Eliminar.HeaderText = "Eliminar producto";
            this.Eliminar.MinimumWidth = 6;
            this.Eliminar.Name = "Eliminar";
            this.Eliminar.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ProductoId
            // 
            this.ProductoId.HeaderText = "ProductoId";
            this.ProductoId.MinimumWidth = 6;
            this.ProductoId.Name = "ProductoId";
            this.ProductoId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ProductoId.Visible = false;
            this.ProductoId.Width = 125;
            // 
            // RowVersion
            // 
            this.RowVersion.HeaderText = "RowVersion";
            this.RowVersion.MinimumWidth = 6;
            this.RowVersion.Name = "RowVersion";
            this.RowVersion.Visible = false;
            this.RowVersion.Width = 125;
            // 
            // ControlDetalleDeLaVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grbDetalle);
            this.Name = "ControlDetalleDeLaVenta";
            this.Size = new System.Drawing.Size(1138, 302);
            this.grbDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbDetalle;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Producto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Precio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImporteDelDescuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImporteConDescuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn TasaIVA;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImporteSinIVA;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImporteDelIVA;
        private System.Windows.Forms.DataGridViewTextBoxColumn Subtotal;
        private System.Windows.Forms.DataGridViewButtonColumn Modificar;
        private System.Windows.Forms.DataGridViewButtonColumn Eliminar;
        private System.Windows.Forms.DataGridViewButtonColumn ProductoId;
        private System.Windows.Forms.DataGridViewTextBoxColumn RowVersion;
    }
}
