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
        private int _idCliente;
        private string _dni;
        private string _nombre;
        private string _apellidos;
        private int _tlf;
        private string _matricula;
        private string _marca;
        private string _modelo;




        //Propiedades
        public int IdCliente { get => _idCliente; set => _idCliente = value; }
        public string Dni { get => _dni; set => _dni = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Apellidos { get => _apellidos; set => _apellidos = value; }
        public int Tlf { get => _tlf; set => _tlf = value; }
        public string Matricula { get => _matricula; set => _matricula = value; }
        public string Marca { get => _marca; set => _marca = value; }
        public string Modelo { get => _modelo; set => _modelo = value; }

        //Constructor:

        public Cliente() {
        }



    }
}
