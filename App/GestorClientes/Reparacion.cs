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
        int _idCliente;
        string _matriCoche;
        int _codServicio;
        string _nombreServicio;  //Campo adicional para mostrar el resultado de join o subconsulta de la tabla servicio dentro de la tabla reapraciones y mostrar el nombre de  las reparaciones en lugar de el codigo
        string _fecha;
     
        
        //Propiedades
        public int NumReparacion { get => _numReparacion; set => _numReparacion = value; }
        public int IdCliente { get => _idCliente; set => _idCliente = value; }
        public string MatriCoche { get => _matriCoche; set => _matriCoche = value; }
        public int CodServicio { get => _codServicio; set => _codServicio = value; }
        //Propiedad adicional para mostrar el resultado de join o subconsulta de la tabla servicio dentro de la tabla reapraciones y mostrar el nombre de  las reparaciones en lugar de el codigo
        public String NombreServicio { get => _nombreServicio; set => _nombreServicio = value; }
        //-------------//
        public String Fecha { get => _fecha; set => _fecha = value; }
       

        //Constructor:
        public Reparacion() {

        }

        public Reparacion(int numRepa, int idCli,string matricoch,string nombreServic,string fech)
        {
            NumReparacion = numRepa;
            IdCliente = idCli;
            MatriCoche = matricoch;
            NombreServicio = nombreServic;
            Fecha = fech;
        }


    }
}
