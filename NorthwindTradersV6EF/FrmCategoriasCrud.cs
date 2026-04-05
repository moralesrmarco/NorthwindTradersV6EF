using BLL.EF;
using DAL.EF;
using DTOs.EF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF
{
    public partial class FrmCategoriasCrud : Form
    {

        private bool EjecutarConfDgv = true;
        internal Dictionary<string, object> valoresOriginales;
        OpenFileDialog openFileDialog;

        public FrmCategoriasCrud()
        {
            InitializeComponent();
        }

        private void GrbPaint(object sender, PaintEventArgs e) => Utils.GrbPaint(this, sender, e);

        private void FrmCategoriasCrud_FormClosed(object sender, FormClosedEventArgs e) => MDIPrincipal.ActualizarBarraDeEstado();

        internal void FrmCategoriasCrud_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrar) == DialogResult.No)
                    e.Cancel = true;
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            splitContainer1.SplitterDistance = 250;
        }

        private void tabcOperacion_DrawItem(object sender, DrawItemEventArgs e) => Utils.DibujarPestañas(sender as TabControl, e);

        private void FrmCategoriasCrud_Load(object sender, EventArgs e)
        {
            // Establecer el tamaño inicial
            splitContainer1.SplitterDistance = 250;
            // Asociar el evento
            splitContainer1.SplitterMoved += new SplitterEventHandler(splitContainer1_SplitterMoved);

            tabcOperacion.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabcOperacion.DrawItem += tabcOperacion_DrawItem;
            DeshabilitarControles();
            Utils.ConfDgv(Dgv);
            LlenarDgv(false);
            CargarValoresOriginales();
        }

        private void DeshabilitarControles()
        {
            txtCategoria.ReadOnly = txtDescripcion.ReadOnly = true;
            picFoto.Enabled = false;
        }

        private void HabilitarControles()
        {
            txtCategoria.ReadOnly = txtDescripcion.ReadOnly = false;
            picFoto.Enabled = true;
        }

        private void LlenarDgv(bool selectorRealizaBusqueda)
        {
            try
            {
                MDIPrincipal.ActualizarBarraDeEstado(Utils.clbdd);
                DtoCategoriasBuscar criterios = new DtoCategoriasBuscar()
                {
                    IdIni = string.IsNullOrWhiteSpace(txtBIdIni.Text) ? 0 : int.Parse(txtBIdIni.Text),
                    IdFin = string.IsNullOrWhiteSpace(txtBIdFin.Text) ? 0 : int.Parse(txtBIdFin.Text),
                    CategoryName = txtBCategoria.Text.Trim()
                };
                // La siguientes 2 instrucciones son necesarias para poder manejar el LlenarDgv sin realizar una recarga del registro de la categoria, como en los otros metodos de los demas FrmCrud...
                // cambia mucho la estructuración, ojo no modificar.
                var categorias = CategoryBLL.ObtenerCategorias(selectorRealizaBusqueda, criterios, false);
                var categoriasSinRowVersionTimeStamp = categorias
                                                        .Select(c => new DtoCategoriaDgv
                                                        {
                                                            CategoryID = c.CategoryID,
                                                            CategoryName = c.CategoryName,
                                                            Description = c.Description,
                                                            Picture = c.Picture,
                                                            RowVersionStr = c.RowVersionStr
                                                        })
                                                        .ToList();
                Dgv.DataSource = categoriasSinRowVersionTimeStamp;
                if (EjecutarConfDgv)
                {
                    ConfDgv();
                    EjecutarConfDgv = false;
                }
                if (selectorRealizaBusqueda)
                    MDIPrincipal.ActualizarBarraDeEstado($"Se encontraron {Dgv.RowCount} registro(s)");
                else
                    MDIPrincipal.ActualizarBarraDeEstado($"Se muestran las últimas {Dgv.RowCount} categoría(s) registrada(s)");
            }
            catch (Exception ex)
            {
                U.MsgCatchOue(ex);
            }
        }

        private void ConfDgv()
        {
            Dgv.Columns["RowVersionStr"].Visible = false;

            Dgv.Columns["CategoryID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Dgv.Columns["CategoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            Dgv.Columns["Picture"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Dgv.Columns["Picture"].Width = 80;
            Dgv.RowTemplate.Height = 80;
            Dgv.Columns["Picture"].DefaultCellStyle.Padding = new Padding(4);
            ((DataGridViewImageColumn)Dgv.Columns["Picture"]).ImageLayout = DataGridViewImageCellLayout.Zoom;

            Dgv.Columns["CategoryID"].HeaderText = "Id";
            Dgv.Columns["CategoryName"].HeaderText = "Categoría";
            Dgv.Columns["Description"].HeaderText = "Descripción";
            Dgv.Columns["Picture"].HeaderText = "Foto";
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            BorrarDatosCategoria();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(true);
            CargarValoresOriginales();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            BorrarDatosBusqueda();
            BorrarDatosCategoria();
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
                DeshabilitarControles();
            LlenarDgv(false);
            CargarValoresOriginales();
        }

        private void BorrarMensajesError() => errorProvider1.Clear();

        private void BorrarDatosBusqueda()
        {
            txtBCategoria.Text = txtBIdIni.Text = txtBIdFin.Text = "";
        }

        private void BorrarDatosCategoria()
        {
            txtCategoria.Text = txtDescripcion.Text = txtId.Text = "";
            picFoto.Image = null;
            picFoto.BackgroundImage = Properties.Resources.Categorias;
        }

        void txtBId_KeyPress(object sender, KeyPressEventArgs e) => Utils.ValidarDigitosSinPunto(sender, e);

        private void txtBId_Enter(object sender, EventArgs e) => ((TextBox)sender).SelectAll();

        void txtBId_Leave(object sender, EventArgs e)
        {
            // Castear el objeto que disparó el evento
            TextBox tb = sender as TextBox;
            if (tb == null) return; // seguridad
            if (tb == txtBIdIni)
                Utils.ValidaTxtBIdIni(txtBIdIni, txtBIdFin);
            else if (tb == txtBIdFin)
                Utils.ValidaTxtBIdFin(txtBIdIni, txtBIdFin);
        }

        private bool ValidarControles()
        {
            bool valida = true;
            if (txtCategoria.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtCategoria, "Ingrese la categoría");
            }
            if (txtDescripcion.Text.Trim() == "")
            {
                valida = false;
                errorProvider1.SetError(txtDescripcion, "Ingrese la descripción");
            }
            if (picFoto.Image == null)
            {
                valida = false;
                errorProvider1.SetError(btnCargar, "Ingrese la imagen");
            }
            return valida;
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab != tbpRegistrar)
            {
                DeshabilitarControles();
                DataGridViewRow dgvr = Dgv.CurrentRow;
                txtId.Text = dgvr.Cells["CategoryId"].Value.ToString();
                txtCategoria.Text = dgvr.Cells["CategoryName"].Value.ToString();
                txtDescripcion.Text = dgvr.Cells["Description"].Value.ToString();
                if (dgvr.Cells["Picture"].Value != DBNull.Value)
                {
                    byte[] foto = (byte[])dgvr.Cells["Picture"].Value;
                    MemoryStream ms;
                    if (int.Parse(txtId.Text) <= 8)
                    {
                        ms = new MemoryStream(foto, 78, foto.Length - 78);
                        btnCargar.Enabled = false; // no se permite modificar porque desconozco el formato de la imagen
                    }
                    else
                    {
                        ms = new MemoryStream(foto);
                        btnCargar.Enabled = true;
                    }
                    picFoto.Image = Image.FromStream(ms);
                    picFoto.BackgroundImage = null;
                }
                else
                {
                    picFoto.Image = null;
                    picFoto.BackgroundImage = Properties.Resources.Categorias;
                }
                txtId.Tag = dgvr.Cells["RowVersionStr"].Value;
                if (tabcOperacion.SelectedTab == tbpConsultar)
                {
                    btnCargar.Visible = false;
                    btnOperacion.Visible = false;
                }
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    HabilitarControles();
                    btnOperacion.Visible = true;
                    btnCargar.Visible = true;
                    btnOperacion.Enabled = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnCargar.Visible = false;
                    btnOperacion.Visible = true;
                    btnOperacion.Enabled = true;
                }
            }
            CargarValoresOriginales();
        }

        private void Dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // debe estar vinculado a la clase List<> a la cual esta vinculado el DataGridView.DataSource
            Utils.OrdenarPorColumna<Category>(Dgv, e);
        }

        void ActualizaDgv() => btnLimpiar.PerformClick();

        private void tabcOperacion_Selected(object sender, TabControlEventArgs e)
        {
            BorrarDatosCategoria();
            BorrarMensajesError();
            picFoto.BackgroundImage = Properties.Resources.Categorias;
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                Dgv.CellClick -= new DataGridViewCellEventHandler(Dgv_CellClick);
                Dgv.CellClick -= new DataGridViewCellEventHandler(Dgv_CellClick);
                HabilitarControles();
                BorrarDatosBusqueda();
                btnOperacion.Text = "Registrar categoría";
                btnOperacion.Visible = true;
                btnOperacion.Enabled = true;
                btnCargar.Visible = true;
                btnCargar.Enabled = true;
            }
            else
            {
                Dgv.CellClick -= new DataGridViewCellEventHandler(Dgv_CellClick);
                Dgv.CellClick += new DataGridViewCellEventHandler(Dgv_CellClick);
                DeshabilitarControles();
                btnOperacion.Enabled = false;
                btnCargar.Enabled = false;
                if (tabcOperacion.SelectedTab == tbpConsultar)
                {
                    btnOperacion.Visible = false;
                    btnCargar.Visible = false;
                }
                else if (tabcOperacion.SelectedTab == tbpModificar)
                {
                    btnOperacion.Text = "Modificar categoría";
                    btnOperacion.Enabled = false;
                    btnOperacion.Visible = true;
                    btnCargar.Visible = true;
                }
                else if (tabcOperacion.SelectedTab == tbpEliminar)
                {
                    btnOperacion.Text = "Eliminar categoría";
                    btnOperacion.Enabled = false;
                    btnOperacion.Visible = true;
                    btnCargar.Visible = false;
                }
            }
            CargarValoresOriginales();
        }

        private void btnOperacion_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();
            if (tabcOperacion.SelectedTab == tbpRegistrar)
            {
                if (ValidarControles())
                {
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.insertandoRegistro);
                    DeshabilitarControles();
                    btnOperacion.Enabled = false;
                    btnCargar.Enabled = false;
                    byte[] fileFoto = null;
                    Image image = picFoto.Image;
                    ImageConverter converter = new ImageConverter();
                    fileFoto = (byte[])converter.ConvertTo(image, typeof(byte[]));
                    try
                    {
                        var categoria = new Category
                        {
                            CategoryName = txtCategoria.Text.Trim(),
                            Description = txtDescripcion.Text.Trim(),
                            Picture = fileFoto
                        };
                        int numRegs = CategoryBLL.Insertar(categoria);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se insertaron {numRegs} registro(s)");
                        string idyNombreCategoria = $"La categoría con Id: {txtId.Text} - Nombre de categoria: {txtCategoria.Text}:";
                        if (numRegs > 0)
                        {
                            txtId.Text = categoria.CategoryID.ToString();
                            idyNombreCategoria = $"La categoría con Id: {txtId.Text} - Nombre de categoria: {txtCategoria.Text}:";
                            U.NotificacionInformation(idyNombreCategoria + Utils.srs);
                        }
                        else
                            U.NotificacionError(idyNombreCategoria + Utils.nfrs);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    HabilitarControles();
                    btnOperacion.Enabled = true;
                    btnCargar.Enabled = true;
                    picFoto.BackgroundImage = Properties.Resources.Categorias;
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpModificar)
            {
                // Verificar si hubo cambios en el formulario
                if (!Utils.HayCambios(this, valoresOriginales))
                {
                    U.NotificacionWarning(Utils.ndc);
                    return; // Salir sin hacer UPDATE
                }

                // aqui no manejo la concurrencia con el entity framework, modo de simultaneidad = none, por que la concurrencia la maneja el stored procedure
                if (ValidarControles())
                {
                    btnOperacion.Enabled = false;
                    btnCargar.Enabled = false;
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.modificandoRegistro);
                    DeshabilitarControles();
                    byte[] fileFoto = null;
                    Image image = picFoto.Image;
                    ImageConverter converter = new ImageConverter();
                    fileFoto = (byte[])converter.ConvertTo(image, typeof(byte[]));
                    try
                    {
                        var categoria = new Category
                        {
                            CategoryID = int.Parse(txtId.Text),
                            CategoryName = txtCategoria.Text.Trim(),
                            Description = txtDescripcion.Text.Trim(),
                            Picture = fileFoto,
                            RowVersionStr = txtId.Tag.ToString() // reconstruye el byte[] a partir del string para enviarlo al BLL, y lo asigna al RowVersion del objeto categoria, ojo no eliminar esta linea porque es necesaria para el manejo de la concurrencia optimista, si se elimina se pierde la funcionalidad de la concurrencia optimista y se podrían presentar problemas de actualización sin que el usuario se dé cuenta, como por ejemplo que se sobreescriban cambios realizados por otro usuario sin que se muestre un mensaje de error indicando que hubo un conflicto de concurrencia.
                        };
                        int numRegs = CategoryBLL.Actualizar(categoria);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se actualizaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string idyNombreCategoria = $"La categoría con Id: {txtId.Text} - Nombre de categoría: {txtCategoria.Text}:";
                        if (numRegs > 0)
                            U.NotificacionInformation(idyNombreCategoria + Utils.sms);
                        else if (numRegs == 0)
                            U.NotificacionWarning(idyNombreCategoria + Utils.ndc);
                        else if (numRegs == -1)
                            U.NotificacionError(idyNombreCategoria + Utils.nfmfe);
                        else if (numRegs == -2)
                            U.NotificacionError(idyNombreCategoria + Utils.nfmfm);
                        else
                            U.NotificacionError(idyNombreCategoria + Utils.nfmmd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    picFoto.BackgroundImage = Properties.Resources.Categorias;
                    ActualizaDgv();
                }
            }
            else if (tabcOperacion.SelectedTab == tbpEliminar)
            {
                if (U.NotificacionQuestion($"[orange]¿Esta seguro de eliminar la categoría con Id: {txtId.Text} - Nombre de categoría: {txtCategoria.Text}?") == DialogResult.Yes)
                {
                    btnOperacion.Enabled = false;
                    MDIPrincipal.ActualizarBarraDeEstado(Utils.eliminandoRegistro);
                    try
                    {
                        var categoria = new Category
                        {
                            CategoryID = int.Parse(txtId.Text),
                            RowVersionStr = txtId.Tag.ToString() // reconstruye el byte[] a partir del string para enviarlo al BLL, y lo asigna al RowVersion del objeto categoria, ojo no eliminar esta linea porque es necesaria para el manejo de la concurrencia optimista, si se elimina se pierde la funcionalidad de la concurrencia optimista y se podrían presentar problemas de eliminación sin que el usuario se dé cuenta, como por ejemplo que se eliminen registros que otro usuario haya modificado sin que se muestre un mensaje de error indicando que hubo un conflicto de concurrencia.
                        };
                        int numRegs = CategoryBLL.Eliminar(categoria);
                        MDIPrincipal.ActualizarBarraDeEstado($"Se eliminaron {(numRegs < 0 ? 0 : numRegs)} registro(s)");
                        string idyNombreCategoria = $"La categoría con Id: {txtId.Text} - Nombre de categoría: {txtCategoria.Text}:";
                        if (numRegs > 0)
                            U.NotificacionInformation(idyNombreCategoria + Utils.ses);
                        else if (numRegs == 0)
                            U.NotificacionWarning(idyNombreCategoria + Utils.ndc);
                        else if (numRegs == -1)
                            U.NotificacionError(idyNombreCategoria + Utils.nfefe);
                        else if (numRegs == -2)
                            U.NotificacionError(idyNombreCategoria + Utils.nfefm);
                        else
                            U.NotificacionError(idyNombreCategoria + Utils.nfemd);
                    }
                    catch (Exception ex)
                    {
                        U.MsgCatchOue(ex);
                    }
                    picFoto.BackgroundImage = Properties.Resources.Categorias;
                    ActualizaDgv();
                }
            }
            CargarValoresOriginales();
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            // Mostrar el cuadro de diálogo OpenFileDialog
            //La instrucción siguiente es para que nos muestre todos los tipos juntos
            openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Archivos de imagen (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.InitialDirectory = "c:\\Imágenes\\";
            //La instrucción siguiente es para que nos muestre varias filas en el openfiledialog que nos permita abrir por un tipo especifico
            openFileDialog.Filter = "Archivos jpg (*.jpg)|*.jpg|Archivos jpeg (*.jpeg)|*.jpeg|Archivos png (*.png)|*.png|Archivos bmp (*.bmp)|*.bmp";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Cargar la imagen seleccionada en un objeto Image
                Image image = Image.FromFile(openFileDialog.FileName);

                // Mostrar la imagen en un control PictureBox
                picFoto.Image = image;
                picFoto.BackgroundImage = null;
                errorProvider1.SetError(btnCargar, "");
            }
        }

        private void CargarValoresOriginales()
        {
            // Captura inicial usando la utilidad
            valoresOriginales = Utils.CapturarValoresOriginales(this);
        }

        private void tabcOperacion_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Utils.HayCambios(this, valoresOriginales, errorProvider1))
                if (U.NotificacionQuestion(Utils.preguntaCerrarPestaña) == DialogResult.No)
                    e.Cancel = true;
        }
    }
}
