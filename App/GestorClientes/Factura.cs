using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorClientes
{
    class Factura
    {
        int _numeroFactura;
        int _linea;
        int idCliente;
        //Estos dos campos son para pura estetica del listado,no son propios de la tabla Factura;
        string _nombreCliente;
        string _apellidosCliente;
        string _matricula;
        int codServicio;
        string fecha;
        int _numeroFacturaAnulada;
      

        public int NumeroFactura { get => _numeroFactura; set => _numeroFactura = value; }
        public int Linea { get => _linea; set => _linea = value; }
        public int IdCliente { get => idCliente; set => idCliente = value; }
        public string NombreCliente { get => _nombreCliente; set => _nombreCliente = value; }
        public string ApellidosCliente { get => _apellidosCliente; set => _apellidosCliente = value; }
        public string Matricula { get => _matricula; set => _matricula = value; }
        public int CodServicio { get => codServicio; set => codServicio = value; }
        public string Fecha { get => fecha; set => fecha = value; }
        public int NumeroFacturaAnulada { get => _numeroFacturaAnulada; set => _numeroFacturaAnulada = value; }
      
    }
}
