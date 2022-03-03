using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Estudiantes
{
    internal class Estudiante //clase para almacenar datos del estudiante y sus registros de faltas
    {
        public string Carne { get; set; }
        public string Nombre { get; set; }
        public string Carrera { get; set; }
        public List<string> Cursos { get; set; }
        public int Tregistros { get; set; }

        //inicializacion de los datos del estudiante
        public Estudiante(string Scarne, string Snombre, string Scarrera)
        {
            Carne = Scarne;
            Nombre = Eliminarcentos(Snombre).ToUpper();
            Carrera = Eliminarcentos(Scarrera).ToUpper();
            Cursos = new List<string>();
        }

        //agrega un nuevo curso al listado
        public void NuevoCurso(string Scurso)
        {
            Scurso = Eliminarcentos(Scurso).ToUpper();
            if (Cursos.Find(s => s == Scurso) == null)
            {
                Cursos.Add(Scurso);
            }
            Tregistros++;
        }

        //elimina los acentos en el texto recibido
        public static string Eliminarcentos(string text) 
        {
            StringBuilder sbnuevotexto = new StringBuilder();
            var caracteres = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letra in caracteres)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letra) != UnicodeCategory.NonSpacingMark)
                    sbnuevotexto.Append(letra);
            }
            return sbnuevotexto.ToString();
        }
    }
}
