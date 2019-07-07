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
        private string _dni;
        private string _nombre;
        private string _apellidos;
        private int _tlf;
        string _matricula;
        string _marca;
        string _modelo;




        //Propiedades
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
