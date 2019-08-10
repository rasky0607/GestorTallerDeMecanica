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
        //-----------//
        string _matricula;
        int codServicio;
        //Estos dos campos son para pura estetica del listado,no son propios de la tabla Factura;
        string _nombreServicio;
        //----------//
        string fecha;
        string _estadoFactura;
        string _numeroFacturaAnulada;//Es de tipo string en lugar de int como _numeroFactura por que este puede ser NULL enla BD pero numerodeFactura no, y al leer un null y guardarlo en un tipo int puede saltar excepcion
        double _precioServicio;


        public int NumeroFactura { get => _numeroFactura; set => _numeroFactura = value; }
        public int Linea { get => _linea; set => _linea = value; }
        public string EstadoFactura { get => _estadoFactura; set => _estadoFactura = value; }
        public string NumeroFacturaAnulada { get => _numeroFacturaAnulada; set => _numeroFacturaAnulada = value; }
        public int IdCliente { get => idCliente; set => idCliente = value; }
        public string NombreCliente { get => _nombreCliente; set => _nombreCliente = value; }
        public string ApellidosCliente { get => _apellidosCliente; set => _apellidosCliente = value; }
        public string Matricula { get => _matricula; set => _matricula = value; }
        public int CodServicio { get => codServicio; set => codServicio = value; }
        public string NombreServicio { get => _nombreServicio; set => _nombreServicio = value; }
        public double PrecioServicio { get => _precioServicio; set => _precioServicio = value; }
        public string Fecha { get => fecha; set => fecha = value; }
      
     
        
    }
}
