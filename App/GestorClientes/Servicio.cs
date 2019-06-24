using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorClientes
{
    class Servicio
    {
        //Campos
        private int _codigo;
        private string _descripcion;
        private double _precio;

        //Propiedades
        public int Codigo { get => _codigo; set => _codigo = value; }
        public string Descripcion { get => _descripcion; set => _descripcion = value; }
        public double Precio { get => _precio; set => _precio = value; }

        //Constructor

        public Servicio()
        {
        }

        public Servicio(int codigo, string descripcion, double precio) {
            Codigo = codigo;
            Descripcion = descripcion;
            Precio = precio;
        }
    }
}
