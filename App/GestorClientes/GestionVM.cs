using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//añadido
using System.ComponentModel;
using System.Windows;
using System.Data.SQLite;

namespace GestorClientes
{
    class GestionVM: INotifyPropertyChanged
    {
        DaoSqlite _dao = new DaoSqlite();
        #region Variables
        string colorRojo = "#FFD66E6E";
        string colorAzul = "#FF45A3CF";
        #endregion
        #region campos
       
        bool _habilitado = false;//Habilita o deshabilita ciertos botones o opciones
        string _mensaje;//Mensaje de informacion     
        string _mensajeActualizacion;
        string _conectadoDesconectado="Conectar";//Nombre que daremos al boton de conexion y desconexion segun el estado de dicha conexion
        bool _estadoConexion = false;//true abierta, false cerrada
        string _colorConexion= "#FF45A3CF";
        List<object> _listado;
        //Campos pestaña Añadir/Insertar
        string _mostrarMensajeInsercion="Hidden";//Hidden-Visible
        string _mensajeInsercion;
        List<string> _listablas;
        string _tablaSelecionada; //tabla selecionada en el combobox de la pestaña añadir

        //Datos Añdir Cliente
        string _dniCliInsert;
        string _nombreCliInsert;
        string _apellidosCliInsert;
        int _tlfCliInsert;

        //Datos Añadir Servicio(Convertir Propiedades)
        string _descripcionInsert;
        string _precioInsert;

        //Datos Añadir Coche(Convertir Propiedades)
        string _matriculaInsert;
        string _marcaInsert;
        string _modeloInsert;

        //Datos Añadir Reparacion(Convertir Propiedades)
        string _dniClirepaInsert;
        string _matriculaRepaInsert;
        string _codServicioRepa;
        DateTime _fechaRepaInsert;
        int _idRepaInsert;


        //Campos pestaña Modificar
        bool _habilitarModificaciones = false;//Deshabilitado hasta que se marque un registro y se pinche en el boton modificar

        #endregion

        #region Propiedades
        public bool Habilitado {
            get { return _habilitado; }
            set {
                if (EstadoConexion)
                {
                    _habilitado = true;
                    Notificador("Habilitado");
                    //Notificador("EstadoConexion");
                }
                else
                {
                    _habilitado = false;
                    Notificador("Habilitado");
   
                }
            }
        }

        public string Mensaje {
            get { return _mensaje; }

            set
            {
                if (_mensaje != value)
                {
                    _mensaje = value;
                    Notificador("Mensaje");
                }
            }

        }

        public string MensajeInsercion {
            get { return _mensajeInsercion; }

            set
            {
                if (_mensajeInsercion != value)
                {
                    _mensajeInsercion = value;
                    Notificador("MensajeInsercion");
                }
            }
        }

        public string MensajeActualizacion {
            get { return _mensajeActualizacion; }

            set
            {
                if (_mensajeActualizacion != value)
                {
                    _mensajeActualizacion = value;
                    Notificador("MensajeActualizacion");
                }
            }
        }

        public bool EstadoConexion {
            get { return _estadoConexion; }

            set
            {
                if (_estadoConexion != value)
                {
                    _estadoConexion = value;
                    Notificador("EstadoConexion");                   
                }
            }
        }


        //Propiedad Content de el boton conectar/desconectar
        public string ConectadoDesconectado
        {
            get { return _conectadoDesconectado; }

            set
            {
                if (EstadoConexion)
                {
                    _conectadoDesconectado = "Desconectar";//Si el estado dela conecxion es verdad es decir  hay conexion, el boton muestra la palabra desconectar
                    Notificador("ConectadoDesconectado");
                    Habilitado = true;
                
                }
                else {
                    _conectadoDesconectado = "Conectar";//Si el estado dela conecxion es falso es decir no hay conexion, el boton muestra la palabra conectar
                    Notificador("ConectadoDesconectado");
                    Habilitado = false;
                    
                }
            }
        }

        public string ColorConexion {
            get { return _colorConexion; }

            set
            {
                if (EstadoConexion)
                {
                    _colorConexion = colorRojo;//Esta conectado, por lo que el boton se mostrara rojoy con la palbra Desconectar, para resaltar la psobilidad de desconectar
                    Notificador("ColorConexion");
                }
                else
                {
                    _colorConexion = colorAzul;//Esta conectado, por lo que el boton se mostrara rojoy con la palbra Desconectar, para resaltar la psobilidad de desconectar
                    Notificador("ColorConexion");
                    Notificador("EstadoConexion");
                }
            }
            
        }

        public List<object> Listado
        {
            get { return _listado; }

            set
            {
                if (_listado != value)
                {
                    _listado = value;
                    Notificador("Listado");
                }
            }
        }

        #region  Propiedades pestaña Añadir
        public List<string> Listablas
        {
            get { return _listablas; }

            set
            {
                if (_listablas != value)
                {
                    _listablas = value;
                    Notificador("Listablas");
                }
            }
        }

        public string TablaSelecionada
        {
            get { return _tablaSelecionada; }
            set
            {
                if (_tablaSelecionada != value)
                {
                    _tablaSelecionada = value;
                    Notificador("TablaSelecionada");
                }
            }
        }

        public string MostrarMensajeInsercion
        {
            get { return _mostrarMensajeInsercion; }
            set
            {
                if (_mostrarMensajeInsercion != value)
                {
                    _mostrarMensajeInsercion = value;
                    Notificador("MostrarMensajeInsercion");
                }
            }
        }

        #region Campos Cliente
        public string DniCliInsert
        {
            get { return _dniCliInsert; }
            set
            {
                if (_dniCliInsert != value)
                {
                    _dniCliInsert = value;
                    Notificador("DniCliInsert");
                }
            }
        }

        public string NombreCliInsert
        {
            get { return _nombreCliInsert; }
            set
            {
                if (_nombreCliInsert != value)
                {
                    _nombreCliInsert = value;
                    Notificador("NombreCliInsert");
                }
            }
        }

        public string ApellidosCliInsert
        {
            get { return _apellidosCliInsert; }
            set
            {
                if (_apellidosCliInsert != value)
                {
                    _apellidosCliInsert = value;
                    Notificador("ApellidosCliInsert");
                }
            }
        }

        public int TlfCliInsert
        {
            get { return _tlfCliInsert; }
            set
            {
                if (_tlfCliInsert != value)
                {
                    _tlfCliInsert = value;
                    Notificador("TlfCliInsert");
                }
            }
        }
        #endregion

        #region Campos Servicios
        public string DescripcionInsert
        {
            get { return _descripcionInsert; }
            set
            {
                if (_descripcionInsert != value)
                {
                    _descripcionInsert = value;
                    Notificador("DescripcionInsert");
                }
            }
        }

        public string PrecioInsert
        {
            get { return _precioInsert; }
            set
            {
                if (_precioInsert != value)
                {
                    _precioInsert = value;
                    Notificador("PrecioInsert");
                }
            }
        }
        #endregion

        #region Campos Coche
        public string MatriculaInsert
        {
            get { return _matriculaInsert; }
            set
            {
                if (_matriculaInsert != value)
                {
                    _matriculaInsert = value;
                    Notificador("MatriculaInsert");
                }
            }
        }

        public string MarcaInsert
        {
            get { return _marcaInsert; }
            set
            {
                if (_marcaInsert != value)
                {
                    _marcaInsert = value;
                    Notificador("MarcaInsert");
                }
            }
        }

        public string ModeloInsert
        {
            get { return _modeloInsert; }
            set
            {
                if (_modeloInsert != value)
                {
                    _modeloInsert = value;
                    Notificador("ModeloInsert");
                }
            }
        }
        #endregion

        #region Campos Reparacion

        public string DniClirepaInsert
        {
            get { return _dniClirepaInsert; }
            set
            {
                if (_dniClirepaInsert != value)
                {
                    _dniClirepaInsert = value;
                    Notificador("DniClirepaInsert");
                }
            }
        }

        public string MatriculaRepaInsert
        {
            get { return _matriculaRepaInsert; }
            set
            {
                if (_matriculaRepaInsert != value)
                {
                    _matriculaRepaInsert = value;
                    Notificador("MatriculaRepaInsert");
                }
            }
        }

        public string CodServicioRepa
        {
            get { return _codServicioRepa; }
            set
            {
                if (_codServicioRepa != value)
                {
                    _codServicioRepa = value;
                    Notificador("CodServicioRepa");
                }
            }
        }

        public DateTime FechaRepaInsert
        {
            get { return _fechaRepaInsert; }
            set
            {
                if (_fechaRepaInsert != value)
                {
                    _fechaRepaInsert = value;
                    Notificador("FechaRepaInsert");
                }
            }
        }

        public int IdRepaInsert
        {
            get { return _idRepaInsert; }
            set
            {
                if (_idRepaInsert != value)
                {
                    _idRepaInsert = value;
                    Notificador("IdRepaInsert");
                }
            }
        }

        #endregion

        #endregion



       

        //Propiedades de pestaña Modificaciones
        public bool HabilitarModificaciones
        {
            get { return _habilitarModificaciones; }
            set
            {
                if (_habilitarModificaciones != value)
                {
                    _habilitarModificaciones = value;
                    Notificador("HabilitarModificaciones");
                }
            }
        }



        #endregion

        #region Metodos que invocan los Metodos que devuelven objetos RealyCommand mas abajo
        //Listado de registros
        private void ConectarListado()
        {
            try
            {
                if (ConectadoDesconectado == "Conectar")
                {
                    EstadoConexion = _dao.Conectar();
                    Listado = conversion(_dao.selectReparacion());
                   
                    ConectadoDesconectado = "Desconectar";//Ya que esta conectado y este boton ademas lo mostraremos en rojo.. y cuando este desconectado, mostraremos la palabra'conectar' con el fondo verde
                    ColorConexion = colorRojo;
                    Listablas = _dao.VerTablas();
                }
                else
                {
                    if (_dao.Desconectar())
                    {
                        EstadoConexion = _dao.EstadoConexion();
                        Listado = null;
                        ConectadoDesconectado = "Conectar";//Ya que esta conectado y este boton ademas lo mostraremos en rojo.. y cuando este desconectado, mostraremos la palabra'conectar' con el fondo verde
                        ColorConexion = colorAzul;
                        Mensaje = "";
                    }
                }
            }
            catch
            {
                Mensaje = "Error al conectar...";
            }

        }

        private void ListadoClientes()
        {
            if (EstadoConexion)
            {
                try
                {
                    Listado = conversion(_dao.selectCliente());
                }
                catch
                {
                    throw;
                }
            }
        }

        private void ListadoCoches()
        {
            if (EstadoConexion)
            {
                try
                {
                    Listado = conversion(_dao.selectCoche());
                }
                catch
                {
                    throw;
                }
            }
        }

        private void ListadoServicios()
        {
            if (EstadoConexion)
            {
                try
                {
                    Listado = conversion(_dao.selectServicio());
                }
                catch
                {
                    throw;
                }
            }
        }

        private void ListadoReparacion()
        {
            if (EstadoConexion)
            {
                try
                {
                    Listado = conversion(_dao.selectReparacion());
                }
                catch
                {
                    throw;
                }
            }
        }

        private void InsertarRegistro()
        {
            if (EstadoConexion)
            {
                try
                {
                    switch (TablaSelecionada)
                    {
                        case "cliente":
                            if (_dao.InsertCliente(DniCliInsert, NombreCliInsert, ApellidosCliInsert, TlfCliInsert))
                            {
                                MensajeInsercion = "Insercion realizada correctamente";
                                MostrarMensajeInsercion = "Visible";
                            }
                            else
                            {
                                MensajeInsercion = "Insercion fallida.";
                                MostrarMensajeInsercion = "Visible";
                            }
                            break;
                        case "coche":
                            MessageBox.Show("Coche: Me has selecionado!");
                            break;
                        case "servicio":
                            MessageBox.Show("Servicio: Me has selecionado!");
                            break;
                        case "reparacion":
                            MessageBox.Show("Reparacion: Me has selecionado!");
                            break;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        //----Fin Listado de registros---

        #endregion

        #region Metodos para propiedades Command de los componentes de la ventana
        //Listado de registros
        public RelayCommand Conectar_click
        {
            get { return new RelayCommand(conectando => ConectarListado(), conectando => true); }
        }

        public RelayCommand RegistroClientes_click
        {
            get { return new RelayCommand(listadoCli => ListadoClientes(), ListadoCli => true); }
        }

        public RelayCommand RegistroCoches_click
        {
            get { return new RelayCommand(listadoCoh => ListadoCoches(), ListadoCoh => true); }
        }

        public RelayCommand RegistroServicios_click
        {
            get { return new RelayCommand(listadoServ => ListadoServicios(), ListadoServ => true); }
        }

        public RelayCommand RegistroReparaciones_click
        {
            get { return new RelayCommand(insertRegis => ListadoReparacion(), insertRegis => true); }
        }

        public RelayCommand InsertarRegistro_click
        {
            get { return new RelayCommand(listadoRep => InsertarRegistro(), ListadoRep => true); }
        }
        //----Fin Listado de registros---
        #endregion


        #region Conversion de tipo de listas a lista object
        /*Estos metodos son usados para convertir las listas de objetos concretos que nos devuelve el objeto
         dao tra hacer la conexion y  consulta a la BD
         en lista de object para poder tratarlos todos por la misma propiedad "Listado" a la que el datagrid
         esta bindeada para que muestre todos los listados que sean soliciatdos*/

        public List<object> conversion(List<Cliente> lcliente)
        {
            List<object> listobjetos = new List<object>();
            foreach (Cliente cli in lcliente)
            {
                listobjetos.Add(cli);
            }
            return listobjetos;
        }

        public List<object> conversion(List<Coche> lcoche)
        {
            List<object> listobjetos = new List<object>();
            foreach (Coche micoche in lcoche)
            {
                listobjetos.Add(micoche);
            }
            return listobjetos;
        }

        public List<object> conversion(List<Servicio> lservicios)
        {
            List<object> listobjetos = new List<object>();
            foreach (Servicio miServicio in lservicios)
            {
                listobjetos.Add(miServicio);
            }
            return listobjetos;
        }

        public List<object> conversion(List<Reparacion> lreparacio)
        {
            List<object> listobjetos = new List<object>();
            foreach (Reparacion miReparacion in lreparacio)
            {
                listobjetos.Add(miReparacion);
            }
            return listobjetos;
        }
        #endregion

        #region Propagacion de cambios de propiedad
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notificador(string propiedad)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propiedad));
        }
        #endregion


    }
}
/*
//ERRORES:
1-

 //Tareas pendientes:
-> controlar que la propiedad _tlfInsert en su textbox correspondiente
no pueda añadirse otra cosa que no sean numeros.

 -Añadir.
 -Modificaciones.
 -Eliminacion.
 -Filtros
 -Generar Factura()
 -Copia de seguridad BD automatica

//Tareas Finalizadas:
Conectar
Ver otros registros

//Extras:
1-Control de DniClientes solo puede tener una letra y debe ser el ultimo caracter "controlar en codigo o en un trigger de la BD"
2-Controlar  insercion de fechas pasadas


 */
