using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estudiantes
{
    internal class Registros //clase para almacenar los registros del archivo
    {
        //variables de cada registro
        public Estudiante Est { get; set; }
        public string Carne { get; set; }
        public string Nombre { get; set; }
        public string Carrera { get; set; }
        public string Curso { get; set; }
        public string Tipo_seccion { get; set; }  
        public string Numero_seccion { get; set; }
        public bool[] Incurrencias { get; set; } //almacena la falta que tuvo el estudiante
        public string Observaciones { get; set; }

        //inicializacion de registro
        public Registros(Estudiante Eest, string Scurso, string Stipo,
            string Snumero, string Sinc1, string Sinc2, string Sinc3, string Sinc4, string Sinc5, string Sobservaciones)
        {
            Incurrencias = new bool[5];
            Est = Eest;
            Curso = EliminarAcentos(Scurso).ToUpper();
            Tipo_seccion = Stipo;
            Numero_seccion = Snumero;
            Incurrencias[0] = Sinc1.Trim() != "" ? true : false;
            Incurrencias[1] = Sinc2.Trim() != "" ? true : false;
            Incurrencias[2] = Sinc3.Trim() != "" ? true : false;
            Incurrencias[3] = Sinc4.Trim() != "" ? true : false;
            Incurrencias[4] = Sinc5.Trim() != "" ? true : false;
            Observaciones = Sobservaciones;
        }

        //elimina los acentos en el texto recibido
        public static string EliminarAcentos(string text)
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

        //retorna los datos como un arreglo de objetos
        public object[] RegData()
        {
            return new object[5] { Curso, Tipo_seccion, Numero_seccion, Incs(), Observaciones };
        }

        //retorna como texto consolidado las flatas que tuvo el estudiante
        public string Incs()
        {
            StringBuilder sb = new StringBuilder();
            if (Incurrencias[0]) { sb.AppendLine("-Nunca se ha presentado"); }
            if (Incurrencias[1]) { sb.AppendLine("-Dejo de presentarse"); }
            if (Incurrencias[2]) { sb.AppendLine("-Su asistencia es muy irregular"); }
            if (Incurrencias[3]) { sb.AppendLine("-Muy bajo desempeño en clase"); }
            if (Incurrencias[0]) { sb.AppendLine("-Incurre en faltas académicas"); }
            return sb.ToString();
        }
    }
}
