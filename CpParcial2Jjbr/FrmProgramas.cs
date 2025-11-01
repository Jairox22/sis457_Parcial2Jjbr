using CadParcial2Jjbr;
using ClnParcial2Jjbr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpParcial2Jjbr
{
    public partial class FrmProgramas : Form
    {
        private bool esNuevo = false;
        public FrmProgramas()
        {
            InitializeComponent();
        }
        private void listar()
        {
            var lista = ProgramaCln.listarPa(txtParametro.Text.Trim());
            dgvLista.DataSource = lista;
            dgvLista.Columns["id"].Visible = false;
            dgvLista.Columns["idCanal"].Visible = false;
            dgvLista.Columns["estadoRegistro"].Visible = false;

            dgvLista.Columns["titulo"].HeaderText = "Título";
            dgvLista.Columns["descripcion"].HeaderText = "Descripción";
            dgvLista.Columns["duracion"].HeaderText = "Duración (min)";
            dgvLista.Columns["productor"].HeaderText = "Productor";
            dgvLista.Columns["fechaEstreno"].HeaderText = "Fecha Estreno";
            dgvLista.Columns["estado"].HeaderText = "Estado";
            dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario Registro";
            dgvLista.Columns["fechaRegistro"].HeaderText = "Fecha Registro";

            dgvLista.Columns["nombreCanal"].HeaderText = "Canal";

            if (lista.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["Titulo"];
            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }
        private void cargarCanal()
        {
            var lista = CanalCln.listar();
            cbxCanal.DataSource = lista;
            cbxCanal.ValueMember = "id";
            cbxCanal.DisplayMember = "nombre";
            cbxCanal.SelectedIndex = -1;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmProgramas_Load(object sender, EventArgs e)
        {
            Size = new Size(1050, 650);
            listar();
            cargarCanal();

            cbxEstado.DataSource = new List<KeyValuePair<int, string>> {
            new KeyValuePair<int, string>(1, "En emisión"),
            new KeyValuePair<int, string>(2, "Finalizado"),
            new KeyValuePair<int, string>(3, "Suspendido")
            };
            cbxEstado.DisplayMember = "Value";
            cbxEstado.ValueMember = "Key";
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            limpiar(); // ← importante, limpia todo el formulario
            txtTitulo.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var programa = ProgramaCln.obtenerUno(id);

            cbxCanal.SelectedValue = programa.idCanal;
            txtTitulo.Text = programa.titulo;
            txtDescripcion.Text = programa.descripcion;
            nudDuracion.Value = (decimal)programa.duracion;
            txtDirector.Text = programa.productor;
            dtpFecha.Value = (DateTime)programa.fechaEstreno;
            cbxEstado.SelectedValue = programa.estado != null ? programa.estado : 1;

            txtTitulo.Focus();
        }
        private void limpiar()
        {
            txtTitulo.Clear();
            txtDescripcion.Clear();
            cbxCanal.SelectedIndex = -1;
            nudDuracion.Value = 60;
            txtDirector.Clear();
            dtpFecha.Value = DateTime.Now;
            cbxEstado.SelectedIndex = -1;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            plnAcciones.Enabled = true;
            limpiar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }
        public bool validar()
        {
            bool esValido = true;

            erpTitulo.Clear();
            erpCanal.Clear();
            erpDescripcion.Clear();
            erpDuracion.Clear();
            erpProductor.Clear();
            erpFechaEstreno.Clear();
            erpEstado.Clear();

            if (string.IsNullOrEmpty(txtTitulo.Text))
            {
                erpTitulo.SetError(txtTitulo, "El título es obligatorio");
                esValido = false;
            }

            if (cbxCanal.SelectedIndex == -1)
            {
                erpCanal.SetError(cbxCanal, "Debe seleccionar un canal");
                esValido = false;
            }

            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                erpDescripcion.SetError(txtDescripcion, "La descripción es obligatoria");
                esValido = false;
            }

            if (nudDuracion.Value == 0)
            {
                erpDuracion.SetError(nudDuracion, "La duración debe ser mayor a cero");
                esValido = false;
            }

            if (string.IsNullOrEmpty(txtDirector.Text))
            {
                erpProductor.SetError(txtDirector, "El productor es obligatorio");
                esValido = false;
            }

            if (dtpFecha.Value == null)
            {
                erpFechaEstreno.SetError(dtpFecha, "Debe seleccionar fecha de estreno");
                esValido = false;
            }

            if (cbxEstado.SelectedIndex == -1)
            {
                erpEstado.SetError(cbxEstado, "Debe seleccionar un estado");
                esValido = false;
            }

            return esValido;
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (validar())
            {
                var programa = new Programa();
                programa.titulo = txtTitulo.Text.Trim();
                programa.descripcion = txtDescripcion.Text.Trim();
                programa.idCanal = (int)cbxCanal.SelectedValue;
                programa.duracion = (int)nudDuracion.Value;
                programa.productor = txtDirector.Text.Trim();
                programa.fechaEstreno = dtpFecha.Value;
                programa.usuarioRegistro = "admin";

                programa.estado = Convert.ToInt16(cbxEstado.SelectedValue);

                programa.estadoRegistro = 1; // Activo

                if (esNuevo)
                {
                    programa.fechaRegistro = DateTime.Now;
                    programa.estadoRegistro = 1; // Activo
                    ProgramaCln.insertar(programa);
                }
                else
                {
                    programa.id = (int)dgvLista.CurrentRow.Cells["id"].Value;
                    ProgramaCln.actualizar(programa);
                }

                listar();
                btnCancelar.PerformClick();

                MessageBox.Show("Programa guardado correctamente",
                    "::: Mensaje - Parcial2Mcv :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null || dgvLista.CurrentRow.Cells["id"].Value == null)
            {
                MessageBox.Show("Seleccione un programa para eliminar.",
                                "::: Mensaje - Parcial2Mcv :::",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            string titulo = dgvLista.CurrentRow.Cells["titulo"].Value?.ToString() ?? "(sin título)";

            var confirm = MessageBox.Show(
                $"¿Está seguro de eliminar el programa '{titulo}'?",
                "::: Confirmar eliminación :::",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                ProgramaCln.eliminar(id, "admin");

                listar();

                MessageBox.Show(
                    $"El programa '{titulo}' fue eliminado correctamente.",
                    "::: Mensaje - Parcial2Mcv :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al eliminar el programa:\n" + ex.Message,
                    "::: Error :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
        }
        /*private void cargarCanal()
        {
            var lista = CanalCln.listar();
            cbxCanal.DataSource = lista;
            cbxCanal.ValueMember = "id";
            cbxCanal.DisplayMember = "nombre";
            cbxCanal.SelectedIndex = -1;
        }*/
    }
}
