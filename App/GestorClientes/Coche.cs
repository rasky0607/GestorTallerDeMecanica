using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorClientes
{
    class Coche
    {
        //Campos
        string _matricula;
        string _marca;
        string _modelo;

        //Propiedades
        public string Matricula { get => _matricula; set => _matricula = value; }
        public string Marca { get => _marca; set => _marca = value; }
        public string Modelo { get => _modelo; set => _modelo = value; }

        //Constructores
        public Coche() {
        }

        public Coche(string matricula,string marca,string modelo) {
            Matricula = matricula;
            Marca = marca;
            Modelo = modelo;
        }

    }
}
