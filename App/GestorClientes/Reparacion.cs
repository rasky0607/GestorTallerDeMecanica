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
        string _nombreServicio;  //Campo adicional para mostrar el resultado de join o subconsulta de la tabla servicio dentro de la tabla reapraciones y mostrar el nombre de  las reparaciones en lugar de el codigo(pura estetica)
        string _fecha;
        string _nombreCliRepa;//Campo adicional para mostrar resultado de una subconsulta(pura estetica)
        string _apellidosCliRepa;//campo adicional para mostrar resultado de una subconsulta(pura estetica)
        
        //Propiedades
        public int NumReparacion { get => _numReparacion; set => _numReparacion = value; }
        public int IdCliente { get => _idCliente; set => _idCliente = value; }
        //Propiedad adicional para mostrar el resultado de join o subconsulta de la tabla servicio dentro de la tabla reapraciones y mostrar el nombre de  las reparaciones en lugar de el codigo
        public String NombreCliRepa { get => _nombreCliRepa; set => _nombreCliRepa = value; }
        public string ApellidosCliRepa { get => _apellidosCliRepa; set => _apellidosCliRepa = value; }
        //-------------//
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
