using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_2
{
    public class Token
    {
        public enum Tipos { Identificador, Numero, Asignacion, Inicializacion,
                            OperadorRelacional, OperadorTermino, OperadorFactor,
                            IncrementoTermino, IncrementoFactor, Cadena, Ternario,
                            FinSentencia, OperadorLogico, Inicio, Fin, Caracter,
                            TipoDato, Zona, Condicion, Ciclo };
        private string Contenido;
        private Tipos   Clasificacion;

        public Token()
        {
            Contenido = "";
            Clasificacion = Tipos.Identificador;
        }

        public void setContenido(string Contenido)
        {
            this.Contenido = Contenido;
        }
        public void setClasificacion(Tipos Clasificacion)
        {
            this.Clasificacion = Clasificacion;
        }
        public string getContenido()
        {
            return this.Contenido;
        }
        public Tipos getClasificacion()
        {
            return this.Clasificacion;
        }
    }
}