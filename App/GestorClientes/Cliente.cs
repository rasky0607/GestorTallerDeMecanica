using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorClientes
{
    class Cliente
    {
        //Campos
        int _id;
        private string _dni;
        private string _nombre;
        private string _apellidos;
        private int _tlf;

        //Propiedades
        public int Id { get => _id; set => _id = value; }
        public string Dni { get => _dni; set => _dni = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Apellidos { get => _apellidos; set => _apellidos = value; }
        public int Tlf { get => _tlf; set => _tlf = value; }

        //Constructor:

        public Cliente() {
        }

        public Cliente(int id,string dni,string nombre, string apellidos, int tlf)
        {
            Id = id;
            Dni = dni;
            Nombre = nombre;
            Apellidos = apellidos;
            Tlf = tlf;
        }


    }
}
