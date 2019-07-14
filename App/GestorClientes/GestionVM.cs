using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//añadido
using System.ComponentModel;
using System.Windows;
using System.Data.SQLite;
using System.Threading;

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
        bool _activarFiltros = false;//Activa la ventana de filtros cuandola tabla listada sea reparaciones
        #endregion

        #region Campos pestaña listar

        List<object> _listado;
        #endregion

        #region campos pestaña Añadir
        //Campos pestaña Añadir/Insertar
        int _esCorrecto = -1;//Si la insercion es 0 es que es correcto si es 1 falso
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
        DateTime _fechaRepaInser = DateTime.Now;
        int _numRepaInsert=1;

        #endregion

        #region Campos pestaña Modificar
        //Campos pestaña Modificar
        int _esCorrectoMod = -1;//Si la modificaion es 0 es que es correcto si es 1 falso
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
        //No hay Modificacion para reparaciones

        #endregion
            //POR AQUI
        #region Campos Filtros
        string _filtroDniSelecionado;
        DateTime _filtroFecha = DateTime.Now;//Luego hay que tratarla pra que sea diferente ala hora de realizar la consulta con formato 'yyyy-MM-dd'
        bool _filtrarFechaConcreta;//Filtra reparaciones de una fecha concreta
        bool _filtrarMesFecha;//Filtra reapraciones de el mes de la fecha selecionada
        bool _filtrarCalculoTotalMes;//Calcula la suma total de los precio de todos los serivicios realizados en todo el mes ischeckd
        double _resultadoCalculoTotalMes;
        string _estadoVisible = "Hidden";//Si _filtrarCalculoTotalMes no es true (osea no esta checkeado) esta propiedad valdra hidden"Es decir ocultara  el lbResultado y tbcResultado de el apartado de filtros"
        string _estadoVisiblecbxDniFiltro = "Visible"; //Al contrario que  la propiedad _estadoVisible para el componente de resultado del mes.Si _filtrarCalculoTotalMes es falso, entonces _estadoVisiblecbxDniFiltro sera visible,"Es decir el cbxDniFiltros estara visible y su titulo "DNI:" tambien
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

        public bool ActivarFiltros
        {
            get { return _activarFiltros; }
            set
            {
                if (_activarFiltros !=value)
                {
                    _activarFiltros = value;
                    Notificador("ActivarFiltros");
                }
                
            }
        }

        #endregion

        //POR AQUI
        #region Propiedades Filtros
        public string FiltroDniSelecionado
        {
            get { return _filtroDniSelecionado; }

            set
            {
                if (_filtroDniSelecionado != value)
                {
                    _filtroDniSelecionado = value;
                    Notificador("FiltroDniSelecionado");
                }
            }

        }

        public DateTime FiltroFecha
        {
            get { return _filtroFecha; }

            set
            {
                if (_filtroFecha != value)
                {
                    _filtroFecha = value;
                    Notificador("FiltroFecha");
                }
            }

        }

        public double ResultadoCalculoTotalMes
        {
            get { return _resultadoCalculoTotalMes; }

            set
            {
                if (_resultadoCalculoTotalMes != value)
                {
                    _resultadoCalculoTotalMes = value;
                    Notificador("ResultadoCalculoTotalMes");
                }
            }

        }

        public string EstadoVisible
        {
            get { return _estadoVisible; }

            set
            {
                if (_estadoVisible != value)
                {
                    _estadoVisible = value;
                    Notificador("EstadoVisible");
                }
            }

        }

        public string EstadoVisiblecbxDniFiltro
        {
            get { return _estadoVisiblecbxDniFiltro; }

            set
            {
                if (_estadoVisiblecbxDniFiltro != value)
                {
                    _estadoVisiblecbxDniFiltro = value;
                    Notificador("EstadoVisiblecbxDniFiltro");
                }
            }

        }

        //Selecion de radioButtons de filtros:

        public bool FiltrarFechaConcreta
        {
            get { return _filtrarFechaConcreta; }

            set
            {
                if (_filtrarFechaConcreta != value)
                {
                    _filtrarFechaConcreta = value;
                    Notificador("FiltrarFechaConcreta");
                }
            }
        }

        public bool FiltrarMesFecha
        {
            get { return _filtrarMesFecha; }

            set
            {
                if (_filtrarMesFecha != value)
                {
                    _filtrarMesFecha = value;
                    Notificador("FiltrarMesFecha");
                }
            }
        }

        public bool FiltrarCalculoTotalMes
        {
            get { return _filtrarCalculoTotalMes; }

            set
            {
                if (_filtrarCalculoTotalMes != value)
                {
                    _filtrarCalculoTotalMes = value;
                    Notificador("FiltrarCalculoTotalMes");
                    if (_filtrarCalculoTotalMes == true)
                    {
                        EstadoVisible = "Visible";
                        EstadoVisiblecbxDniFiltro = "Hidden";
                    }
                    else
                    {
                        EstadoVisible = "Hidden";
                        EstadoVisiblecbxDniFiltro = "Visible";
                    }
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
        
        public int EsCorrectoInsert
        {
            get { return _esCorrecto; }

            set
            {
                if (_esCorrecto != value)
                {
                    _esCorrecto = value;
                    Notificador("EsCorrectoInsert");
                }
            }
        }

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

        public DateTime FechaRepaInser
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

        public int EsCorrectoMod
        {
            get { return _esCorrectoMod; }

            set
            {
                if (_esCorrectoMod != value)
                {
                    _esCorrectoMod = value;
                    Notificador("EsCorrectoMod");
                }
            }
        }

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
                    if (_tablaAcltualListada == "reparacion")//Si la tabla listada es reparacion activa filtros si no, no
                        ActivarFiltros = true;
                    else
                        ActivarFiltros = false;
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
                    ActivarFiltros = true;
                    if (Listado.Count == 0)
                        Mensaje = "No hay registros de reparaciones actualmente.";
                    else
                        Mensaje = "";

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
                        ActivarFiltros = false;
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
                    if (Listado.Count == 0)
                        Mensaje = "No clientes registrados actualmente.";
                    else
                        Mensaje = "";
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
                    if (Listado.Count == 0)
                        Mensaje = "No hay servicios dados de alta actualmente.";
                    /*else
                        Mensaje = "";*/
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
                    if (Listado.Count == 0)
                        Mensaje = "No hay registros de reparaciones actualmente.";
                    /*else
                        Mensaje = "";*/
                }
                catch
                {
                    throw;
                }
            }
        }
        //----Fin Listado de registros---

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
                                //MensajeInsercion = "Insercion realizada correctamente";
                                EsCorrectoInsert = 0;//es correcto Para cambiar el foco a tblistado en lugar de estar en tbAñadir
                                //El mensaje informativo se da con un hilo que se ejecuta en paralelo y lo muestra durante 3 segundos en la pestaña listado ya que el foco vovlera a esta
                                Thread h1 = new Thread(new ThreadStart(MensajeInformacionInsercion));
                                h1.Start();
                                //Listado = conversion(_dao.selectCliente());
                                ListadoClientes();
                                EsCorrectoInsert = -1;//Reiniciamos la propiedad,para que en la siguiente ronda que vaya a añadir produzca de nuevo un cambio en la propiedad y ejecute el metodo de MainWindows TbxInserCorrect_TextChanged
                            }                          
                            break;                             
                        case "servicio":
                            if (_dao.InsertServicio(DescripcionInsert, PrecioInsert))
                            {
                                //MensajeInsercion = "Insercion realizada correctamente";
                                EsCorrectoInsert = 0;//es correcto Para cambiar el foco a tblistado en lugar de estar en tbAñadir
                                //El mensaje informativo se da con un hilo que se ejecuta en paralelo y lo muestra durante 3 segundos en la pestaña listado ya que el foco vovlera a esta
                                Thread h1 = new Thread(new ThreadStart(MensajeInformacionInsercion));
                                h1.Start();

                                //Listado = conversion(_dao.selectServicio());
                                ListadoServicios();
                                EsCorrectoInsert = -1;//Reiniciamos la propiedad,para que en la siguiente ronda que vaya a añadir produzca de nuevo un cambio en la propiedad y ejecute el metodo de MainWindows TbxInserCorrect_TextChanged
                            }
                            break;
                        case "reparacion":                          
                            CodServicioRepa = _dao.selectServicioCodigo(ServicioRepa);
                            /*Necesario para comprobar el numero de reparacion en el partado de los datos, ya que la comprobacion de CbxDniClienteInsert_SelectionChanged de la clase MainWindow solo afecta ala parte grafica,
                            ya que el cambio no consigue activar  una alteracion de el dato de la propiead,de esta forma queda asegurada*/
                            NumRepaInsert = _dao.selectNumRepara(DniClirepaInsert, MatriculaRepaInsert, FechaRepaInser.ToString("yyyy-MM-dd"));
                            if (NumRepaInsert >= 1)
                                NumRepaInsert++;
                            else
                                NumRepaInsert = 1;
                            if (_dao.InsertReparacion(NumRepaInsert, DniClirepaInsert, MatriculaRepaInsert,CodServicioRepa, FechaRepaInser.ToString("yyyy-MM-dd")))
                            {
                                //MensajeInsercion = "Insercion realizada correctamente";
                                EsCorrectoInsert = 0;//es correcto Para cambiar el foco a tblistado en lugar de estar en tbAñadir
                                 //El mensaje informativo se da con un hilo que se ejecuta en paralelo y lo muestra durante 3 segundos en la pestaña listado ya que el foco vovlera a esta
                                Thread h1 = new Thread(new ThreadStart(MensajeInformacionInsercion));
                                h1.Start();
                                // Listado = conversion(_dao.selectReparacion());
                                ListadoReparacion();
                                EsCorrectoInsert = -1;//Reiniciamos la propiedad,para que en la siguiente ronda que vaya a añadir produzca de nuevo un cambio en la propiedad y ejecute el metodo de MainWindows TbxInserCorrect_TextChanged
                            }
                            break;
                    }
                }
                catch
                {
                    MensajeInsercion = "Insercion fallida.";
                    EsCorrectoInsert = 1;//es falso
                    EsCorrectoInsert = -1;//Reiniciamos la propiedad,para que en la siguiente ronda que vaya a añadir produzca de nuevo un cambio en la propiedad y ejecute el metodo de MainWindows TbxInserCorrect_TextChanged
                }
            }
        }

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
                                EsCorrectoMod = 0;//es correcto  la modificacion Para cambiar el foco a tblistado en lugar de estar en tbAñadir
                                Mensaje = "Actualizacion realizada con exito";
                                //El mensaje informativo se da con un hilo que se ejecuta en paralelo y lo muestra durante 3 segundos en la pestaña listado ya que el foco vovlera a esta
                                Thread h1 = new Thread(new ThreadStart(MensajeInformacionModCorrec));
                                h1.Start();
                                Listado = conversion(_dao.selectCliente());
                                EsCorrectoMod = -1;
                                //Limpieza por si vuelve clicar en el mismo registro para m odificar justo despues de haberlo hecho antes:
                                DniCliMod = string.Empty;
                                NombreCliMod = string.Empty;
                                ApellidosCliMod = string.Empty;
                                TlfCliMod= 0;
                                MatriculaMod = string.Empty;
                                MarcaMod = string.Empty;
                                ModeloMod = string.Empty;

                            }

                            break;
                        case "servicio":
                            Servicio s = new Servicio();
                            s = (Servicio)SelecionRegistroAModificar;
                            if (_dao.UpdateServicio(s.Codigo,DescripcionMod,PrecioMod))
                            {
                                EsCorrectoMod = 0;//es correcta la modificacion Para cambiar el foco a tblistado en lugar de estar en tbAñadir
                                MensajeActualizacion = "Actualizacion realizada con exito";
                                //El mensaje informativo se da con un hilo que se ejecuta en paralelo y lo muestra durante 3 segundos en la pestaña listado ya que el foco vovlera a esta
                                Thread h1 = new Thread(new ThreadStart(MensajeInformacionModCorrec));
                                h1.Start();
                                Listado = conversion(_dao.selectServicio());
                                EsCorrectoMod = -1;
                                //Limpieza por si vuelve clicar en el mismo registro para m odificar justo despues de haberlo hecho antes:
                                DescripcionMod = string.Empty;
                                PrecioMod = 0;
                            }
                            break;
                    }
                }
                catch
                {
                    MensajeActualizacion = "Insercion fallida.";
                    Thread h1 = new Thread(new ThreadStart(MensajeInformacionModIncorrec));
                    h1.Start();
                }
            }
        }
       //Este metodo refresca el listado despues de haber eliminado los registros desde la clase MainWindow en el metodo BtnEliminar_Click
        private void EliminarRegistro()
        {
            if (EstadoConexion)
            {
                try
                {                  
                    switch (TablaAcltualListada)
                    {
                        case "cliente":                    
                                Listado = conversion(_dao.selectCliente());
                            break;
                        case "servicio":
                         
                                Listado = conversion(_dao.selectServicio());
                            break;
                        case "reparacion":
                            Listado = conversion(_dao.selectReparacion());
                            break;
                    }
                    Thread h1 = new Thread(new ThreadStart(MensajeInformacionEliminacionCorrecta));
                    h1.Start();

                }
                catch
                {
                    MensajeActualizacion = "Eliminacion fallida.";
                }
            }
        }

        private void VolverAtrasInsert()
        {
            #region Limpieza de propiedades
            //Cliente
            MensajeInsercion = string.Empty;
            DniCliInsert = string.Empty;
            NombreCliInsert = string.Empty;
            ApellidosCliInsert = string.Empty;
            MatriculaInsert = string.Empty;
            MarcaInsert = string.Empty;
            ModeloInsert = string.Empty;
            //TlfCliInsert = int.Parse(string.Empty);
            TlfCliInsert = 0;
            //Servicio
            DescripcionInsert = string.Empty;
            PrecioInsert = 0;

            //Reparacion
            DniClirepaInsert = string.Empty;
            MatriculaRepaInsert = string.Empty;
            ServicioRepa = string.Empty;
            FechaRepaInser = DateTime.Now;
            CodServicioRepa = 0;
            NumRepaInsert = 1;
            EsCorrectoInsert = -1;

            //Otros
            Mensaje = string.Empty;
            #endregion

        }

        private void VolverAtrasMod()
        {
            #region Limpieza de propiedades
            //Cliente
            MensajeActualizacion = string.Empty;
            DniCliMod = string.Empty;
            NombreCliMod = string.Empty;
            ApellidosCliMod = string.Empty;
            MatriculaMod = string.Empty;
            MarcaMod = string.Empty;
            ModeloMod = string.Empty;
            //TlfCliInsert = int.Parse(string.Empty);
            TlfCliMod = 0;
            //Servicio
            DescripcionMod = string.Empty;
            PrecioMod = 0;
           
            //Reparacions(No hay modificaciones de reparaciones)

            //Otros
            Mensaje = string.Empty;
            #endregion
        }

        private void RestablecerFiltros()
        {
            FiltroDniSelecionado = null;
            FiltrarCalculoTotalMes = false;
            FiltrarFechaConcreta = false;
            FiltrarMesFecha = false;
            FiltroFecha = DateTime.Now;
            Listado = conversion(_dao.selectReparacion());
        }

        //POR AQUI
        private void AplicarFiltros()
        {
            if (FiltrarFechaConcreta && FiltroDniSelecionado != null)
                Listado = conversion(_dao.selectReparacionFiltroFecha(FiltroDniSelecionado, FiltroFecha.ToString("yyyy-MM-dd")));
            else if(FiltrarFechaConcreta && FiltroDniSelecionado is null)
                Listado = conversion(_dao.selectReparacionFiltroFecha(FiltroFecha.ToString("yyyy-MM-dd")));

            if (FiltrarMesFecha && FiltroDniSelecionado!=null)
                Listado = conversion(_dao.selectReparacionFiltroFechaMes(FiltroDniSelecionado, FiltroFecha.ToString("yyyy-MM-dd")));
            else if(FiltrarMesFecha && FiltroDniSelecionado is null)
                Listado = conversion(_dao.selectReparacionFiltroFechaMes(FiltroFecha.ToString("yyyy-MM-dd")));

            if (FiltrarCalculoTotalMes)
            {
                ResultadoCalculoTotalMes = _dao.selectReparacionFiltroCalculoMes(FiltroFecha.ToString("yyyy-MM-dd"));
                Listado = conversion(_dao.selectReparacionFiltroFechaMes(FiltroFecha.ToString("yyyy-MM-dd")));
            }
        }

        #endregion

        #region Metodos para propiedades Command de los componentes de la ventana
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
            get { return new RelayCommand(listadoRep => ListadoReparacion(), ListadoRep => true); }           
        }

        public RelayCommand InsertarRegistro_click
        {
            get { return new RelayCommand(insertRegis => InsertarRegistro(), ListadoRep => true); }
        }
   
        public RelayCommand ModificacionRegistro_click
        {
            get { return new RelayCommand(listadoRep => ModificacionRegistro(), ListadoRep => true); }
        }

        public RelayCommand EliminarRegistro_click
        {
            get { return new RelayCommand(EliminarRegis => EliminarRegistro(), EliminarRegis => true); }
        }

        public RelayCommand VolverInsert_click
        {
            get { return new RelayCommand(volver => VolverAtrasInsert(), volver => true); }
        }

        public RelayCommand VolverMod_click
        {
            get { return new RelayCommand(volver => VolverAtrasMod(), volver => true); }
        }

        public RelayCommand RestablecerFiltros_click
        {
            get { return new RelayCommand(restablecerF => RestablecerFiltros(), restablecerF => true); }
        }

        public RelayCommand AplicarFiltros_click
        {
            get { return new RelayCommand(aplicarF => AplicarFiltros(), aplicarF => true); }
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

        #region Metodos llamados por los hilos de ejecucion paralela en los mensajes de insercion y modificacion
        private void MensajeInformacionInsercion()
        {           
            Mensaje = "Insercion Correcta";
            Thread.Sleep(3000);
            Mensaje = string.Empty;
            
        }

        private void MensajeInformacionModCorrec()
        {          
            Mensaje = "Modificacion realizada Correctamente";
            Thread.Sleep(3000);
            Mensaje = string.Empty;

        }

        private void MensajeInformacionModIncorrec()
        {
            Mensaje = "Error al realizar la modificacion";
            Thread.Sleep(3000);
            Mensaje = string.Empty;

        }

        private void MensajeInformacionEliminacionCorrecta()
        {
            Mensaje = "Se ha eliminado con existo los registros indicados";
            Thread.Sleep(3000);
            Mensaje = string.Empty;

        }
        #endregion
      
    }
}
/*
//ERRORES:
1-

 //Tareas pendientes:
 ---Actualmente en curso---
1-> controlar que la propiedad _tlfInsert en su textbox correspondiente
no pueda añadirse otra cosa que no sean numeros.
2-> Intentar que al actualizar un registro la pestaba tbModificaciones devuelva el foco a tbListado
3-> Comprobar detalles  de insercion o modificacion como que en la tabla cliente que ni el dni ni la matricula esten vacios
y especificarlo en caso de fallo,el numero d tlf solo puede ser un campo  numerico.
En la tabla servicios a la hora de insertar la descripcion no puede tener numeros ni el precio letras.
4-> filtros.
5-> intentar que los mensajes se muestren solo durante unos segundos y luego cambie(Hilos??)

--En cola---
 -Filtros
 -Generar Factura()
 -Copia de seguridad BD automatica

//Tareas Finalizadas:
 -Conectar
 -Inserciones
 -Modificaciones.
 -Eliminacion.
 -Ver otros registros


//Extras:
1-Control de DniClientes solo puede tener una letra y debe ser el ultimo caracter "controlar en codigo o en un trigger de la BD"
2-Controlar  insercion de fechas pasadas



 */
