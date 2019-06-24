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
        int _id;
        int _idCliente;
        string _matriCoche;
        int _codServicio;
        string _fecha;
        
        //Propiedades
        public int Id { get => _id; set => _id = value; }
        public int IdCliente { get => _idCliente; set => _idCliente = value; }
        public string MatriCoche { get => _matriCoche; set => _matriCoche = value; }
        public int CodServicio { get => _codServicio; set => _codServicio = value; }
        public String Fecha { get => _fecha; set => _fecha = value; }

        //Constructor:
        public Reparacion() {

        }

        public Reparacion(int id, int idCliente,string matriCoche,int codServicio,DateTime fecha) {
            Id = id;
            IdCliente = idCliente;
            MatriCoche = matriCoche;
            CodServicio = codServicio;
            Fecha = fecha.ToShortDateString();//Solo el dia, nada de la hora

        }
    }
}
