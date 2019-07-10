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
      
        #region Variables
        string colorRojo = "#FFD66E6E";
        string colorAzul = "#FF45A3CF";
        public DaoSqlite _dao = new DaoSqlite();
        #endregion

        #region campos

        #region campos generales 
        bool _habilitado = false;//Habilita o deshabilita ciertos botones o opciones
        string _mensaje;//Mensaje de informacion     
        string _conectadoDesconectado="Conectar";//Nombre que daremos al boton de conexion y desconexion segun el estado de dicha conexion
        bool _estadoConexion = false;//true abierta, false cerrada
        string _colorConexion= "#FF45A3CF";
        #endregion

        #region Campos pestaña listar

        List<object> _listado;
        #endregion

        #region campos pestaña Añadir
        //Campos pestaña Añadir/Insertar

        string _mensajeInsercion;
        List<string> _listablas;
        string _tablaSelecionada; //tabla selecionada en el combobox de la pestaña añadir

        //Datos Añdir Cliente
        string _dniCliInsert;
        string _nombreCliInsert;
        string _apellidosCliInsert;
        int _tlfCliInsert;
        string _matriculaInsert;
        string _marcaInsert;
        string _modeloInsert;

        //Datos Añadir Servicio(Convertir Propiedades)
        string _descripcionInsert;
        double _precioInsert;  

        //Datos Añadir Reparacion(Convertir Propiedades)
        string _dniClirepaInsert;//Seleciona un string de un dni de cliente y a partir de el buscamos el id luego internamente almacenandolo en _idClienteRepaInsert
        string _matriculaRepaInsert;
        string _ServicioRepa;//Seleciona un string de servicio y a partir de el buscamos el cod luego internamente almacenandolo en _CodServicioRepa
        int _CodServicioRepa;
        string _fechaRepaInser = DateTime.Now.ToString();
        int _numRepaInsert;

        #endregion

        #region Campos pestaña Modificar
        //Campos pestaña Modificar
        bool _habilitarModificaciones = false;//Deshabilitado hasta que se marque un registro y se pinche en el boton modificar
        string _tablaAcltualListada;//Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
        object _selecionRegistroAModificar = null; //Propiedad para el registro selecionado
        string _mensajeActualizacion="";

        //Datos Modificar Cliente
        string _dniCliMod;
        string _nombreCliMod;
        string _apellidosCliMod;
        int _tlfCliMod;
        string _matriculaMod;
        string _marcaMod;
        string _modeloMod;

        //Datos Modificar Servicio(Convertir Propiedades)
        string _descripcionMod;
        double _precioMod;

        //Datos Modificar Reparacion(Convertir Propiedades)
        string _dniClirepaMod;//Seleciona un string de un dni de cliente y a partir de el buscamos el id luego internamente almacenandolo en _idClienteRepaInsert
        string _matriculaRepaMod;
        string _ServicioRepaMod;//Seleciona un string de servicio y a partir de el buscamos el cod luego internamente almacenandolo en _CodServicioRepa
        int _CodServicioRepaMod;
        string _fechaRepaMod = DateTime.Now.ToShortTimeString();
        int _numRepaMod;
        #endregion

        #endregion
        #region Propiedades

        #region Propiedades Generales
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

        #endregion

        #region Propiedades pestaña Listar
        public List<object> Listado
        {
            get { return _listado; }

            set
            {
                if (_listado != value)
                {
                    _listado = value;
                    SelecionRegistroAModificar = null;
                    Notificador("Listado");
                }
            }
        }
        #endregion

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

        public string MensajeInsercion
        {
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

        public double PrecioInsert
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

        public string ServicioRepa
        {
            get { return _ServicioRepa; }
            set
            {
                if (_ServicioRepa != value)
                {
                    _ServicioRepa = value;
                    Notificador("ServicioRepa");
                }
            }
        }

        public int CodServicioRepa
        {
            get { return _CodServicioRepa; }
            set
            {
                if (_CodServicioRepa != value)
                {
                    _CodServicioRepa = value;
                    Notificador("CodServicioRepa");
                }
            }
        }

        public string FechaRepaInser
        {
            get { return _fechaRepaInser; }
            set
            {
                if (_fechaRepaInser != value)
                {
                    _fechaRepaInser = value;
                    Notificador("FechaRepaInser");
                }
                
            }
        }

  
        public int NumRepaInsert
        {
            get { return _numRepaInsert; }
            set
            {
                if (_numRepaInsert != value)
                {
                    _numRepaInsert = value;
                    Notificador("NumRepaInsert");
                }
            }
        }

        #endregion

        #endregion

        #region Propiedades pestaña Modificar

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
      
        public string TablaAcltualListada //Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
        {
            get { return _tablaAcltualListada; }

            set
            {
                if (_tablaAcltualListada!=value)
                {
                    _tablaAcltualListada = value;
                    Notificador("TablaAcltualListada");
                }
               
            }

        }

       
        public object SelecionRegistroAModificar {
            get { return _selecionRegistroAModificar; }

            set
            {
                if (_selecionRegistroAModificar != value)
                {
                    _selecionRegistroAModificar = value;
                    if (_selecionRegistroAModificar != null)//Debemos pregunar si el selectItem de el listado  en este caso_selecionRegistroAModificar esta a NULL para evitar excepcion
                    {
                        //Preparacion de propiedades segun la fila del datagrid selecionada
                        switch (TablaAcltualListada)
                        {
                            case "cliente":
                                Cliente c = new Cliente();
                                c = (Cliente)_selecionRegistroAModificar;
                                DniCliMod = c.Dni;
                                NombreCliMod = c.Nombre;
                                ApellidosCliMod = c.Apellidos;
                                TlfCliMod = c.Tlf;
                                MatriculaMod = c.Matricula;
                                MarcaMod = c.Marca;
                                ModeloMod = c.Modelo;
                                break;

                            case "servicio":
                                Servicio s = new Servicio();
                                s = (Servicio)_selecionRegistroAModificar;
                                DescripcionMod = s.Descripcion;
                                PrecioMod = s.Precio;
                                break;
                                /*La preparacion de los datos selecionado de el registro de reaparacion en el datagrid de listado para su modificacion
                                 * son preparados en la clase MainWindow en el metodo:
                                 (Clase MainWIndows en BtnEditar_Click)
                                */


                        }
                    }

                    Notificador("SelecionRegistroAModificar");
                }

            }
        }

        public string MensajeActualizacion
        {
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


        //Propiedades de los componentes de la pestaña modificaciones

        #region Campos Cliente
        public string DniCliMod
        {
            get { return _dniCliMod; }
            set
            {
                if (_dniCliMod != value)
                {
                    _dniCliMod = value;
                    Notificador("DniCliMod");
                }
            }
        }

        public string NombreCliMod
        {
            get { return _nombreCliMod; }
            set
            {
                if (_nombreCliMod != value)
                {
                    _nombreCliMod = value;
                    Notificador("NombreCliMod");
                }
            }
        }

        public string ApellidosCliMod
        {
            get { return _apellidosCliMod; }
            set
            {
                if (_apellidosCliMod != value)
                {
                    _apellidosCliMod = value;
                    Notificador("ApellidosCliMod");
                }
            }
        }

        public int TlfCliMod
        {
            get { return _tlfCliMod; }
            set
            {
                if (_tlfCliMod != value)
                {
                    _tlfCliMod = value;
                    Notificador("TlfCliMod");
                }
            }
        }

        public string MatriculaMod
        {
            get { return _matriculaMod; }
            set
            {
                if (_matriculaMod != value)
                {
                    _matriculaMod = value;
                    Notificador("MatriculaMod");
                }
            }
        }

        public string MarcaMod
        {
            get { return _marcaMod; }
            set
            {
                if (_marcaMod != value)
                {
                    _marcaMod = value;
                    Notificador("MarcaMod");
                }
            }
        }

        public string ModeloMod
        {
            get { return _modeloMod; }
            set
            {
                if (_modeloMod != value)
                {
                    _modeloMod = value;
                    Notificador("ModeloMod");
                }
            }
        }
   
        #endregion

        #region Campos Servicios
        public string DescripcionMod
        {
            get { return _descripcionMod; }
            set
            {
                if (_descripcionMod != value)
                {
                    _descripcionMod = value;
                    Notificador("DescripcionMod");
                }
            }
        }

        public double PrecioMod
        {
            get { return _precioMod; }
            set
            {
                if (_precioMod != value)
                {
                    _precioMod = value;
                    Notificador("PrecioMod");
                }
            }
        }
        #endregion

        #region Campos Reparacion

        public string DniClirepaMod
        {
            get { return _dniClirepaMod; }
            set
            {
                if (_dniClirepaMod != value)
                {
                    _dniClirepaMod = value;
                    Notificador("DniClirepaMod");
                }
            }
        }     

        public string MatriculaRepaMod
        {
            get { return _matriculaRepaMod; }
            set
            {
                if (_matriculaRepaMod != value)
                {
                    _matriculaRepaMod = value;
                    Notificador("MatriculaRepaMod");
                }
            }
        }

        public string ServicioRepaMod
        {
            get { return _ServicioRepaMod; }
            set
            {
                if (_ServicioRepaMod != value)
                {
                    _ServicioRepaMod = value;
                    Notificador("ServicioRepaMod");
                }
            }
        }

        public int CodServicioRepaMod
        {
            get { return _CodServicioRepaMod; }
            set
            {
                if (_CodServicioRepaMod != value)
                {
                    _CodServicioRepaMod = value;
                    Notificador("CodServicioRepaMod");
                }
            }
        }

        public string FechaRepaMod
        {
            get { return _fechaRepaMod; }
            set
            {
                if (_fechaRepaMod != value)
                {
                    _fechaRepaMod = value;
                    Notificador("FechaRepaMod");
                }

            }
        }

        public int NumRepaMod
        {
            get { return _numRepaMod; }
            set
            {
                if (_numRepaMod != value)
                {
                    _numRepaMod = value;
                    Notificador("NumRepaMod");
                }
            }
        }

        #endregion


        #endregion

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
                    //Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
                    TablaAcltualListada = "reparacion";                 

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
                    //Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
                    TablaAcltualListada = "cliente";
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
                    //Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
                    TablaAcltualListada = "servicio";
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
                    //Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
                    TablaAcltualListada = "reparacion";
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
                            if (_dao.InsertCliente(DniCliInsert, NombreCliInsert, ApellidosCliInsert, TlfCliInsert,MatriculaInsert,MarcaInsert,ModeloInsert))
                            {
                                MensajeInsercion = "Insercion realizada correctamente";
                                Listado = conversion(_dao.selectCliente());
                            }                          
                            break;                             
                        case "servicio":
                            if (_dao.InsertServicio(DescripcionInsert, PrecioInsert))
                            {
                                MensajeInsercion = "Insercion realizada correctamente";
                                Listado = conversion(_dao.selectServicio());
                            }
                            break;
                        case "reparacion":                          
                            CodServicioRepa = _dao.selectServicioCodigo(ServicioRepa);                         
                            if (_dao.InsertReparacion(NumRepaInsert, DniClirepaInsert, MatriculaRepaInsert,CodServicioRepa, FechaRepaInser))
                            {
                                MensajeInsercion = "Insercion realizada correctamente";                               
                                Listado = conversion(_dao.selectReparacion());
                            }
                            break;
                    }
                }
                catch
                {
                    MensajeInsercion = "Insercion fallida.";                   
                }
            }
        }
        //POR AQUI  
        private void ModificacionRegistro()
        {
            if (EstadoConexion)
            {
                try
                {
                    switch (TablaAcltualListada)
                    {
                        case "cliente":
                            Cliente c = new Cliente();
                            c = (Cliente)SelecionRegistroAModificar;
                            if (_dao.UpdateCliente(c.IdCliente, DniCliMod, NombreCliMod, ApellidosCliMod, TlfCliMod, MatriculaMod, MarcaMod, ModeloMod))
                            {
                                Mensaje = "Actualizacion realizada con exito";
                                Listado = conversion(_dao.selectCliente());
                            }

                            break;
                        case "servicio":
                            Servicio s = new Servicio();
                            s = (Servicio)SelecionRegistroAModificar;
                            if (_dao.UpdateServicio(s.Codigo,DescripcionMod,PrecioMod))
                            {
                                Mensaje = "Actualizacion realizada con exito";
                                MensajeActualizacion = "Actualizacion realizada con exito";
                                Listado = conversion(_dao.selectServicio());
                            }
                            break;
                        case "reparacion":
                            
                            break;
                    }
                }
                catch
                {
                    MensajeActualizacion = "Insercion fallida.";                  
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

        public RelayCommand ModificacionRegistro_click
        {
            get { return new RelayCommand(listadoRep => ModificacionRegistro(), ListadoRep => true); }
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
->Eliminacion de registro marcando en la pestaña de listado.
-> Intentar que al actualizar un registro la pestaba tbModificaciones devuelva el foco a tbListado


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
