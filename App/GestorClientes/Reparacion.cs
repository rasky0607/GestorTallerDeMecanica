using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorClientes
{
    class Reparacion
    {
        //Campos
        int _numReparacion;
        string _dniCliente;
        string _matriCoche;
        int _codServicio;
        string _fecha;
        
        //Propiedades
        public int NumReparacion { get => _numReparacion; set => _numReparacion = value; }
        public string DniCliente { get => _dniCliente; set => _dniCliente = value; }
        public string MatriCoche { get => _matriCoche; set => _matriCoche = value; }
        public int CodServicio { get => _codServicio; set => _codServicio = value; }
        public String Fecha { get => _fecha; set => _fecha = value; }

        //Constructor:
        public Reparacion() {

        }

 
    }
}
