using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using WinFormsSortOrder = System.Windows.Forms.SortOrder;

namespace Utilities
{
    public static class Utils
    {
        #region VariablesGlobales
        public static string nwtr => ConfigurationManager.AppSettings["nwtr"];
        public const string clbdd = "Consultando la base de datos... ";
        public const string oueclbdd = "Ocurrio un error con la base de datos:\n";
        public const string oue = "Ocurrio un error:\n";
        public const string preguntaCerrar = "[orange]Se detectaron cambios en los datos del formulario.\n[blue]¿Esta seguro de querer cerrar el formulario?\n[red]Si responde SI, se perderan los datos no guardados.";
        public const string preguntaCerrarPestaña = "[orange]Se detectaron cambios en los datos, que no han sido guardados.\n[blue]Si cambia de pestaña se perderan los datos no guardados.\n[red]¿Desea cambiar de pestaña?\n[blue]Si responde SI, se perderan los datos no guardados.";
        public const string insertandoRegistro = "Insertando registro(s) en la base de datos...";
        public const string modificandoRegistro = "Actualizando registro(s) en la base de datos...";
        public const string eliminandoRegistro = "Eliminando registro(s) en la base de datos...";
        public const string errorCriterioSelec = "[orange]     Error: Proporcione los criterios de selección.";
        public const string noDatos = "No se encontraron registros para mostrar en el reporte.";
        public const string srs = "\n[green]Se registró satisfactoriamente.";
        public const string nfrs = "\n[red]NO fue registrado en la base de datos por un motivo desconocido.";
        public const string sms = "\n[green]Se actualizó satisfactoriamente.";
        public const string nfrfa = "\n[red]NO fue registrado en la base de datos, el registro ya fue agregado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfrii = "\n[red]NO fue registrado en la base de datos, el inventario del producto fue insuficiente.\n[gold]El registro de productos pudo haber sido modificado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfrie = "\n[red]NO fue registrado en la base de datos, el inventario del producto excedió el límite máximo que se puede almacenar en la base de datos (32,767 unidades).";
        public const string nfrin = "\n[red]NO fue registrado en la base de datos, el nuevo inventario del producto sería inválido (negativo).";

        public const string nfmfe = "\n[red]NO fue actualizado en la base de datos, el registro fue eliminado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfmfm = "\n[red]NO fue actualizado en la base de datos, el registro fue modificado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfmmd = "\n[red]NO fue actualizado en la base de datos por un motivo desconocido.";
        public const string nfmcqn = "\n[red]NO fue actualizado en la base de datos, su Campo Quantity del detalle de la venta es NULO.\n[gold]El registro pudo haber sido modificado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfmii = "\n[red]NO fue actualizado en la base de datos, el inventario del producto fue insuficiente. \n[gold]El registro de productos pudo haber sido modificado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfmie = "\n[red]NO fue actualizado en la base de datos, el nuevo inventario del producto excedió el límite máximo que se puede almacenar en la base de datos (32,767 unidades).";
        public const string nfmin = "\n[red]NO fue actualizado en la base de datos, el nuevo inventario del producto sería inválido (negativo).";

        public const string oevvd = "\n[red]Ocurrió un error al verificar la versión de los datos de la venta.\n[black]Intente refrescar los datos.";
        public const string fmpousmn = "\n[gold]Fue modificado previamente por otro usuario de la red.\n[green]Se mostrará la nota de remisión con los datos proporcionados por el otro usuario.";
        public const string oed = "\n[red]Ocurrió un error desconocido.\n[black]Intente refrescar los datos.";

        public const string erfep = "\n[orange]El registro fue eliminado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string ses = "\n[green]Se eliminó satisfactoriamente.";
        public const string nfefe = "\n[red]NO fue eliminado en la base de datos, el registro fue eliminado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfefm = "\n[red]NO fue eliminado en la base de datos, el registro fue modificado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfemd = "\n[red]NO fue eliminado en la base de datos por un motivo desconocido.";
        public const string fepou = "\n[red]Fue eliminado previamente por otro usuario de la red.\n[black]Intente refrescar los datos";
        public const string fmpou = "\n[red]Fue modificado previamente por otro usuario de la red.\n[black]Intente refrescar los datos";
        public const string nfecqn = "\n[red]NO fue eliminado en la base de datos, su Campo Quantity del detalle de la venta es NULO.\n[gold]El registro pudo haber sido modificado previamente por otro usuario de la red.\n[black]Intente refrescar los datos.";
        public const string nfeie = "\n[red]NO fue eliminado en la base de datos, el nuevo inventario del producto excedió el límite máximo que se puede almacenar en la base de datos (32,767 unidades).";
        public const string nfein = "\n[red]NO fue eliminado en la base de datos, el nuevo inventario del producto sería inválido (negativo).";

        public const string ecfm = "Este campo fue modificado, guarde el formulario o cierrelo sin confirmar los cambios";
        #endregion

        public static string ComputeSha256Hash(string rawData)
        {
            // 1. Instancia el algoritmo
            using (SHA256 sha256 = SHA256.Create())
            {
                // 2. Convierte la cadena a bytes (UTF-8)
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // 3. Convierte cada byte a su representación hex (2 dígitos)
                var builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                // 4. Retorna el string hex completo
                return builder.ToString();
            }
        }

        public static DateTime? ObtenerFechaHora(DateTimePicker dtpFecha, DateTimePicker dtpHora)
        {
            // Si el 'padre' no está marcado, no importa lo que diga la hora, es NULL
            if (dtpFecha == null || !dtpFecha.Checked)
                return null;

            // Si por alguna razón el control de hora es nulo (poco probable pero posible), 
            // devolvemos solo la fecha a las 00:00:00
            if (dtpHora == null)
                return dtpFecha.Value.Date;

            return dtpFecha.Value.Date.Add(dtpHora.Value.TimeOfDay);
        }
        /// <summary>
        /// Ordena un DataGridView enlazado a una lista genérica y muestra el glyph de ordenamiento.
        /// </summary>
        public static void OrdenarPorColumna<T>(DataGridView dgv, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            var col = dgv.Columns[e.ColumnIndex];
            if (col.SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            string propertyName = col.DataPropertyName;
            var lista = dgv.DataSource as List<T>;
            if (lista == null || string.IsNullOrWhiteSpace(propertyName)) return;
            // para mantener el orden de columnas que definiste por programación
            // Guardar orden actual de columnas (DisplayIndex)
            var ordenColumnas = dgv.Columns.Cast<DataGridViewColumn>()
                .OrderBy(c => c.DisplayIndex)
                .Select(c => c.Name)
                .ToList();
            // Limpia flechitas de todas las columnas ordenables
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                if (c.SortMode == DataGridViewColumnSortMode.Programmatic)
                    c.HeaderCell.SortGlyphDirection = WinFormsSortOrder.None;
            }
            // Alternar orden: primer clic → ASC
            bool asc = dgv.Tag?.ToString() != "ASC";
            Func<T, object> keySelector = p => p.GetType().GetProperty(propertyName)?.GetValue(p);
            dgv.DataSource = asc
                ? lista.OrderBy(keySelector).ToList()
                : lista.OrderByDescending(keySelector).ToList();
            dgv.Tag = asc ? "ASC" : "DESC";
            // para mantener el orden de columnas que definiste por programación
            // Restaurar orden de columnas
            for (int i = 0; i < ordenColumnas.Count; i++)
            {
                dgv.Columns[ordenColumnas[i]].DisplayIndex = i;
            }
            // Mostrar flechita igual que DataTable
            // Reobtener la columna después de que el DataSource se aplicó
            var refreshedCol = dgv.Columns[e.ColumnIndex];
            refreshedCol.HeaderCell.SortGlyphDirection = asc
                ? WinFormsSortOrder.Ascending   // primer clic → flecha arriba
                : WinFormsSortOrder.Descending; // segundo clic → flecha abajo
        }

        /// <summary>
        /// Ordena un DataTable/DataView y muestra el glyph de ordenamiento.
        /// </summary>
        public static void OrdenarPorColumna(DataGridView dgv, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            var col = dgv.Columns[e.ColumnIndex];
            if (col.SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            string propertyName = col.DataPropertyName;
            if (string.IsNullOrWhiteSpace(propertyName)) return;
            // Obtener DataView desde el DataSource
            var dv = dgv.DataSource as DataView;
            if (dv == null)
            {
                var dt = dgv.DataSource as DataTable;
                dv = dt?.DefaultView;
            }
            if (dv == null) return;
            // Guardar orden actual de columnas
            var ordenColumnas = dgv.Columns.Cast<DataGridViewColumn>()
                .OrderBy(c => c.DisplayIndex)
                .Select(c => c.Name)
                .ToList();
            // Limpiar flechitas
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                if (c.SortMode == DataGridViewColumnSortMode.Programmatic)
                    c.HeaderCell.SortGlyphDirection = WinFormsSortOrder.None;
            }
            // Alternar orden
            bool asc = dgv.Tag?.ToString() != "ASC";
            dv.Sort = $"{propertyName} {(asc ? "ASC" : "DESC")}";
            dgv.Tag = asc ? "ASC" : "DESC";
            // Restaurar orden de columnas
            for (int i = 0; i < ordenColumnas.Count; i++)
            {
                dgv.Columns[ordenColumnas[i]].DisplayIndex = i;
            }
            // Mostrar flechita
            col.HeaderCell.SortGlyphDirection = asc
                ? WinFormsSortOrder.Ascending
                : WinFormsSortOrder.Descending;
        }

        public static event Action<Form> FormularioAgregado;

        // Los siguientes tres métodos trabajan juntos para detectar cambios en TextBox y ComboBox,
        // para que funcionen los metodos FormClosing de los formularios de mantenimiento
        // Método recursivo para recorrer todos los controles anidados
        private static IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                yield return ctrl;
                // Recorrer hijos si existen
                foreach (var child in GetAllControls(ctrl))
                    yield return child;
            }
        }

        public static string GetValorOriginal(string controlName, Dictionary<string, object> valoresOriginales)
        {
            return valoresOriginales.TryGetValue(controlName, out var valor)
                ? valor?.ToString() ?? string.Empty
                : string.Empty;
        }

        // Captura valores iniciales SOLO de TextBox y ComboBox,
        // ignorando los que empiezan con txtB o cboB
        public static Dictionary<string, object> CapturarValoresOriginales(Control parent)
        {
            var valores = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (Control ctrl in GetAllControls(parent))
            {
                var name = ctrl.Name;
                if (string.IsNullOrEmpty(name)) continue;

                if (ctrl is TextBox txt)
                {
                    if (!name.StartsWith("txtB", StringComparison.OrdinalIgnoreCase))
                        valores[name] = txt.Text.Trim() ?? string.Empty;
                }
                else if (ctrl is ComboBox cbo)
                {
                    if (!name.StartsWith("cboB", StringComparison.OrdinalIgnoreCase))
                        valores[name] = cbo.SelectedIndex;
                }
                else if (ctrl is CheckBox chk)
                {
                    if (!name.StartsWith("chkbB", StringComparison.OrdinalIgnoreCase))
                        valores[name] = chk.Checked;
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    if (!name.StartsWith("dtpB", StringComparison.OrdinalIgnoreCase))
                        valores[name] = dtp.Value;
                }
                else if (ctrl is NumericUpDown nud)
                {
                    if (!name.StartsWith("nudB", StringComparison.OrdinalIgnoreCase))
                        valores[name] = nud.Value;
                }
            }
            return valores;
        }

        // Compara valores actuales contra los originales
        // Ahora recibe también el ErrorProvider
        public static bool HayCambios(Control parent, Dictionary<string, object> valoresOriginales, ErrorProvider errorProvider)
        {
            bool hayCambios = false;

            // Limpiar errores previos
            errorProvider.Clear();

            foreach (Control ctrl in GetAllControls(parent))
            {
                var name = ctrl.Name;
                if (string.IsNullOrEmpty(name)) continue;

                if (ctrl is TextBox txt)
                {
                    if (name.StartsWith("txtB", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        string actual = txt.Text.Trim();

                        if (!Equals(original, actual))
                        {
                            hayCambios = true;
                            errorProvider.SetError(txt, ecfm);
                        }
                    }
                }
                else if (ctrl is ComboBox cbo)
                {
                    if (name.StartsWith("cboB", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = cbo.SelectedIndex;
                        if (!Equals(original, actual))
                        {
                            hayCambios = true;
                            errorProvider.SetError(cbo, ecfm);
                        }
                    }
                }
                else if (ctrl is CheckBox chk)
                {
                    if (name.StartsWith("chkbB", StringComparison.OrdinalIgnoreCase)) continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = chk.Checked;
                        if (!Equals(original, actual))
                        {
                            hayCambios = true;
                            errorProvider.SetError(chk, ecfm);
                        }
                    }
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    if (name.StartsWith("dtpB", StringComparison.OrdinalIgnoreCase)) continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = dtp.Value;
                        if (!Equals(original, actual))
                        {
                            hayCambios = true;
                            errorProvider.SetError(dtp, ecfm);
                        }
                    }
                }
                else if (ctrl is NumericUpDown nud)
                {
                    if (name.StartsWith("nudB", StringComparison.OrdinalIgnoreCase)) continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = nud.Value;
                        if (!Equals(original, actual))
                        {
                            hayCambios = true;
                            errorProvider.SetError(nud, ecfm);
                        }
                    }
                }
            }
            return hayCambios;
        }

        // solo detecta si hay cambios, no muestra errores en los controles
        public static bool HayCambios(Control parent, Dictionary<string, object> valoresOriginales)
        {
            bool hayCambios = false;

            foreach (Control ctrl in GetAllControls(parent))
            {
                var name = ctrl.Name;
                if (string.IsNullOrEmpty(name)) continue;

                if (ctrl is TextBox txt)
                {
                    if (name.StartsWith("txtB", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = txt.Text.Trim() ?? string.Empty;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
                else if (ctrl is ComboBox cbo)
                {
                    if (name.StartsWith("cboB", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = cbo.SelectedIndex;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
                else if (ctrl is CheckBox chk)
                {
                    if (name.StartsWith("chkbB", StringComparison.OrdinalIgnoreCase)) continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = chk.Checked;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    if (name.StartsWith("dtpB", StringComparison.OrdinalIgnoreCase)) continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = dtp.Value;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
                else if (ctrl is NumericUpDown nud)
                {
                    if (name.StartsWith("nudB", StringComparison.OrdinalIgnoreCase)) continue;

                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = nud.Value;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
            }
            return hayCambios;
        }

        // solo detecta si hay cambios en venta, no muestra errores en los controles
        public static bool HayCambiosEnVenta(Control parent, Dictionary<string, object> valoresOriginales)
        {
            bool hayCambios = false;

            // Lista de controles que sí deben verificarse
            var controlesPermitidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "cboCliente", "cboEmpleado", "dtpVenta", "dtpHoraVenta",
                "dtpRequerido", "dtpHoraRequerido", "dtpEnvio", "dtpHoraEnvio",
                "cboTransportista", "txtDirigidoa", "txtDomicilio", "txtCiudad",
                "txtRegion", "txtCP", "txtPais", "nudFlete"
            };

            foreach (Control ctrl in GetAllControls(parent))
            {
                var name = ctrl.Name;
                if (string.IsNullOrEmpty(name)) continue;

                // Ignorar controles que no estén en la lista
                if (!controlesPermitidos.Contains(name))
                    continue;

                if (ctrl is TextBox txt)
                {
                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = txt.Text.Trim() ?? string.Empty;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
                else if (ctrl is ComboBox cbo)
                {
                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = cbo.SelectedIndex;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = dtp.Value;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
                else if (ctrl is NumericUpDown nud)
                {
                    if (valoresOriginales.TryGetValue(name, out var original))
                    {
                        var actual = nud.Value;
                        if (!Equals(original, actual))
                            hayCambios = true;
                    }
                }
            }
            return hayCambios;
        }

        public static byte[] StripOleHeader(byte[] oleBytes, int employeeId)
        {
            if (oleBytes == null || oleBytes.Length == 0)
                return oleBytes;
            const int OLE_HEADER_LENGTH = 78; // Tamaño típico del encabezado OLE
            int offset = (employeeId <= 9 && oleBytes.Length > OLE_HEADER_LENGTH)
            ? OLE_HEADER_LENGTH
            : 0;

            int length = oleBytes.Length - offset;
            if (length <= 0)
                return Array.Empty<byte>();

            var imageBytes = new byte[length];
            Buffer.BlockCopy(oleBytes, offset, imageBytes, 0, length);
            return imageBytes;
        }

        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;
            using (var clone = new Bitmap(image))
            using (var ms = new MemoryStream())
            {
                clone.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Ajusta los valores de dos NumericUpDown que representan un rango (inicial y final).
        /// Usa el parámetro sender para identificar cuál control disparó el evento.
        /// Reglas:
        /// - Si el evento lo dispara el inicial:
        ///   1. Si inicial = 0 → final = 0
        ///   2. Si inicial > 0 y final = 0 → final = inicial
        ///   3. Si final < inicial → final = inicial
        /// - Si el evento lo dispara el final:
        ///   1. Si final > 0 y inicial = 0 → inicial = final
        ///   2. Si inicial > final → inicial = final
        ///
        /// Con esto se garantiza siempre: 0 ≤ Inicial ≤ Final
        /// </summary>
        /// <param name="sender">Es el parámetro sender para identificar cuál control disparó el evento</param>
        /// <param name="nudIni">NumericUpDown que representa el ID inicial</param>
        /// <param name="nudFin">NumericUpDown que representa el ID final</param>

        public static void ValidarRango(object sender, NumericUpDown nudIni, NumericUpDown nudFin)
        {
            var nudSender = sender as NumericUpDown;
            if (nudSender == null) return;

            if (nudSender == nudIni)
            {
                // Reglas cuando se sale del inicial
                if (nudIni.Value == 0)
                    nudFin.Value = 0;
                else if (nudIni.Value > 0 && nudFin.Value == 0)
                    nudFin.Value = nudIni.Value;
                else if (nudFin.Value < nudIni.Value)
                    nudFin.Value = nudIni.Value;
            }
            else if (nudSender == nudFin)
            {
                // Reglas cuando se sale del final
                if (nudFin.Value > 0 && nudIni.Value == 0)
                    nudIni.Value = nudFin.Value;
                else if (nudIni.Value > nudFin.Value)
                    nudIni.Value = nudFin.Value;
            }
        }

        public static void ValidaTxtBIdIni(TextBox txtBIdIni, TextBox txtBIdFin)
        {
            int numBIdIni = 0, numBIdFin = 0;
            if (txtBIdIni.Text != "")
            {
                if (int.TryParse(txtBIdIni.Text, out int numTxtBIdIni))
                {
                    if (numTxtBIdIni == 0)
                    {
                        MessageBox.Show("El valor del Id inicial no puede ser cero", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBIdIni.Text = "1";
                        txtBIdIni.Focus();
                        return;
                    }
                    numBIdIni = numTxtBIdIni;
                    if (txtBIdFin.Text == "")
                        txtBIdFin.Text = txtBIdIni.Text;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (txtBIdFin.Text != "")
            {
                if (int.TryParse(txtBIdFin.Text, out int numTxtBIdFin))
                {
                    numBIdFin = numTxtBIdFin;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (numBIdFin < numBIdIni)
                txtBIdFin.Text = txtBIdIni.Text;
        }

        public static void ValidaTxtBIdFin(TextBox txtBIdIni, TextBox txtBIdFin)
        {
            int numBIdIni = 0, numBIdFin = 0;
            if (txtBIdIni.Text != "")
            {
                if (int.TryParse(txtBIdIni.Text, out int numTxtBIdIni))
                {
                    numBIdIni = numTxtBIdIni;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtBIdIni.Text = txtBIdFin.Text;
            }
            if (txtBIdFin.Text != "")
            {
                if (int.TryParse(txtBIdFin.Text, out int numTxtBIdFin))
                {
                    if (numTxtBIdFin == 0)
                    {
                        MessageBox.Show("El valor del Id final no puede ser cero", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBIdFin.Text = "1";
                        txtBIdFin.Focus();
                        Utils.ValidaTxtBIdIni(txtBIdIni, txtBIdFin);
                        return;
                    }
                    numBIdFin = numTxtBIdFin;
                }
                else
                    MessageBox.Show("Por favor ingrese un número valido", Utils.nwtr, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (numBIdIni > numBIdFin)
                txtBIdIni.Text = txtBIdFin.Text;
        }

        public static void ValidarDigitosConPunto(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            // valida que exista solo un punto decimal
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
            // forzar que solo se capturen como máximo dos dígitos despues del punto decimal
            if (e.KeyChar != 8)
            {
                string numsDecimales = (sender as TextBox).Text + e.KeyChar;
                if ((sender as TextBox).Text.IndexOf('.') > -1)
                {
                    int posComienzo = (sender as TextBox).Text.IndexOf('.');
                    numsDecimales = numsDecimales.Substring(posComienzo, numsDecimales.Length - posComienzo);
                    if (numsDecimales.Length > 3)
                        e.Handled = true;
                }
            }
        }

        public static void ValidarDigitosSinPunto(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || (int)e.KeyChar == 8);
        }

        /// <summary>
        /// Configuración estándar para DataGridView.
        /// </summary>
        public static void ConfDgv(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //para evitar el parpadeo en el borde inferior del DataGridView suele deberse a problemas de redibujado cuando el contenido toca el límite visual, se modifica la siguiente propiedad
            //ya no es necesaria porque se usa el doble buffer más abajo
            //dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BackgroundColor = SystemColors.GradientInactiveCaption;
            dgv.RowHeadersVisible = false;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            //para evitar el parpadeo en el borde inferior del DataGridView suele deberse a problemas de redibujado cuando el contenido toca el límite visual, se modifica la siguiente propiedad
            //ya no es necesaria porque se usa el doble buffer más abajo
            //dgv.BorderStyle = BorderStyle.None;
            dgv.AutoResizeColumns();

            //para evitar el parpadeo en el borde inferior del DataGridView suele deberse a problemas de redibujado cuando el contenido toca el límite visual.
            // Aquí activamos el doble buffer para todos los DataGridView, reduciendo el parpadeo al mínimo.
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgv, new object[] { true });

            // Configurar SortMode de columnas
            dgv.DataBindingComplete += (s, e) =>
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    // Imagen → no ordenable
                    if (col is DataGridViewImageColumn)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    // Botón → no ordenable
                    else if (col is DataGridViewButtonColumn)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    // CheckBox → solo ordenable si está ligado a datos
                    else if (col is DataGridViewCheckBoxColumn)
                    {
                        if (!string.IsNullOrEmpty(col.DataPropertyName))
                            col.SortMode = DataGridViewColumnSortMode.Programmatic;
                        else
                            col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    // Columnas ligadas a datos → ordenables
                    else if (!string.IsNullOrEmpty(col.DataPropertyName))
                    {
                        col.SortMode = DataGridViewColumnSortMode.Programmatic;
                    }
                    // Todo lo demás → no ordenable
                    else
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
            };
        }

        public static void MsgCatchOue(Exception ex, Action actualizarBarraEstado)
        {
            MsgError(Utils.oue + ex.Message);
            actualizarBarraEstado?.Invoke();
        }

        public static void MsgCatchOueclbdd(SqlException ex, Action actualizarBarraEstado)
        {
            if (ex.Number == 53) // Error de conexión
                MsgError("No se pudo conectar a la base de datos.\n\nVerifique su conexión.");
            else
                MsgError(Utils.oueclbdd + ex.Message);
            actualizarBarraEstado?.Invoke();
        }

        // Métodos específicos que llaman al genérico
        public static void MsgWarning(string mensaje) =>
            MostrarMensaje(mensaje, icono: MessageBoxIcon.Warning);

        public static void MsgExclamation(string mensaje) =>
            MostrarMensaje(mensaje, icono: MessageBoxIcon.Exclamation);

        public static void MsgError(string mensaje) =>
            MostrarMensaje(mensaje, icono: MessageBoxIcon.Error);

        public static void MsgInformation(string mensaje) =>
            MostrarMensaje(mensaje, icono: MessageBoxIcon.Information);

        public static DialogResult MsgQuestion(string mensaje) =>
            MostrarMensaje(mensaje, botones: MessageBoxButtons.YesNo, icono: MessageBoxIcon.Question, defaultButton: MessageBoxDefaultButton.Button2);

        public static DialogResult MsgCerrarForm() =>
            MostrarMensaje(preguntaCerrar, botones: MessageBoxButtons.YesNo, icono: MessageBoxIcon.Question, defaultButton: MessageBoxDefaultButton.Button2);
        // Método genérico
        public static DialogResult MostrarMensaje(
            string mensaje,
            string titulo = null,
            MessageBoxButtons botones = MessageBoxButtons.OK,
            MessageBoxIcon icono = MessageBoxIcon.Information,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            return MessageBox.Show(
                mensaje,
                titulo ?? nwtr,
                botones,
                icono,
                defaultButton
            );
        }

        public static void AgregarFormularioEnTab(TabControl tabControl, Form formulario, string titulo)
        {
            // Buscar si ya existe una pestaña con ese título
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Text == titulo)
                {
                    // Si ya existe, seleccionarla y salir
                    tabControl.SelectedTab = page;
                    return;
                }
            }
            // Crear una nueva pestaña
            TabPage nuevaPestaña = new TabPage(titulo);
            nuevaPestaña.ToolTipText = titulo;

            // Asignar ícono por defecto
            if (tabControl.ImageList != null && tabControl.ImageList.Images.ContainsKey("tabIcon"))
                nuevaPestaña.ImageKey = "tabIcon";

            // Configurar el formulario para incrustarlo
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            // Agregar el formulario a la pestaña
            nuevaPestaña.Controls.Add(formulario);
            // Agregar la pestaña al TabControl
            tabControl.TabPages.Add(nuevaPestaña);
            // Seleccionar la pestaña recién agregada
            tabControl.SelectedTab = nuevaPestaña;
            // Mostrar el formulario incrustado
            // Disparar evento
            FormularioAgregado?.Invoke(formulario);
            formulario.Show();

            // Ajustar tamaños si el control tiene el método AjustarTabSizes
            var metodo = tabControl.GetType().GetMethod("AjustarTabSizes",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            if (metodo != null)
                metodo.Invoke(tabControl, null);
        }

        public static void DibujarPestañas(TabControl tabControl, DrawItemEventArgs e)
        {
            TabPage page = tabControl.TabPages[e.Index];
            bool isSelected = (e.Index == tabControl.SelectedIndex);

            Color backColor = isSelected ? SystemColors.Highlight : SystemColors.GradientActiveCaption;
            Color textColor = isSelected ? SystemColors.HighlightText : SystemColors.ActiveCaptionText;
            Font textFont = isSelected ? new Font(e.Font, FontStyle.Italic) : e.Font;

            Rectangle rect = e.Bounds;
            using (SolidBrush brush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(brush, rect);

            // Texto
            TextRenderer.DrawText(e.Graphics, page.Text, textFont,
                                  new Point(rect.Left + 4, rect.Top + (rect.Height - e.Font.Height) / 2),
                                  textColor);

            // Ícono (normal o verde según selección)
            string key = isSelected ? "tabIconOn" : "tabIcon";
            if (tabControl.ImageList != null && tabControl.ImageList.Images.ContainsKey(key))
            {
                Image img = tabControl.ImageList.Images[key];
                int iconX = rect.Right - 16 - 5; // margen de 5px
                int iconY = rect.Top + (rect.Height - 16) / 2 + 2; // +2 para centrar mejor verticalmente
                e.Graphics.DrawImage(img, iconX, iconY, 11, 14);
            }
        }

        public static void ConfigurarTabControl(TabControl tabControl, Image iconOff, Image iconOn)
        {
            // Configuración básica
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Appearance = TabAppearance.Normal;
            tabControl.SizeMode = TabSizeMode.Fixed;

            // Configuración de íconos
            tabControl.ImageList = new ImageList();
            tabControl.ImageList.ImageSize = new Size(11, 14);
            tabControl.ImageList.Images.Add("tabIcon", iconOff);
            tabControl.ImageList.Images.Add("tabIconOn", iconOn);           

            foreach (TabPage page in tabControl.TabPages)
                page.ImageKey = "tabIcon";

            int maxWidth = 0;
            foreach (TabPage page in tabControl.TabPages)
            {
                Size textSize = TextRenderer.MeasureText(page.Text, tabControl.Font);
                int width = textSize.Width + 12 + 10;
                if (width > maxWidth) maxWidth = width;
            }
            tabControl.ItemSize = new Size(maxWidth, tabControl.ItemSize.Height);

            // Asociar evento de dibujo centralizado
            tabControl.DrawItem += (s, e) => DibujarPestañas(tabControl, e);

            // Buscar manejadores en el formulario contenedor
            var form = tabControl.FindForm();
            if (form != null)
            {
                var type = form.GetType();

                // Selected
                var selectedMethod = type.GetMethod("tabcOperacion_Selected",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (selectedMethod != null)
                {
                    var handler = (TabControlEventHandler)Delegate.CreateDelegate(
                        typeof(TabControlEventHandler), form, selectedMethod);

                    tabControl.Selected -= handler;
                    tabControl.Selected += handler;
                }

                var selectingMethod = type.GetMethod("tabcOperacion_Selecting",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (selectingMethod != null)
                {
                    var handler = (TabControlCancelEventHandler)Delegate.CreateDelegate(
                        typeof(TabControlCancelEventHandler), form, selectingMethod);

                    tabControl.Selecting -= handler; // evita duplicado
                    tabControl.Selecting += handler;
                }

                // DrawItem
                var drawItemMethod = type.GetMethod("tabcOperacion_DrawItem",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (drawItemMethod != null)
                {
                    var handler = (DrawItemEventHandler)Delegate.CreateDelegate(
                        typeof(DrawItemEventHandler), form, drawItemMethod);

                    tabControl.DrawItem -= handler;
                    tabControl.DrawItem += handler;
                }
            }
        }

        public static void CerrarTodasLasPestañas(TabControl tabControl)
        {
            // Usamos una lista temporal para las páginas que sí se pueden quitar
            var paginasParaQuitar = new List<TabPage>();
            foreach (TabPage page in tabControl.TabPages)
            {
                Form form = null;
                foreach (Control ctrl in page.Controls)
                {
                    if (ctrl is Form f)
                    {
                        form = f;
                        f.Close(); // dispara FormClosing
                        break;
                    }
                }
                // Solo marcamos la página para quitar si el form se cerró de verdad
                if (form == null || form.IsDisposed)
                {
                    paginasParaQuitar.Add(page);
                }
            }
            // Ahora sí quitamos solo las pestañas que se cerraron
            foreach (var page in paginasParaQuitar)
            {
                tabControl.TabPages.Remove(page);
            }
        }

        public static void CerrarPestañaSeleccionada(TabControl tabControl)
        {
            if (tabControl.SelectedTab != null)
            {
                Form form = null;
                foreach (Control ctrl in tabControl.SelectedTab.Controls)
                {
                    if (ctrl is Form f)
                    {
                        form = f;
                        f.Close();
                        break;
                    }
                }
                // Solo quitar la pestaña si el formulario se cerró de verdad
                if (form == null || form.IsDisposed)
                {
                    tabControl.TabPages.Remove(tabControl.SelectedTab);
                }
            }
        }

        public static void OpenForm<T>(Form mdiParent) where T : Form, new()
        {
            T newForm = new T
            {
                MdiParent = mdiParent,
                WindowState = FormWindowState.Maximized
            };
            newForm.Show();
        }

        public static void CloseForms(Action actualizarBarraDeEstado)
        {
            //Declaramos una lista de tipo Form
            List<Form> formularios = new List<Form>();
            //Recorremos Application.OpenForms el cual tiene la lista de formularios y metemos todos los forms en la lista que declarmos
            foreach (Form form in Application.OpenForms)
                formularios.Add(form);
            // recorremos la lista de formularios
            for (int i = 0; i < formularios.Count; i++)
            {
                // validamos que el nombre de los formularios sean distintos al unico formulario que queremos abierto
                if (formularios[i].Name != "MDIPrincipal")
                    formularios[i].Close();
                else
                    //MDIPrincipal.ActualizarBarraDeEstado();
                    actualizarBarraDeEstado?.Invoke();
            }
        }

        // Método para pintar GroupBox con borde negro y texto negro
        public static void GrbPaint(Form form, object sender, PaintEventArgs e)
        {
            if (sender is GroupBox groupBox)
                DrawGroupBox(form, groupBox, e.Graphics, Color.Black, SystemColors.Desktop, Color.White);
        }

        // Método para pintar GroupBox con borde gris y texto negro
        public static void GrbPaint2(Form form, object sender, PaintEventArgs e)
        {
            if (sender is GroupBox groupBox)
                DrawGroupBox(form, groupBox, e.Graphics, Color.LightSlateGray, Color.LightSeaGreen, Color.White);
        }

        // Método genérico para dibujar cualquier GroupBox
        public static void DrawGroupBox(Form form, GroupBox box, Graphics g,
                                        Color borderColor, Color textBackColor, Color textColor,
                                        int radius = 30)
        {
            if (box == null) return;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Medir el texto
            SizeF strSize = g.MeasureString(box.Text, box.Font);
            int textStart = box.Padding.Left;

            // Rectángulo principal del GroupBox
            Rectangle rect = new Rectangle(
                box.ClientRectangle.X,
                box.ClientRectangle.Y + (int)(strSize.Height / 2),
                box.ClientRectangle.Width - 1,
                box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

            g.Clear(form.BackColor);

            int textRadius = 10;   // radio de las esquinas del rectángulo del texto
            int textPadding = 7;  // espacio en blanco antes y después del texto

            // Rectángulo detrás del texto con padding horizontal
            Rectangle textRect = new Rectangle(
                textStart + 20,
                0,
                (int)strSize.Width + (textPadding * 2),
                (int)strSize.Height
            );

            // Fondo con esquinas redondeadas
            using (GraphicsPath textPath = new GraphicsPath())
            {
                textPath.AddArc(textRect.X, textRect.Y, textRadius, textRadius, 180, 90); // sup. izq
                textPath.AddArc(textRect.Right - textRadius, textRect.Y, textRadius, textRadius, 270, 90); // sup. der
                textPath.AddArc(textRect.Right - textRadius, textRect.Bottom - textRadius, textRadius, textRadius, 0, 90); // inf. der
                textPath.AddArc(textRect.X, textRect.Bottom - textRadius, textRadius, textRadius, 90, 90); // inf. izq
                textPath.CloseFigure();

                using (Brush backBrush = new SolidBrush(textBackColor))
                    g.FillPath(backBrush, textPath);

                using (Pen borderPenText = new Pen(Color.Black, 1))
                    g.DrawPath(borderPenText, textPath);
            }

            // Texto encima con padding interno
            using (Brush textBrush = new SolidBrush(textColor))
                g.DrawString(box.Text, box.Font, textBrush, textRect.X + textPadding, textRect.Y);

            using (Pen borderPen = new Pen(borderColor, 1.5f))
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                // Esquina sup. izq
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);

                // Línea superior hasta inicio del texto
                path.AddLine(rect.X + radius - 10, rect.Y, textRect.X, rect.Y);

                // Salto del texto
                path.StartFigure();

                // Línea superior después del texto
                path.AddLine(textRect.Right, rect.Y, rect.Right - radius, rect.Y);

                // Esquina sup. der
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);

                // Lado derecho
                path.AddLine(rect.Right, rect.Y + radius, rect.Right, rect.Bottom - radius);

                // Esquina inf. der
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);

                // Línea inferior
                path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);

                // Esquina inf. izq
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);

                // Lado izquierdo
                path.AddLine(rect.X, rect.Bottom - radius, rect.X, rect.Y + radius - 14);

                g.DrawPath(borderPen, path);

                //// Rellenar el área interna en blanco
                //using (Brush whiteBrush = new SolidBrush(Color.White))
                //    g.FillPath(whiteBrush, path);

                //// Dibujar el borde encima
                //g.DrawPath(borderPen, path);
            }
        }
    }
}
