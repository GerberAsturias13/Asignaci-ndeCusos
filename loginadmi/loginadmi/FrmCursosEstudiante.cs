using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace loginadmi
{
    public partial class FrmCursosEstudiante : Form
    {
        public FrmCursosEstudiante()
        {
            InitializeComponent();
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            FrmHomeEstudiantes nuevoFormulario = new FrmHomeEstudiantes();
            nuevoFormulario.Show();
            this.Hide(); 
        }

        private void btnInscripcion_Click(object sender, EventArgs e)
        {
            FrmInscripcion nuevoFormulario = new FrmInscripcion();
            nuevoFormulario.Show();
            this.Hide();
        }

        private void btnAsignacion_Click(object sender, EventArgs e)
        {
            FrmAsignacion nuevoFormulario = new FrmAsignacion();
            nuevoFormulario.Show();
            this.Hide();
        }

        private void btnCursos_Click(object sender, EventArgs e)
        {
            FrmCursosEstudiante nuevoFormulario = new FrmCursosEstudiante();
            nuevoFormulario.Show();
            this.Hide();
        }

        private void btnNotas_Click(object sender, EventArgs e)
        {
            FrmNotasEstudiante nuevoFormulario = new FrmNotasEstudiante();
            nuevoFormulario.Show();
            this.Hide();
        }

        private void btnPensum_Click(object sender, EventArgs e)
        {
            FrmPensumEstudiante nuevoFormulario = new FrmPensumEstudiante();
            nuevoFormulario.Show();
            this.Hide();
        }

        private void btnMostrarCursos_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAño.Text) || string.IsNullOrEmpty(txtSemestre.Text))
            {
                MessageBox.Show("Complete todos los campos para mostrar los cursos.");
                return;
            }
            else
            {
                string sConexionBD = ConexionBD.CadenaConexion();
                try
                {
                    using (MySqlConnection conexion = new MySqlConnection(sConexionBD))
                    {
                        conexion.Open();
                        string sSeleccionCursos = @"SELECT ac.codigoCurso_fk as codigo, c.nombreCurso as Curso, ac.seccion as Seccion,
                                                    ac.salon as Salon, ac.diasCurso AS Dia, CONCAT(horaInicio, ' - ', horaSalida) AS horario
                                                    FROM AsignacionAlumnoE ae
                                                    JOIN AsignacionAlumnoD ad ON ae.codigoAsignacion_pk = ad.codigoAsignacion_fk
                                                    JOIN AsignacionCurso ac ON ad.codigoAsignacionCurso_fk = ac.codigoAsignacionCurso_pk
                                                    JOIN Inscripcion i ON ae.noDocumento_fk = i.noDocumento_pk
                                                    JOIN CostoInscripcion ci ON i.codigoCostoInscripcion_fk = ci.codigoCostoInscripcion_pk
                                                    JOIN Curso c ON ac.codigoCurso_fk = c.codigoCurso_pk
                                                    WHERE ae.carnetEstudiante_fk = @carnet AND ci.semestre = @semestre AND ci.año = @año";
                        MySqlCommand comando = new MySqlCommand(sSeleccionCursos, conexion);
                        comando.Parameters.AddWithValue("@carnet", clsSesion.CarnetEstudiante);
                        comando.Parameters.AddWithValue("@año", txtAño.Text);
                        comando.Parameters.AddWithValue("@semestre", txtSemestre.Text);
                        MySqlDataReader resultado = comando.ExecuteReader();
                        DataTable tabla = new DataTable();
                        tabla.Columns.Add("Código");
                        tabla.Columns.Add("Curso");
                        tabla.Columns.Add("Sección");
                        tabla.Columns.Add("Días Asignados");
                        tabla.Columns.Add("Horario");
                        tabla.Columns.Add("Salón");
                        while (resultado.Read())
                        {
                            string sCodigo = resultado["codigo"].ToString();
                            string sCurso = resultado["Curso"].ToString();
                            string sSeccion = resultado["Seccion"].ToString();
                            string sHorario = resultado["horario"].ToString();
                            int dia = Convert.ToInt32(resultado["Dia"]);
                            string sSalon = resultado["Salon"].ToString();
                            string sDiasAsignados = clsDiasCurso.sObtenerDiasCurso(dia);

                            tabla.Rows.Add(sCodigo, sCurso, sSeccion, sDiasAsignados, sHorario, sSalon);
                        }
                        if (tabla.Rows.Count > 0)
                        {
                            dgvNotas.DataSource = tabla;
                            dgvNotas.Columns["Código"].FillWeight = 50;
                            dgvNotas.Columns["Sección"].FillWeight = 60;
                            dgvNotas.Columns["Salón"].FillWeight = 60;
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron cursos con los datos proporcionados");
                            return;
                        }
                        dgvNotas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener los cursos: " + ex.Message);
                }
            }
        }
    }
}
