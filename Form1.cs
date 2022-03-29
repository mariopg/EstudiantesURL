using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPOI.XSSF.UserModel; //Librerías necesarias para el manejo del archivo
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;

namespace Estudiantes
{
    public partial class Form1 : Form
    {
        IWorkbook Libro; //manejo de archivo excel
        List<Registros> list; //manejo de registros del archivo
        List<Estudiante> estudiantes; //manejo de estudiantes
        List<string> carreras; //almacenamiento de carreras y cursos
        List<string> cursos;

        int empty;
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Excel Files|*.xls;*.xlsx"; //agrega filtro para mostrar solo archivos de excel
        }

        //evento para seleccionar un archivo
        private void btn_select_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Libro  = LeerLibro(openFileDialog1.FileName);
                if (Libro != null)
                {
                    ProcesarLibro();
                    CargarCombos();
                    MessageBox.Show("Archivo cargado exitosamente");
                }
            }
        }

        //Procesamiento libro de excel seleccionado
        public void ProcesarLibro()
        {
            ISheet hoja = Libro.GetSheetAt(0);
            int lastRow = hoja.LastRowNum;
            estudiantes = new List<Estudiante>();
            carreras = new List<string>();
            cursos = new List<string>();
            list = new List<Registros>();
            empty = 0;
            for (int i = hoja.FirstRowNum; i <= lastRow; i++) //se lee fila por fila de la hoja del libro
            {
                IRow row = hoja.GetRow(i);
                try
                {
                    InsertFila(row);
                    i = empty >= 3 ? lastRow + 1 : i; //tolerancia de 3 filas en blanco
                }
                catch
                {
                    Console.WriteLine("i: " + i.ToString() + " Fila: " + row.RowNum);
                };
            }
        }

        //Validacion para lectura del libro
        public IWorkbook LeerLibro(string path)
        {
            IWorkbook libro;

            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                //Intenta leer el libro tipo XLSX:
                try
                {
                    libro = new XSSFWorkbook(fs);
                }
                catch (Exception ex)
                {
                    libro = null;
                }

                //Si falla la lectura, intenta leer como tipo XLS:
                if (libro == null)
                {
                    libro = new HSSFWorkbook(fs);
                }
                return libro;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en lectura de archivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        //Inserta la fila leída en las listas correspondientes
        public void InsertFila(IRow fila)
        {
            if (fila.RowNum > 1)
            {
                List<ICell> celdas = fila.Cells; //se obtiene las celdas de la fila 

                if (!string.IsNullOrEmpty(celdas[0].ToString().Trim()))
                {
                    Estudiante reg = estudiantes.Find(x => x.Carne == celdas[0].ToString().Trim()); //valida si ya se ingreso el estudiante
                    if(reg == null)
                    {
                        reg = new Estudiante(celdas[0].ToString().Trim(), celdas[1].ToString(), celdas[2].ToString().Trim()); //agrega el estudiante
                        estudiantes.Add(reg);
                    }
                    reg.NuevoCurso(celdas[3].ToString()); //agrega el curso al estudiante
                    Registros NuevoReg = new Registros(reg,celdas[3].ToString(), celdas[4].ToString(), celdas[5].ToString(), celdas[6].ToString(),
                        celdas[7].ToString(), celdas[8].ToString(), celdas[9].ToString(), celdas[10].ToString(), celdas[11].ToString()); 
                    list.Add(NuevoReg); //ingresa un nuevo registro de reporte
                    CarrerasList(NuevoReg.Est.Carrera); //agrega Carrera y Curso al listado
                    CursosList(NuevoReg.Curso);
                }
                else
                {
                    empty++;
                }
            }
        }

        //Agrega carreras a la lista
        void CarrerasList(string carrera)
        {
            if(carreras.Find(x => x == carrera) == null)
            {
                carreras.Add(carrera);
            }
        }

        //Agrega cursos a la lista
        void CursosList(string curso)
        {
            if (cursos.Find(x => x == curso) == null)
            {
                cursos.Add(curso);
            }
        }

        //Carga las listas de carreras y cursos en los combobox
        public void CargarCombos()
        {
            comboCarreras.Items.Clear();
            comboCarreras.Items.Add("-TODOS-");
            foreach (string carrera in carreras)
            {
                comboCarreras.Items.Add(carrera);
            }
            comboCursos.Items.Clear();
            comboCursos.Items.Add("-TODOS-");
            foreach (string curso in cursos)
            {
                comboCursos.Items.Add(curso);
            }
            comboCarreras.SelectedIndex = 0;
            comboCursos.SelectedIndex = 0;
        }

        //boton para busqueda por carnet
        private void btn_buscar_Click(object sender, EventArgs e)
        {
            if(list != null)
            {
                dgvEstudiantes.Rows.Clear();
                List<Estudiante> results = estudiantes.FindAll(x => x.Carne.Equals(tb_carne.Text)); //busca en la lista de estudiantes por medio de carnet
                dgvEstudiantes.Rows.Clear();
                foreach (Estudiante est in results) //lee los resultados de la busqueda
                {
                    dgvEstudiantes.Rows.Add(new object[] { "Ver", est.Carne, est.Nombre, est.Tregistros }); //muestra en el datagrid los resultados

                }
                dgvEstudiantes.Sort(dgvEstudiantes.Columns[3], ListSortDirection.Descending);
                lblTotal.Text = "Total encontrados: " + results.Count;
            }
        }

        //boton para buscar por curso y carrera *OBSOLETO*
        private void btn_buscarcc_Click(object sender, EventArgs e)
        {
            if (list != null)
            {
                dgvEstudiantes.Rows.Clear();
                List<Estudiante> results = ResultadosCarreraCurso();
                
                dgvEstudiantes.Rows.Clear();
                foreach (Estudiante est in results)
                {
                    dgvEstudiantes.Rows.Add(new object[] { "Ver", est.Carne, est.Nombre, est.Tregistros });
                }
                dgvEstudiantes.Sort(dgvEstudiantes.Columns[3], ListSortDirection.Descending);
                lblTotal.Text = "Total encontrados: " + results.Count;
            }
        }

        //retorna la lista de estudiantes segun condiciones de busqueda por curso y carrera
        List<Estudiante> ResultadosCarreraCurso()
        {
            int indexCarreras = comboCarreras.SelectedIndex;
            int indexCursos = comboCursos.SelectedIndex;

            if (indexCarreras > 0 && indexCursos > 0)
            {
                return estudiantes.FindAll(x => x.Carrera.Equals(comboCarreras.Text) && x.Cursos.Find(y => y == comboCursos.Text) != null);
            }
            if (indexCarreras > 0)
            {
                return estudiantes.FindAll(x => x.Carrera.Equals(comboCarreras.Text));
            }
            if(indexCursos > 0)
            {
                return estudiantes.FindAll(x => x.Cursos.Find( y => y == comboCursos.Text) != null);
            }
            return estudiantes;
        }

        //boton para por numero de reportes *OBSOLETO*
        private void btn_buscarnum_Click(object sender, EventArgs e)
        {
            if (list != null)
            {
                dgvEstudiantes.Rows.Clear();
                List<Estudiante> ests = rbTotal.Checked ? 
                    estudiantes.FindAll(x => x.Tregistros == numericUpDown1.Value) : estudiantes.FindAll(x => x.Tregistros >= numericUpDown1.Value);

                dgvEstudiantes.Rows.Clear();
                foreach (Estudiante estudiante in ests)
                {
                    dgvEstudiantes.Rows.Add(new object[] { "Ver", estudiante.Carne, estudiante.Nombre, estudiante.Tregistros });
                }
                dgvEstudiantes.Sort(dgvEstudiantes.Columns[3], ListSortDirection.Descending);
                lblTotal.Text = "Total encontrados: " + ests.Count;
            }
        }

        //boton para busqueda consolidada CARRERA, CURSO Y NUM DE REPORTES
        private void btn_search_Click(object sender, EventArgs e)
        {
            if (list != null)
            {
                dgvEstudiantes.Rows.Clear();
                List<Estudiante> ests = ResultadosCarreraCurso();

                List<Estudiante> results = rbTotal.Checked ?
                    ests.FindAll(x => x.Tregistros == numericUpDown1.Value) : ests.FindAll(x => x.Tregistros >= numericUpDown1.Value);

                dgvEstudiantes.Rows.Clear();
                foreach (Estudiante est in results)
                {
                    dgvEstudiantes.Rows.Add(new object[] { "Ver", est.Carne, est.Nombre, est.Tregistros });
                }
                dgvEstudiantes.Sort(dgvEstudiantes.Columns[3], ListSortDirection.Descending);
                lblTotal.Text = "Total encontrados: " + results.Count;
            }
        }

        //Evento del primer datagrid para mostrar los resultados en el segundo datagrid
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 0)
            {
                string Carnet = dgvEstudiantes[1, e.RowIndex].Value.ToString(); //se obtiene el carnet del estudiante seleccionado
                Estudiante estudiante = estudiantes.Find(x => x.Carne == Carnet); //se obtiene el estudiante
                lblEst.Text = estudiante.Nombre;    //se muestra la informacion
                lblCarnet.Text = estudiante.Carne;
                lblCarr.Text = estudiante.Carrera;
                dgvReportes.Rows.Clear();
                List<Registros> resultado = list.FindAll(x => x.Est.Carne == Carnet); //se obtiene el listado de registros correspondientes al estudiante
                foreach(Registros tabla in resultado)
                {
                    dgvReportes.Rows.Add(tabla.RegData());
                }
                dgvReportes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgvReportes.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvReportes.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvReportes.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvReportes.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
        }
    }
}