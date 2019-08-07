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
//PFD y Factuas
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Windows.Forms;
//Para usar process y arrancar applicaciones ,como cuando creemos un pdf
using System.Diagnostics;
using MessageBox = System.Windows.Forms.MessageBox;
using GestorClientes.Properties;

namespace GestorClientes
{
    class GestionVM : INotifyPropertyChanged
    {

        //static string rutaImg = @"../../img/cartel.png";
        #region Variables
        string colorRojo = "#FFD66E6E";
        string colorAzul = "#FF45A3CF";
        //public DaoSqlite _dao = new DaoSqlite();
        public DaoSql _dao = new DaoSql();
        List<string> listadoTablasBD = new List<string>(); //Listado de tablas de la BD en la que si se puede insertar manualmente por le usuario (cliente,servicio,reparacion) PERO NO FACTURA de hay que no lo cojamos de el  show tables, como haciamos antes
        List<Factura> listLineasFacturaSutituta = new List<Factura>();//Listado de datos de la factura sustituta para anular una factura y sustituirla por la nueva con nuevas lineas en ModificacionRegistro
        List<Reparacion> listReparacionesPorAnadir = new List<Reparacion>();//Conjunto de reparaciones en la pestaña  añadir, para evitar que el usuario este constantemente cambiando de pestaña al añadir una reparaciond eun cliente para el mismo dia
        #endregion

        #region campos

        #region campos generales 
        bool _habilitado = false;//Habilita o deshabilita ciertos botones o opciones
        string _mensaje;//Mensaje de informacion     
        string _conectadoDesconectado = "Conectar";//Nombre que daremos al boton de conexion y desconexion segun el estado de dicha conexion
        bool _estadoConexion = false;//true abierta, false cerrada
        string _colorConexion = "#FF45A3CF";
        bool _activarFiltros = false;//Activa la ventana de filtros cuandola tabla listada sea reparaciones
        string _tablaMostraEnListado = string.Empty;

        string _ocultarBtnsActionEdiUpDelete = "Visible";//Oculta el conjunto de botones donde estan los 3 putitos que desplega el boton de editar borrar y crear o insertar

        string _ocultarBtnsAnularFacturaYRecrearFactura = "Hidden";//Oculta el conjunto de botones donde estan los 3 putitos que desplega el boton de editar borrar y crear o insertar


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
        int _IdCliInsert;
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
        int _idClirepaInsert=0;//Seleciona un string de un idCliente de cliente y a partir de el buscamos el id luego internamente almacenandolo en _idClienteRepaInsert
        string _matriculaRepaInsert;
        string _ServicioRepa;//Seleciona un string de servicio y a partir de el buscamos el cod luego internamente almacenandolo en _CodServicioRepa
        int _CodServicioRepa;
        DateTime _fechaRepaInser = DateTime.Now;
        int _numRepaInsert = 1;

        bool _bloquearCbxIdCliInsertRepa = true;
        bool _bloquearCbxMatriculaInsertRepa = true;
        bool _bloquearCbxFechaInsertRepa = true;


        #endregion

        #region Campos pestaña Modificar
        //Campos pestaña Modificar
        int _esCorrectoMod = -1;//Si la modificaion es 0 es que es correcto si es 1 falso
        bool _habilitarModificaciones = false;//Deshabilitado hasta que se marque un registro y se pinche en el boton modificar
        string _tablaAcltualListada;//Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
        object _selecionRegistroAModificar = null; //Propiedad para el registro selecionado
        string _mensajeActualizacion = "";

        //Datos Modificar Cliente
        int _idCliMod;//CAMBIADO
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

        #region Campos Filtros
        string _filtroMatriculaSelecionado;
        DateTime _filtroFecha = DateTime.Now;//Luego hay que tratarla pra que sea diferente ala hora de realizar la consulta con formato 'yyyy-MM-dd'
        bool _filtrarFechaConcreta;//Filtra reparaciones de una fecha concreta
        bool _filtrarMesFecha;//Filtra reapraciones de el mes de la fecha selecionada
        bool _filtrarCalculoTotalMes;//Calcula la suma total de los precio de todos los serivicios realizados en todo el mes ischeckd
        double _resultadoCalculoTotalMes;
        string _estadoVisible = "Hidden";//Si _filtrarCalculoTotalMes no es true (osea no esta checkeado) esta propiedad valdra hidden"Es decir ocultara  el lbResultado y tbcResultado de el apartado de filtros"
        string _estadoVisiblecbxIDFiltro = "Visible"; //Al contrario que  la propiedad _estadoVisible para el componente de resultado del mes.Si _filtrarCalculoTotalMes es falso, entonces _estadoVisiblecbxIDFiltro sera visible,"Es decir el cbxIDFiltros estara visible y su titulo "ID:" tambien
        List<int> _filtroListIdCliente;//Coleccion de IdCliente que llenaran el itemSource de el filtro ID de Cliente
        string _filtroIDClienteSelecionado;
        int _filtroSelectIndexIdCliente;//Si la lista<int> de la propieda _filtroListIdCliente contine solo un elemento count =1 entonces esta propiedad FiltroSelectIndexIdCliente cambiara el selecteIndex de el despleagable de IdCliente de los filtros a 0, para marcar automaticamente el unico valor disponible
        string _filtroNombreCliente;
        //Este campo funcionara tanto si estas en el listado reparaciones y filtro por una fecha e idcliente como  si esta en facturas y filtro del mismo modo
        string _visibleBtnExtraerFacturasPdf = "Hidden";//Activa o desactiva el boton de extraer  facturas, el cual solo se activara cuando los filtros marcados sean tanto filtro por mes como la matricula y el id del cliente
        #endregion

        #region Campos Factura

        //Datos Modificar Añadiendo una nueva que sustituye la anterior Factura 
        int _numeroFactura;
        int _numeroFacturaSustituta;
        int _idClienteFactura =-1;
        string _matriculaFactura;
        string _ServicioFactura;//Seleciona un string de servicio y a partir de el buscamos el cod luego internamente almacenandolo en _CodServicioRepa
        List<string> _listComboboxServicioFactura;//lista de combobox de servicios realizados(itemSource)
        int _CodServicioFactura;
        DateTime _fechaFactura = DateTime.Now;
        int _lienaFactura = 1;
        List<string> _idclientesComboboxFactura = new List<string>();//lista que recarga el itemsource de el combobox de de idCliente cuando se va anular una factura creando otra.
        bool _bloquearCbxIdClienteFactura = true;
        bool _bloquearCbxMatriculaFactura = true;
        bool _bloquearDataPickerFechaFactura = true;


        #endregion

        #region Campos Extraer CSV de Factura
        bool _cbxCSVTodasLasFacturas;
        bool _cbxCSVfacturaExtracfiltrarPorUnMes;
        bool _activarDtpFacturasMesExtraerCSV = false;//datapicker para filtrar Facturas de un mes y pasar a csv
        bool _activarBtnExtraerCSV = false;
        DateTime _fechaMesExtraerFacturasCsv = DateTime.Now;
        #endregion

        #endregion
        #region Propiedades

        #region Propiedades Generales
        public bool Habilitado
        {
            get { return _habilitado; }
            set
            {
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
        public string Mensaje
        {
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
        public bool EstadoConexion
        {
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
                    _conectadoDesconectado = "Desconectar";//Si el estado de la conecxion es verdad es decir  hay conexion, el boton muestra la palabra desconectar
                    Notificador("ConectadoDesconectado");
                    Habilitado = true;

                }
                else
                {
                    _conectadoDesconectado = "Conectar";//Si el estado dela conecxion es falso es decir no hay conexion, el boton muestra la palabra conectar
                    Notificador("ConectadoDesconectado");
                    Habilitado = false;



                }
            }
        }
        public string ColorConexion
        {
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
                if (_activarFiltros != value)
                {
                    _activarFiltros = value;
                    Notificador("ActivarFiltros");
                }

            }
        }

        public string OcultarBtnsActionEdiUpDelete
        {
            get { return _ocultarBtnsActionEdiUpDelete; }

            set
            {
                if (_ocultarBtnsActionEdiUpDelete != value)
                {
                    _ocultarBtnsActionEdiUpDelete = value;
                    Notificador("OcultarBtnsActionEdiUpDelete");
                }
            }

        }

        public string OcultarBtnsAnularFacturaYRecrearFactura
        {
            get { return _ocultarBtnsAnularFacturaYRecrearFactura; }

            set
            {
                if (_ocultarBtnsAnularFacturaYRecrearFactura != value)
                {
                    _ocultarBtnsAnularFacturaYRecrearFactura = value;
                    Notificador("OcultarBtnsAnularFacturaYRecrearFactura");
                }
            }

        }

        //Indica al usuario que tabla se esta viendo en el apartado listado
        public string TablaMostraEnListado
        {
            get { return _tablaMostraEnListado; }

            set
            {
                if (_tablaMostraEnListado != value)
                {
                    switch (value)
                    {
                        case "cliente":
                            value = "Clientes";
                            OcultarBtnsAnularFacturaYRecrearFactura = "Hidden";
                            OcultarBtnsActionEdiUpDelete = "Visible";
                            break;
                        case "servicio":
                            value = "Servicios";
                            OcultarBtnsAnularFacturaYRecrearFactura = "Hidden";
                            OcultarBtnsActionEdiUpDelete = "Visible";
                            break;
                        case "reparacion":
                            value = "Reparaciones";
                            OcultarBtnsAnularFacturaYRecrearFactura = "Hidden";
                            OcultarBtnsActionEdiUpDelete = "Visible";
                            break;
                        case "factura":
                            value = "Facturas";
                            OcultarBtnsAnularFacturaYRecrearFactura = "Visible";
                            OcultarBtnsActionEdiUpDelete = "Hidden";
                            break;
                    }
                    _tablaMostraEnListado = value;
                    Notificador("TablaMostraEnListado");

                }
            }

        }

        #endregion

        #region Propiedades Filtros
        public string FiltroMatriculaSelecionado
        {
            get { return _filtroMatriculaSelecionado; }

            set
            {
                if (_filtroMatriculaSelecionado != value)
                {
                    _filtroMatriculaSelecionado = value;
                    Notificador("FiltroMatriculaSelecionado");
                    FiltroListIdCliente = _dao.selectIdClienteClientes(_filtroMatriculaSelecionado);
                }
            }

        }

        //Esta se llena cuando la selecion de filtro matricula cambia 
        public List<int> FiltroListIdCliente
        {
            get { return _filtroListIdCliente; }

            set
            {
                if (_filtroListIdCliente != value)
                {
                    _filtroListIdCliente = value;
                    Notificador("FiltroListIdCliente");
                    if (FiltroListIdCliente.Count == 1)
                        FiltroSelectIndexIdCliente = 0;
                    else
                        FiltroSelectIndexIdCliente = -1;

                }
            }

        }

        public string FiltroIDClienteSelecionado
        {
            get { return _filtroIDClienteSelecionado; }

            set
            {
                if (_filtroIDClienteSelecionado != value)
                {
                    _filtroIDClienteSelecionado = value;
                    Notificador("FiltroIDClienteSelecionado");
                    if (_filtroIDClienteSelecionado != null)
                    {
                        FiltroNombreCliente = _dao.selectClienteNombre(int.Parse(_filtroIDClienteSelecionado));
                    }
                    else
                    {
                        FiltroNombreCliente = string.Empty;
                        VisibleBtnExtraerFacturasPdf = "Hidden";
                    }
                }
            }

        }

        public string FiltroNombreCliente
        {
            get { return _filtroNombreCliente; }

            set
            {
                if (_filtroNombreCliente != value)
                {
                    _filtroNombreCliente = value;
                    Notificador("FiltroNombreCliente");
                }
            }

        }

        public int FiltroSelectIndexIdCliente
        {
            get { return _filtroSelectIndexIdCliente; }

            set
            {
                if (_filtroSelectIndexIdCliente != value)
                {
                    _filtroSelectIndexIdCliente = value;
                    Notificador("FiltroSelectIndexIdCliente");
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

        public string EstadoVisiblecbxIDFiltro
        {
            get { return _estadoVisiblecbxIDFiltro; }

            set
            {
                if (_estadoVisiblecbxIDFiltro != value)
                {
                    _estadoVisiblecbxIDFiltro = value;
                    Notificador("EstadoVisiblecbxIDFiltro");
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
                    VisibleBtnExtraerFacturasPdf = "Hidden";//Desactivar boton de emitir factura en cuanto marca esta obcion de filtrado de los 3 raiobutton posibles
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
                        EstadoVisiblecbxIDFiltro = "Hidden";
                    }
                    else
                    {
                        EstadoVisible = "Hidden";
                        EstadoVisiblecbxIDFiltro = "Visible";
                    }
                    VisibleBtnExtraerFacturasPdf = "Hidden";//Desactivar boton de emitir factura en cuanto marca esta obcion de filtrado de los 3 raiobutton posibles
                }
            }
        }

        public string VisibleBtnExtraerFacturasPdf
        {
            get { return _visibleBtnExtraerFacturasPdf; }

            set
            {
                if (_visibleBtnExtraerFacturasPdf != value)
                {
                    _visibleBtnExtraerFacturasPdf = value;
                    Notificador("VisibleBtnExtraerFacturasPdf");

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
        public int IdCliInsert//CAMBIADO
        {
            get { return _IdCliInsert; }
            set
            {
                if (_IdCliInsert != value)
                {
                    _IdCliInsert = value;
                    Notificador("IdCliInsert");
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

        public int IdClirepaInsert
        {
            get { return _idClirepaInsert; }
            set
            {
                if (_idClirepaInsert != value)
                {
                    _idClirepaInsert = value;
                    Notificador("IdClirepaInsert");
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

        //Esteticos Anadir Reparacion 

        public bool BloquearCbxIdCliInsertRepa
        {
            get { return _bloquearCbxIdCliInsertRepa; }
            set
            {
                if (_bloquearCbxIdCliInsertRepa != value)
                {
                    _bloquearCbxIdCliInsertRepa = value;
                    Notificador("BloquearCbxIdCliInsertRepa");
                }
            }
        }

        public bool BloquearCbxMatriculaInsertRepa
        {
            get { return _bloquearCbxMatriculaInsertRepa; }
            set
            {
                if (_bloquearCbxMatriculaInsertRepa != value)
                {
                    _bloquearCbxMatriculaInsertRepa = value;
                    Notificador("BloquearCbxMatriculaInsertRepa");
                }
            }
        }

        public bool BloquearCbxFechaInsertRepa
        {
            get { return _bloquearCbxFechaInsertRepa; }
            set
            {
                if (_bloquearCbxFechaInsertRepa != value)
                {
                    _bloquearCbxFechaInsertRepa = value;
                    Notificador("BloquearCbxFechaInsertRepa");
                }
            }
        }

        #endregion

        #endregion

        //Solo propiedades para anular o modificar factura, ya que la insercion en esta tabla se realiza cuando el usuario crea el pdf de la factura al filtrar las reparaciones de un cliente en un dia concreto con una matricula concreta
        #region Propiedades Anular o modificar Factura

        public int NumeroFactura
        {
            get { return _numeroFactura; }

            set
            {
                if (_numeroFactura != value)
                {
                    _numeroFactura = value;
                    Notificador("NumeroFactura");
                }
            }

        }
        public int NumeroFacturaSustituta
        {
            get { return _numeroFacturaSustituta; }

            set
            {
                if (_numeroFacturaSustituta != value)
                {
                    _numeroFacturaSustituta = value;
                    Notificador("NumeroFacturaSustituta");
                }
            }

        }
        public int IdClienteFactura
        {
            get { return _idClienteFactura; }

            set
            {
                if (_idClienteFactura != value)
                {
                    _idClienteFactura = value;
                    Notificador("IdClienteFactura");
                }
            }

        }
        public string MatriculaFactura
        {
            get { return _matriculaFactura; }

            set
            {
                if (_matriculaFactura != value)
                {
                    _matriculaFactura = value;
                    Notificador("MatriculaFactura");
                }
            }

        }
        public string ServicioFactura
        {
            get { return _ServicioFactura; }

            set
            {
                if (_ServicioFactura != value)
                {
                    _ServicioFactura = value;
                    Notificador("ServicioFactura");
                }
            }

        }
        public int CodServicioFactura
        {
            get { return _CodServicioFactura; }

            set
            {
                if (_CodServicioFactura != value)
                {
                    _CodServicioFactura = value;
                    Notificador("CodServicioFactura");
                }
            }

        }
        public DateTime FechaFactura
        {
            get { return _fechaFactura; }

            set
            {
                if (_fechaFactura != value)
                {
                    _fechaFactura = value;
                    Notificador("FechaFactura");
                }
            }

        }
        public int LineaFactura
        {
            get { return _lienaFactura; }

            set
            {
                if (_lienaFactura != value)
                {
                    _lienaFactura = value;
                    Notificador("LienaFactura");
                }
            }

        }

        //Para recargar el combobox de idCliente cuando se ha insertado una linea de factura y queremos restablecer todos los datos a 0 para la siguiente linea
        public List<string> IdclientesComboboxFactura
        {
            get { return _idclientesComboboxFactura; }

            set
            {
                if (_idclientesComboboxFactura != value)
                {
                    _idclientesComboboxFactura = value;
                    Notificador("IdclientesComboboxFactura");

                }
            }

        }

        public List<string> ListComboboxServicioFactura
        {
            get { return _listComboboxServicioFactura; }

            set
            {
                if (_listComboboxServicioFactura != value)
                {
                    _listComboboxServicioFactura = value;
                    Notificador("ListComboboxServicioFactura");

                }
            }

        }

        public bool BloquearCbxIdClienteFactura
        {
            get { return _bloquearCbxIdClienteFactura; }
            set
            {
                if (_bloquearCbxIdClienteFactura != value)
                {
                    _bloquearCbxIdClienteFactura = value;
                    Notificador("BloquearCbxIdClienteFactura");
                }
            }
        }

        public bool BloquearCbxMatriculaFactura
        {
            get { return _bloquearCbxMatriculaFactura; }
            set
            {
                if (_bloquearCbxMatriculaFactura != value)
                {
                    _bloquearCbxMatriculaFactura = value;
                    Notificador("BloquearCbxMatriculaFactura");
                }
            }
        }

        public bool BloquearDataPickerFechaFactura
        {
            get { return _bloquearDataPickerFechaFactura; }
            set
            {
                if (_bloquearDataPickerFechaFactura != value)
                {
                    _bloquearDataPickerFechaFactura = value;
                    Notificador("BloquearDataPickerFechaFactura");
                }
            }
        }



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
                if (_tablaAcltualListada != value)
                {
                    _tablaAcltualListada = value;
                    Notificador("TablaAcltualListada");
                    if (_tablaAcltualListada == "reparacion" || _tablaAcltualListada == "factura")//Si la tabla listada es reparacion activa filtros si no, no
                        ActivarFiltros = true;
                    else
                        ActivarFiltros = false;

                    TablaMostraEnListado = value;
                }

            }

        }

        public object SelecionRegistroAModificar
        {
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
                                IdCliMod = c.IdCliente;
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
                            case "factura"://Cuando selecionamos una factura para modificarla(Solo dejamo en el apartado de modificacion el numero de factura a anular y el dela sustituta el resto se queda vacio como si de una insercion nueva se tratara) 
                                Factura f = new Factura();
                                f = (Factura)_selecionRegistroAModificar;
                                /*Colocar datos de una factura selecionada en las distintas porpiedades de una factura
                                Y numero de factura sustituta sera  una consulta de un select max(numeroFactura) el cual sacara el numero mas grande de la tabla facturas al que sumaremos uno,
                                que sera el numero de la nueva factura*/
                                NumeroFactura = f.NumeroFactura;
                                NumeroFacturaSustituta = _dao.selectUltimoNumeroFactura();

                                break;


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
        public int IdCliMod
        {
            get { return _idCliMod; }
            set
            {
                if (_idCliMod != value)
                {
                    _idCliMod = value;
                    Notificador("IdCliMod");
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

        #region Propiedades extraer CSV de la tabla Facturas
        public bool CbxCSVTodasLasFacturas
        {
            get { return _cbxCSVTodasLasFacturas; }
            set
            {
                if (_cbxCSVTodasLasFacturas != value)
                {
                    _cbxCSVTodasLasFacturas = value;
                    if (value == true)
                    {
                        ActivarBtnExtraerCSV = true;
                        ActivarDtpFacturasMesExtraerCSV = false;
                    }
                    Notificador("CbxCSVTodasLasFacturas");
                }

            }
        }

        //Extraer todas las facturas de un mes
        public bool CbxCSVfacturaExtracfiltrarPorUnMes
        {
            get { return _cbxCSVfacturaExtracfiltrarPorUnMes; }
            set
            {
                if (_cbxCSVfacturaExtracfiltrarPorUnMes != value)
                {
                    _cbxCSVfacturaExtracfiltrarPorUnMes = value;
                    if (value == true)
                        ActivarDtpFacturasMesExtraerCSV = true;
                    if (value == true && FechaMesExtraerFacturasCsv != null)
                    {
                        ActivarDtpFacturasMesExtraerCSV = true;
                        ActivarBtnExtraerCSV = true;
                    }
                    if (value == true && FechaMesExtraerFacturasCsv == null)
                    {
                        ActivarDtpFacturasMesExtraerCSV = true;
                        ActivarBtnExtraerCSV = false;
                    }
                    Notificador("CbxCSVfacturaExtracfiltrarPorUnMes");
                }

            }
        }

        //Activa el datapicker de extraer facturas de un mes cuando el radioButton de Extraer todas las facturas de un mes esta marcado
        public bool ActivarDtpFacturasMesExtraerCSV
        {
            get { return _activarDtpFacturasMesExtraerCSV; }
            set
            {
                if (_activarDtpFacturasMesExtraerCSV != value)
                {
                    _activarDtpFacturasMesExtraerCSV = value;
                    Notificador("ActivarDtpFacturasMesExtraerCSV");
                }

            }
        }

        //Activa el boton cuando el primer radiobutton esta marcado o cuando el segundo lo esta y hayuna fecha escogida
        public bool ActivarBtnExtraerCSV
        {
            get { return _activarBtnExtraerCSV; }
            set
            {
                if (_activarBtnExtraerCSV != value)
                {
                    _activarBtnExtraerCSV = value;
                    Notificador("ActivarBtnExtraerCSV");
                }

            }
        }

        public DateTime FechaMesExtraerFacturasCsv
        {
            get { return _fechaMesExtraerFacturasCsv; }
            set
            {
                if (_fechaMesExtraerFacturasCsv != value)
                {
                    _fechaMesExtraerFacturasCsv = value;
                    Notificador("FechaMesExtraerFacturasCsv");
                }

            }
        }

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
                    Listado = Conversion(_dao.selectReparacion());

                    ConectadoDesconectado = "Desconectar";//Ya que esta conectado y este boton ademas lo mostraremos en rojo.. y cuando este desconectado, mostraremos la palabra'conectar' con el fondo verde
                    ColorConexion = colorRojo;
                    //Listado de tablas de la BD en la que si se puede insertar manualmente por le usuario (cliente,servicio,reparacion) PERO NO FACTURA de hay que no lo cojamos de el  show tables, como haciamos antes
                    //Listablas = _dao.VerTablas();
                    listadoTablasBD.Add("cliente");
                    listadoTablasBD.Add("servicio");
                    listadoTablasBD.Add("reparacion");
                    Listablas = listadoTablasBD;


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
                    Listado = Conversion(_dao.selectCliente());
                    //Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
                    TablaAcltualListada = "cliente";
                    if (Listado.Count == 0)
                        Mensaje = "No clientes registrados actualmente.";
                    /*else
                        Mensaje = "";*/
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
                    Listado = Conversion(_dao.selectServicio());
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
                    Listado = Conversion(_dao.selectReparacion());
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

        private void ListadoFacturas()
        {
            if (EstadoConexion)
            {
                try
                {
                    Listado = Conversion(_dao.selectFacturas());
                    //Por si se quiere modificar un registro de ese listado saber de que tabla vamos a modificar dicho registro
                    TablaAcltualListada = "factura";
                    if (Listado.Count == 0)
                        Mensaje = "No hay registros de facturas actualmente.";
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
                            if (_dao.InsertCliente(NombreCliInsert, ApellidosCliInsert, TlfCliInsert, MatriculaInsert, MarcaInsert, ModeloInsert))
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
                            int totalDeReparaciones = listReparacionesPorAnadir.Count;

                            if (totalDeReparaciones == 0)
                            {
                                System.Windows.MessageBox.Show("No has indicado los datos para ninguan reparación aun.", "(◑ω◐)¡Ops!.");
                                break;
                            }

                            int contador = 0;
                            foreach (Reparacion reparacion in listReparacionesPorAnadir)
                            {
                                if (_dao.InsertReparacion(reparacion.NumReparacion, reparacion.IdCliente, reparacion.MatriCoche, reparacion.CodServicio, reparacion.Fecha))//.ToString("yyyy-MM-dd")
                                    contador++;
                                else
                                {                                    
                                    System.Windows.MessageBox.Show("Ops,Lo sentimos pero ocurrio un error inesperado.\nPongase en contacto con el administrador." , "(◑ω◐)¡Ops!.");
                                }
                            }

                            if (contador==totalDeReparaciones)
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

                            //Limpiamos la lista de elementos, para futuras reparaciones que deseomos añadir al volver a la pestaña añadir selecionado reparacion
                            listReparacionesPorAnadir.Clear();

                            //Al terminar la operacion en esta pestaña habilitamos de nuevo todos los combobox bloqueados
                            BloquearCbxIdCliInsertRepa = true;
                            BloquearCbxFechaInsertRepa = true;
                            BloquearCbxMatriculaInsertRepa = true;
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

        //Crea una lista de reparacion que sera añadida al dar al boton Insertar(de forma que el cliente no deba estar constantemente cambiando de ventana para insertar varias reparaciones a un mismo cliente)
        private void AnadirReparacionALalistaDePreparacion()
        {
            //Añadimos la linea de factura de la pestaña modificaciones al listado de listLineasFacturaAanular
            try
            {
                if (IdClirepaInsert == 0 || MatriculaRepaInsert is null || ServicioRepa is null)
                {
                    System.Windows.MessageBox.Show("Debes selecionar siempre un cliente,una matricula,una fecha y un servicio.", "(◑ω◐)¡Ops!.");
                }
                else 
                {
                    int cantidad = listReparacionesPorAnadir.Count;
                    if (cantidad == 0)//La primera vez, obtenemos el maximo numero de reparacion para un determinado cliente con un coche en una determinada fecha concreta
                    {
                        /*Necesario para comprobar el numero de reparacion en el apartado de los datos, ya que la comprobacion de CbxIdClienteInsert_SelectionChanged de la clase MainWindow solo afecta ala parte grafica,
                              ya que el cambio no consigue activar  una alteracion de el dato de la propiead,de esta forma queda asegurada*/
                        NumRepaInsert = _dao.selectNumRepara(IdClirepaInsert, MatriculaRepaInsert, FechaRepaInser.ToString("yyyy-MM-dd"));
                        if (NumRepaInsert >= 1)
                            NumRepaInsert++;
                        else
                            NumRepaInsert = 1;
                    }
                    else//Si ya hicimos lo anterior, solo incrementamos el numero de reparacion en 1 mas
                    {
                        NumRepaInsert++;
                        BloquearCbxIdCliInsertRepa = false;
                        BloquearCbxFechaInsertRepa = false;
                        BloquearCbxMatriculaInsertRepa = false;
                    }


                    CodServicioRepa = _dao.selectServicioCodigo(ServicioRepa);

                    Reparacion r = new Reparacion();
                    r.NumReparacion = NumRepaInsert;
                    r.IdCliente = IdClirepaInsert;
                    r.MatriCoche = MatriculaRepaInsert;
                    r.CodServicio = CodServicioRepa;
                    r.Fecha = FechaRepaInser.ToString("yyyy-MM-dd");

                    //Añadimos la nueva reparacion a la lista que sera la que se recorrera e insertara cuando se confirme los cambios en ModificacionRegistro en el case "reparacion"
                    listReparacionesPorAnadir.Add(r);

                    BloquearCbxIdCliInsertRepa = false;
                    BloquearCbxFechaInsertRepa = false;
                    BloquearCbxMatriculaInsertRepa = false;

                    //Recargamos el valor por defecto de todos los datos para la siguiente linea de factura que se inserte:
                    ServicioRepa = null;

                    //Mensaje de linea insertada
                    Thread h1 = new Thread(new ThreadStart(MensjInfoReparacionAnadidaEnListaDeInsercion));
                    h1.Start();
                }
                

            }
            catch (Exception e)
            {
                Mensaje = "ERROR: " + e.Message;
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
                            if (_dao.UpdateCliente(c.IdCliente, NombreCliMod, ApellidosCliMod, TlfCliMod, MatriculaMod, MarcaMod, ModeloMod))
                            {
                                EsCorrectoMod = 0;//es correcto  la modificacion Para cambiar el foco a tblistado en lugar de estar en tbAñadir
                                //Mensaje = "Actualizacion realizada con exito";
                                //El mensaje informativo se da con un hilo que se ejecuta en paralelo y lo muestra durante 3 segundos en la pestaña listado ya que el foco vovlera a esta
                                Thread h1 = new Thread(new ThreadStart(MensajeInformacionModCorrec));
                                h1.Start();
                                Listado = Conversion(_dao.selectCliente());
                                EsCorrectoMod = -1;
                                //Limpieza por si vuelve clicar en el mismo registro para m odificar justo despues de haberlo hecho antes:
                                IdCliMod = 0;
                                NombreCliMod = string.Empty;
                                ApellidosCliMod = string.Empty;
                                TlfCliMod = 0;
                                MatriculaMod = string.Empty;
                                MarcaMod = string.Empty;
                                ModeloMod = string.Empty;

                            }
                            break;

                        case "servicio":
                            Servicio s = new Servicio();
                            s = (Servicio)SelecionRegistroAModificar;
                            if (_dao.UpdateServicio(s.Codigo, DescripcionMod, PrecioMod))
                            {
                                EsCorrectoMod = 0;//es correcta la modificacion Para cambiar el foco a tblistado en lugar de estar en tbAñadir
                                                  // MensajeActualizacion = "Actualizacion realizada con exito";
                                                  //El mensaje informativo se da con un hilo que se ejecuta en paralelo y lo muestra durante 3 segundos en la pestaña listado ya que el foco vovlera a esta
                                Thread h1 = new Thread(new ThreadStart(MensajeInformacionModCorrec));
                                h1.Start();
                                Listado = Conversion(_dao.selectServicio());
                                EsCorrectoMod = -1;
                                //Limpieza por si vuelve clicar en el mismo registro para modificar justo despues de haberlo hecho antes:
                                DescripcionMod = string.Empty;
                                PrecioMod = 0;
                            }
                            break;

                        case "factura":
                            if (listLineasFacturaSutituta.Count != 0)
                            {
                                DialogResult respuesta = MessageBox.Show("Esta seguro de no quere añadir mas lineas a esta nueva factura con el numero: " + NumeroFacturaSustituta + " que sustituira a la factura numero: " + NumeroFactura, "¡¡Atención Ò.Ó!!", MessageBoxButtons.YesNo);
                                if (respuesta == DialogResult.Yes)
                                {
                                    //AQUI entonces realizaremos la insercion del listado de lineas de la factura que habremos preparado y almacenado antes en listLineasFacturaAanular
                                    for (int i = 0; i < listLineasFacturaSutituta.Count; i++)
                                    {
                                        Factura f = new Factura();
                                        f = listLineasFacturaSutituta[i];
                                        /*En este paso comprobamos que este en la tabla reparacion los datos de la linea de factura y de actualizar la factura anulada
                                        a estado ANULADO y que la nueva factura contenta en numeroFacturaAnulada el numero de la factura que se anula*/
                                        _dao.InsertarFacturaSustitutaPorAnulada(f.NumeroFactura, f.Linea, f.IdCliente, f.Matricula, f.CodServicio, f.Fecha, f.NumeroFacturaAnulada);
                                    }

                                    //Volvemos ha activar los combox y datapicker(Solo se bloquearon para asegurarnos que esta factura siempre insertamos el mismo cliente,con el mismo coche de el mismo dia ,con distintos servicios)
                                    BloquearCbxIdClienteFactura = true;
                                    BloquearCbxMatriculaFactura = true;
                                    BloquearDataPickerFechaFactura = true;

                                    EsCorrectoMod = 0;//es correcta la modificacion Para cambiar el foco a tblistado en lugar de estar en tbAñadir

                                    //Vaciamos la lista, por si se vuelve a modificar otra, que no se mezclen los registros de facturas que se acumularian en la lista listLineasFacturaSutituta
                                    listLineasFacturaSutituta.Clear();
                                    Listado = Conversion(_dao.selectFacturas());//Actualizamos la lista
                                    EsCorrectoMod = -1;
                                    Thread h1 = new Thread(new ThreadStart(MensajeInAnulacionFacturaCorrecta));
                                    h1.Start();
                                }

                                //Resumen:
                                /*1º->En primer lugar se guardaran cada  una de las  lineas de facturas nuevas en una lista.
                                  2º->En segundo lugar se comprobara que la linea insertada no existe ya en la tabla reparacion,
                                 * y se insertara las distintas lineas de la factura en la tabla reparacion , sustituiendo el campo linea de la factura por numero de reparacion
                                 *3º-> En tercer lugar se comprobara que la segunda linea de factura si la hay debe coincidir en (fecha,IdCliente y Matricula Coche con la primera), asi como todas las siguiente si las hay
                                 *4º-> Cada vez que se desee añadir una nueva linea de factura se debera dar aun boton de siguiente linea, el cual vaciara los campos excepto Numero de factura a anular y numero de factura sustituta.
                                 * 5º->cuando se finalice de introducir todas sus lineas se hara click en modificar y se creara el pdf con la factura y delvoremos al usuario al listado de facturas(antes de esto se preguntara al usuario si esta seguro de esto)
                                 * 6º En caso de dar a volver  en lugar de modificar, se recorrera la listat de facturas insertadas durante este proceso y se eliminaran, lo mismo con las inserciones realizadas en reparaciones
                                */
                                //List<Factura> listLineasFacturaAanular = new List<Factura>();//Listado de datos de la factura que vamos modificando para anular una factura y sustituirla por otra en ModificacionRegistro VolverAtrasMod
                                break;
                            }
                            else {
                                System.Windows.MessageBox.Show("No has indicado ninguna linea para la nueva factura sustituta.", "(◑ω◐)¡Ops!.");
                                break;
                            }
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
        //Guardamos lineas de la factura sustituta que va anular a otra en modificaciones
        private void AnadirLineaFacturaSustituta()
        {
            //Añadimos la linea de factura de la pestaña modificaciones al listado de listLineasFacturaAanular
            try
            {
                if (IdClienteFactura == -1 || MatriculaFactura is null || ServicioFactura is null)
                    System.Windows.MessageBox.Show("Debes selecionar siempre un cliente,una matricula,una fecha y un servicio.", "(◑ω◐)¡Ops!.");
                else
                {
                    Factura f = new Factura();
                    f.NumeroFactura = NumeroFacturaSustituta;//Numero de nueva factura que sustitutye a la anulada
                                                             //f.Linea = LineaFactura;
                    f.IdCliente = IdClienteFactura;
                    f.Matricula = MatriculaFactura;
                    f.NombreServicio = ServicioFactura;
                    f.CodServicio = _dao.selectServicioCodigo(ServicioFactura);
                    f.NombreCliente = _dao.selectClienteNombre(IdClienteFactura);
                    f.Fecha = FechaFactura.ToString("yyyy-MM-dd");
                    f.EstadoFactura = "VIGENTE";
                    f.NumeroFacturaAnulada = NumeroFactura.ToString();//Numero de factura que se va anular

                    int cantidad = listLineasFacturaSutituta.Count;//El numero de la cantidad de factura que vamos añadiendo se corresponde con el numero de lineas de la factura
                    if (cantidad == 0)
                    {
                        f.Linea = 1;
                        BloquearCbxIdClienteFactura = false;
                        BloquearCbxMatriculaFactura = false;
                        BloquearDataPickerFechaFactura = false;
                    }
                    else if (cantidad >= 1)
                    {
                        f.Linea = cantidad + 1;
                    }

                    //Añadimos la nueva factura a la lista que sera la que se recorrera e insertara cuando se confirme los cambios en ModificacionRegistro en el case "factura"
                    listLineasFacturaSutituta.Add(f);
                    //Una vez añadimos una linea de la factura, la siguiente linea añadida sera la 2 asi que aumentamos la propiedad linea en 1
                    //LineaFactura = f.Linea++;

                    //Recargamos el valor por defecto de todos los datos para la siguiente linea de factura que se inserte:
                    //IdclientesComboboxFactura = _dao.selectClienteIdCliente();
                    //Renovamos el conbobox de Servicio Realizado para que se marque a 0
                    //ListComboboxServicioFactura = _dao.selectServicioDescripcion();
                    ServicioFactura = null;

                    //Mensaje de linea insertada
                    Thread h1 = new Thread(new ThreadStart(MensajeInformacionAnulaFactura));
                    h1.Start();
                }

            }
            catch (Exception e)
            {
                Mensaje = "ERROR: " + e.Message;
            }

        }

        //PENDIENTE COMPROBAR SI HAY UNA FACTURA CON LOS MISMOS DATOS QUE OTRA PERO CON DISTINTO NUMERO DE FACTURA Y ESTADO VIGENTE
        //Insertamos linea de factura  totalmente nueva sin que tenga anulaciones o haga referencia  otra factura anulada (Se ejecutara cuando creamos una factura en pdf desde la tabla reparacion)
        private int InsertarlineaFacturaVirgen()
        {
            int minumeroFactura = -1;
            int numeroFacturaYaexistente = -1;

            //IF que pregunta si esa factura existe ya con  los mismos datos y otro numero de factura y el estado vigente


            if (TablaAcltualListada == "reparacion")
            {
                //Comprobacion de si ya se creo una factura con estos mismos datos y que este vigente
                for (int i = 0; i < Listado.Count; i++)
                {
                    Reparacion r = new Reparacion();
                    r = (Reparacion)Listado[i];
                    if (_dao.SelectExisteEstaFacturaVigente(r.IdCliente, r.MatriCoche, r.Fecha, r.CodServicio))//Si alguno de los resgitros ya  existe como factura, se le pide que anule la factura 
                    {
                        minumeroFactura = -2;
                        numeroFacturaYaexistente = _dao.SelectNumeroFactura(r.IdCliente, r.MatriCoche, r.Fecha);
                        break;
                    }
                }
                if (minumeroFactura != -2)//si ninguno de los registros con los que se va ha crear la factura, ya existe en la tabla factura
                {
                    Reparacion rep = new Reparacion();
                    rep = (Reparacion)Listado[0];

                    //Inserta una nueva factura limpiamente
                    minumeroFactura = _dao.selectUltimoNumeroFactura();
                    for (int i = 0; i < Listado.Count; i++)
                    {
                        Reparacion r = new Reparacion();
                        r = (Reparacion)Listado[i];
                        //Insertamos una linea de factura
                        _dao.InsertarFacturaLimpia(minumeroFactura, r.NumReparacion, r.IdCliente, r.MatriCoche, r.CodServicio, r.Fecha);
                    }
                }
                else
                {

                    MessageBox.Show("Ops!.Ya existe una factura creada con eso registros con el numero de factura: " + numeroFacturaYaexistente + "\nSi desea crear un nuevo pdf de estos registros,dirijase a esta factura clicke en ella y luego vaya al boton que indica \"recrear esta factura en PDF.\"\nTambien puede anular esta factura si lo desea y crear una nueva con los registros de reparaciones pertinentes", "(◑ω◐)¡Ops!.");
                }


            }
            return minumeroFactura;

        }

        //Creacion de factura en PDF
        private void CreacionDeFactura()
        {
            //Añadimos las lineas de la nueva factura a la BD.
            #region Factura sacada de la tabla reparacion
            if (TablaAcltualListada == "reparacion")
            {
                int numeroFactura = InsertarlineaFacturaVirgen();
                if (numeroFactura != -2)//si es -2 no se crea la factura, puesto que ya hay una con esos mismos datos enla tabla factura
                {
                    string ruta = AbrirDialogo();
                    if (ruta != string.Empty && ruta != null)
                    {
                        try
                        {
                            #region Preparacion del documento
                            //Preparacion de documento
                            iTextSharp.text.Document documento = new iTextSharp.text.Document(PageSize.LETTER);

                            PdfWriter lapiz = PdfWriter.GetInstance(documento, new FileStream(ruta, FileMode.Create));
                            documento.Open();
                            //Preparacion de imagen
                            /*iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(rutaImg);
                            imagen.BorderWidth = 100;
                            imagen.Alignment = Element.ALIGN_CENTER;
                            float porcentaje = 0.0f;
                            porcentaje = 250 / imagen.Width;
                            imagen.ScalePercent(porcentaje * 100);*/

                            #endregion
                            //Donde sacaremos mas abajo los datos basicos de la factura como la fecha, nombre de el cliente y matricula del coche
                            Reparacion repar = (Reparacion)Listado[0];

                            #region Contenido texto de el pdf

                            //Lineas de el documento
                            Paragraph saltoParrafo = new Paragraph(" ");
                            /* Paragraph titulo1 = new Paragraph();
                             titulo1.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 18, 3, BaseColor.BLACK);
                             titulo1.Add("Electromecánica Óscar.");
                             titulo1.Alignment = Element.ALIGN_CENTER;//alineacion de el texto a la derecha
                             documento.Add(titulo1);*/
                            documento.Add(saltoParrafo);
                            //Añadir imagen lista
                            //documento.Add(imagen);
                            #region Datos Empresa

                            //Datos de la empresa

                            Paragraph dato = new Paragraph();
                            dato.Font = FontFactory.GetFont(FontFactory.TIMES_BOLDITALIC, 16, 3, BaseColor.BLACK);
                            dato.Add("Electromecanica Óscar.");
                            dato.Alignment = Element.ALIGN_CENTER;//alineacion de el texto a la derecha                 
                            documento.Add(dato);
                            documento.Add(saltoParrafo);

                            Paragraph dato1 = new Paragraph();
                            dato1.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                            dato1.Add("Óscar Castro Pérez.");
                            dato1.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha                 
                            documento.Add(dato1);

                            //Datos de la empresa
                            Paragraph dato2 = new Paragraph();
                            dato2.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                            dato2.Add("AVDA. EUROPA, N11S");
                            dato2.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                            documento.Add(dato2);

                            //Datos de la empresa
                            Paragraph dato3 = new Paragraph();
                            dato3.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                            dato3.Add("CP: 29004-MÁLAGA");
                            dato3.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                            documento.Add(dato3);

                            //Datos de la empresa
                            Paragraph dato4 = new Paragraph();
                            dato4.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                            dato4.Add("CIF/NIF: 76751966T");
                            dato4.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                            documento.Add(dato4);

                            //Datos de la empresa
                            Paragraph dato5 = new Paragraph();
                            dato5.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                            dato5.Add("Tlf: 952 360 979");
                            dato5.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                            documento.Add(dato5);

                            //Datos de la empresa
                            Paragraph dato6 = new Paragraph();
                            dato6.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                            dato6.Add("Correo: electromecanicaoscar11s@gmail.com");
                            dato6.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                            documento.Add(dato6);
                            #endregion


                            documento.Add(saltoParrafo);


                            //Numero de Factura
                            Paragraph tituloNFactura = new Paragraph();
                            tituloNFactura.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            tituloNFactura.Add("Nº Factura" + "\n\n" + numeroFactura.ToString());
                            tituloNFactura.Alignment = Element.ALIGN_CENTER;
                            Paragraph titulofechaFactura = new Paragraph();
                            titulofechaFactura.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            titulofechaFactura.Add("FECHA" + "\n\n" + repar.Fecha);
                            titulofechaFactura.Alignment = Element.ALIGN_CENTER;
                            Paragraph tituloIdCliente = new Paragraph();
                            tituloIdCliente.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            tituloIdCliente.Add("ID CLIENTE" + "\n\n" + repar.IdCliente.ToString());
                            tituloIdCliente.Alignment = Element.ALIGN_CENTER;


                            PdfPTable tabla1 = new PdfPTable(3);
                            tabla1.WidthPercentage = 50;
                            tabla1.AddCell(tituloNFactura);
                            tabla1.AddCell(titulofechaFactura);
                            tabla1.AddCell(tituloIdCliente);
                            /*tabla1.AddCell(numeroFactura.ToString());
                            tabla1.AddCell(repar.Fecha);
                            tabla1.AddCell(repar.IdCliente.ToString());*/
                            //tabla1.HorizontalAlignment = 10;
                            tabla1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            documento.Add(tabla1);
                            //documento.Add(titulo2);

                            //Datos Cliente
                            Paragraph lineaCabecera = new Paragraph();
                            lineaCabecera.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);

                            string apellidos = string.Empty;
                            apellidos = _dao.selectClienteApellidos(repar.IdCliente);
                            lineaCabecera.Add("\nNombre: " + repar.NombreCliRepa + "\nApellidos: " + apellidos + "\nMatricula: " + repar.MatriCoche);//+ "\nFecha:" + repar.Fecha
                            documento.Add(lineaCabecera);

                            //Salto de parrafo entre datos y tabla                   
                            documento.Add(saltoParrafo);

                            //Creamos la tabla
                            PdfPTable tabla = new PdfPTable(2);
                            tabla.WidthPercentage = 100;

                            Paragraph cabecera1 = new Paragraph();
                            cabecera1.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15, BaseColor.BLACK);
                            cabecera1.Add("Servicio");
                            Paragraph cabecera2 = new Paragraph();
                            cabecera2.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15, BaseColor.BLACK);
                            cabecera2.Add("Precio");
                            //Cabeceras de tabla
                            tabla.AddCell(cabecera1);
                            tabla.AddCell(cabecera2);
                            List<double> listPreciosAsumar = new List<double>();//Precio de todos los servicios,para posteriormente ser sumaros y añadir el total de el coste de todos ellos a la tabla

                            //Datos para la tabla (servicio realizado) y (precio de este)
                            foreach (object item in Listado)
                            {
                                Paragraph celdColum1 = new Paragraph();
                                celdColum1.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                                double precio = 0;
                                Reparacion r = (Reparacion)item;
                                celdColum1.Add(r.NombreServicio);
                                tabla.AddCell(celdColum1);
                                precio = _dao.selectServicioPrecio(r.NombreServicio);
                                Paragraph celdColum2 = new Paragraph();
                                celdColum2.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                                celdColum2.Add(precio.ToString() + "€");
                                tabla.AddCell(celdColum2);
                                listPreciosAsumar.Add(precio);
                            }
                            documento.Add(tabla);

                            //Sumatorio de precios
                            double precioTotal = 0;
                            foreach (var valor in listPreciosAsumar)
                            {
                                precioTotal += valor;
                            }

                            string IVA = System.Configuration.ConfigurationManager.AppSettings.Get("IVA");//Optenemos el  dato de configuracion con la clabe IVA de el fichero app.config

                            Paragraph tituloImporteBruto = new Paragraph();
                            tituloImporteBruto.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            tituloImporteBruto.Add("IMPORTE BRUTO");

                            Paragraph tituloBaseImponible = new Paragraph();
                            tituloBaseImponible.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            tituloBaseImponible.Add("BASE IMPONIBLE");

                            Paragraph tituloIVA = new Paragraph();
                            tituloIVA.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            tituloIVA.Add(IVA + "%" + "I.V.A.");

                            Paragraph tituloTotal = new Paragraph();
                            tituloTotal.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            tituloTotal.Add("TOTAL");

                            //Añadimos linea de precio total con un estilo concreto.
                            Paragraph lineaPrecioTotal = new Paragraph();
                            lineaPrecioTotal.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            lineaPrecioTotal.Add(precioTotal.ToString() + "€");

                            double precioBrutoYBImponible = 0;
                            double ivaAplicado = 0;
                            ivaAplicado = (precioTotal * double.Parse(IVA)) / 100;//Sacamos cuanto es 21% de el iva del precio final
                            precioBrutoYBImponible = precioTotal - ivaAplicado;//Le quitamos el 21% del iva el precio total para obtener el precio bruto y la Base imponible por que los descuentos lo hacemos directamente sobre  total de la factura, si no iria a parte

                            //Añadimos linea de precio total con un estilo concreto.
                            Paragraph lineaBrutoYBImponible = new Paragraph();
                            lineaBrutoYBImponible.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                            lineaBrutoYBImponible.Add(precioBrutoYBImponible.ToString());



                            PdfPTable tabla3 = new PdfPTable(3);
                            tabla3.WidthPercentage = 80;

                            tabla3.AddCell(tituloBaseImponible);
                            tabla3.AddCell(tituloIVA);
                            tabla3.AddCell(tituloTotal);

                            tabla3.AddCell(precioBrutoYBImponible.ToString());
                            tabla3.AddCell(ivaAplicado.ToString());
                            tabla3.AddCell(lineaPrecioTotal);

                            tabla3.HorizontalAlignment = Element.ALIGN_LEFT;

                            documento.Add(saltoParrafo);
                            documento.Add(saltoParrafo);
                            documento.Add(tabla3);
                            documento.Close();

                            #endregion

                            System.Windows.MessageBox.Show("Factura del :" + repar.Fecha + "\nNombre: " + repar.NombreCliRepa + " " + apellidos + "\nGuardada en la ruta: \"" + ruta + "\"", "Éxito◑‿◐");

                        }

                        catch
                        {
                            System.Windows.MessageBox.Show("Ops!.Ocurrio un erro al crear la factura en formato PDF.\nIntentelo de nuevo más tarde, o pongase en contacto con el adminsitrador.", "(◑ω◐)¡Ops!.");
                        }
                    }
                }
            }
            #endregion

            #region Factura sacada de la tabla Factura
            //Apartado recrear factura en PDF desde la tabla facturas(Necesitamos la propiedad selecteItem de el datagrid
            if (TablaAcltualListada == "factura")
            {

                Factura fac = new Factura();

                if (SelecionRegistroAModificar != null)
                {
                    fac = (Factura)SelecionRegistroAModificar;



                    if (fac.EstadoFactura == "VIGENTE")
                    {
                        string ruta = AbrirDialogo();
                        if (ruta != string.Empty && ruta != null)
                        {
                            try
                            {
                                #region Preparacion del documento
                                //Preparacion de documento
                                iTextSharp.text.Document documento = new iTextSharp.text.Document(PageSize.LETTER);

                                PdfWriter lapiz = PdfWriter.GetInstance(documento, new FileStream(ruta, FileMode.Create));
                                documento.Open();
                                //Preparacion de imagen
                               /* iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(rutaImg);
                                imagen.BorderWidth = 100;
                                imagen.Alignment = Element.ALIGN_CENTER;
                                float porcentaje = 0.0f;
                                porcentaje = 250 / imagen.Width;
                                imagen.ScalePercent(porcentaje * 100);*/

                                #endregion
                                //Donde sacaremos mas abajo los datos basicos de la factura como la fecha, nombre de el cliente y matricula del coche
                                Factura miFactura = (Factura)SelecionRegistroAModificar;

                                #region Contenido texto de el pdf

                                //Lineas de el documento
                                Paragraph saltoParrafo = new Paragraph(" ");
                                /* Paragraph titulo1 = new Paragraph();
                                 titulo1.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 18, 3, BaseColor.BLACK);
                                 titulo1.Add("Electromecánica Óscar.");
                                 titulo1.Alignment = Element.ALIGN_CENTER;//alineacion de el texto a la derecha
                                 documento.Add(titulo1);*/
                                documento.Add(saltoParrafo);
                                //Añadir imagen lista
                                //documento.Add(imagen);

                                #region Datos Empresa

                                //Datos de la empresa
                                Paragraph dato = new Paragraph();
                                dato.Font = FontFactory.GetFont(FontFactory.TIMES_BOLDITALIC, 16, 3, BaseColor.BLACK);
                                dato.Add("Electromecanica Óscar.");
                                dato.Alignment = Element.ALIGN_CENTER;//alineacion de el texto a la derecha                 
                                documento.Add(dato);
                                documento.Add(saltoParrafo);

                                Paragraph dato1 = new Paragraph();
                                dato1.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                                dato1.Add("Óscar Castro Pérez.");
                                dato1.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha                 
                                documento.Add(dato1);

                                //Datos de la empresa
                                Paragraph dato2 = new Paragraph();
                                dato2.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                                dato2.Add("AVDA. EUROPA, N11S");
                                dato2.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                                documento.Add(dato2);

                                //Datos de la empresa
                                Paragraph dato3 = new Paragraph();
                                dato3.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                                dato3.Add("CP: 29004-MÁLAGA");
                                dato3.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                                documento.Add(dato3);

                                //Datos de la empresa
                                Paragraph dato4 = new Paragraph();
                                dato4.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                                dato4.Add("CIF/NIF: 76751966T");
                                dato4.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                                documento.Add(dato4);

                                //Datos de la empresa
                                Paragraph dato5 = new Paragraph();
                                dato5.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                                dato5.Add("Tlf: 952 360 979");
                                dato5.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                                documento.Add(dato5);

                                //Datos de la empresa
                                Paragraph dato6 = new Paragraph();
                                dato6.Font = FontFactory.GetFont(FontFactory.TIMES, 13, 3, BaseColor.BLACK);
                                dato6.Add("Correo: electromecanicaoscar11s@gmail.com");
                                dato6.Alignment = Element.ALIGN_JUSTIFIED;//alineacion de el texto a la derecha
                                documento.Add(dato6);
                                #endregion

                                documento.Add(saltoParrafo);


                                //Numero de Factura
                                Paragraph tituloNFactura = new Paragraph();
                                tituloNFactura.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                tituloNFactura.Add("Nº Factura" + "\n\n" + miFactura.NumeroFactura.ToString());
                                tituloNFactura.Alignment = Element.ALIGN_CENTER;
                                Paragraph titulofechaFactura = new Paragraph();
                                titulofechaFactura.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                titulofechaFactura.Add("FECHA" + "\n\n" + miFactura.Fecha);
                                titulofechaFactura.Alignment = Element.ALIGN_CENTER;
                                Paragraph tituloIdCliente = new Paragraph();
                                tituloIdCliente.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                tituloIdCliente.Add("ID CLIENTE" + "\n\n" + miFactura.IdCliente.ToString());
                                tituloIdCliente.Alignment = Element.ALIGN_CENTER;


                                PdfPTable tabla1 = new PdfPTable(3);
                                tabla1.WidthPercentage = 50;
                                tabla1.AddCell(tituloNFactura);
                                tabla1.AddCell(titulofechaFactura);
                                tabla1.AddCell(tituloIdCliente);
                                /*tabla1.AddCell(numeroFactura.ToString());
                                tabla1.AddCell(repar.Fecha);
                                tabla1.AddCell(repar.IdCliente.ToString());*/
                                //tabla1.HorizontalAlignment = 10;
                                tabla1.HorizontalAlignment = Element.ALIGN_RIGHT;
                                documento.Add(tabla1);
                                //documento.Add(titulo2);

                                //Datos Cliente
                                Paragraph lineaCabecera = new Paragraph();
                                lineaCabecera.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);

                                //string apellidos = string.Empty;
                                //apellidos = _dao.selectClienteApellidos(miFactura.IdCliente);
                                lineaCabecera.Add("\nNombre: " + miFactura.NombreCliente + "\nApellidos: " + miFactura.ApellidosCliente + "\nMatricula: " + miFactura.Matricula);//+ "\nFecha:" + repar.Fecha
                                documento.Add(lineaCabecera);

                                //Salto de parrafo entre datos y tabla                   
                                documento.Add(saltoParrafo);

                                //Recogemos todas las lineas de esa determinada factura a traves de su numero
                                List<Factura> listLineasFactura = new List<Factura>();
                                listLineasFactura = _dao.selectFacturas(miFactura.NumeroFactura);

                                //Creamos la tabla
                                PdfPTable tabla = new PdfPTable(2);
                                tabla.WidthPercentage = 100;

                                Paragraph cabecera1 = new Paragraph();
                                cabecera1.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15, BaseColor.BLACK);
                                cabecera1.Add("Servicio");
                                Paragraph cabecera2 = new Paragraph();
                                cabecera2.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15, BaseColor.BLACK);
                                cabecera2.Add("Precio");
                                //Cabeceras de tabla
                                tabla.AddCell(cabecera1);
                                tabla.AddCell(cabecera2);
                                List<double> listPreciosAsumar = new List<double>();//Precio de todos los servicios,para posteriormente ser sumaros y añadir el total de el coste de todos ellos a la tabla

                                //Datos para la tabla (servicio realizado) y (precio de este)
                                foreach (Factura item in listLineasFactura)
                                {
                                    Paragraph celdColum1 = new Paragraph();
                                    celdColum1.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                                    double precio = 0;
                                    Factura f = item;
                                    celdColum1.Add(f.NombreServicio);
                                    tabla.AddCell(celdColum1);
                                    precio = _dao.selectServicioPrecio(f.NombreServicio);
                                    Paragraph celdColum2 = new Paragraph();
                                    celdColum2.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                                    celdColum2.Add(precio.ToString() + "€");
                                    tabla.AddCell(celdColum2);
                                    listPreciosAsumar.Add(precio);
                                }
                                documento.Add(tabla);

                                //Sumatorio de precios
                                double precioTotal = 0;
                                foreach (var valor in listPreciosAsumar)
                                {
                                    precioTotal += valor;
                                }

                                string IVA = System.Configuration.ConfigurationManager.AppSettings.Get("IVA");//Optenemos el  dato de configuracion con la clabe IVA de el fichero app.config

                                Paragraph tituloImporteBruto = new Paragraph();
                                tituloImporteBruto.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                tituloImporteBruto.Add("IMPORTE BRUTO");

                                Paragraph tituloBaseImponible = new Paragraph();
                                tituloBaseImponible.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                tituloBaseImponible.Add("BASE IMPONIBLE");

                                Paragraph tituloIVA = new Paragraph();
                                tituloIVA.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                tituloIVA.Add(IVA + "%" + "I.V.A.");

                                Paragraph tituloTotal = new Paragraph();
                                tituloTotal.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                tituloTotal.Add("TOTAL");

                                //Añadimos linea de precio total con un estilo concreto.
                                Paragraph lineaPrecioTotal = new Paragraph();
                                lineaPrecioTotal.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                lineaPrecioTotal.Add(precioTotal.ToString() + "€");

                                double precioBrutoYBImponible = 0;
                                double ivaAplicado = 0;
                                ivaAplicado = (precioTotal * double.Parse(IVA)) / 100;//Sacamos cuanto es 21% de el iva del precio final
                                precioBrutoYBImponible = precioTotal - ivaAplicado;//Le quitamos el 21% del iva el precio total para obtener el precio bruto y la Base imponible por que los descuentos lo hacemos directamente sobre  total de la factura, si no iria a parte

                                //Añadimos linea de precio total con un estilo concreto.
                                Paragraph lineaBrutoYBImponible = new Paragraph();
                                lineaBrutoYBImponible.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                                lineaBrutoYBImponible.Add(precioBrutoYBImponible.ToString());



                                PdfPTable tabla3 = new PdfPTable(3);
                                tabla3.WidthPercentage = 80;

                                tabla3.AddCell(tituloBaseImponible);
                                tabla3.AddCell(tituloIVA);
                                tabla3.AddCell(tituloTotal);

                                tabla3.AddCell(precioBrutoYBImponible.ToString());
                                tabla3.AddCell(ivaAplicado.ToString());
                                tabla3.AddCell(lineaPrecioTotal);

                                tabla3.HorizontalAlignment = Element.ALIGN_LEFT;

                                documento.Add(saltoParrafo);
                                documento.Add(saltoParrafo);
                                documento.Add(tabla3);
                                documento.Close();

                                #endregion

                                System.Windows.MessageBox.Show("Factura del :" + miFactura.Fecha + "\nNombre: " + miFactura.NombreCliente + " " + miFactura.ApellidosCliente + "\nGuardada en la ruta: \"" + ruta + "\"", "Éxito◑‿◐");

                            }

                            catch
                            {
                                System.Windows.MessageBox.Show("Ops!.Ocurrio un erro al crear la factura en formato PDF.\nIntentelo de nuevo más tarde, o pongase en contacto con el adminsitrador.", "(◑ω◐)¡Ops!.");
                            }
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Ops!.Lo sentimos,pero no podemos crear el PDF de una factura anulada!.", "(◑ω◐)¡Ops!.");
                    }

                }
                else
                    System.Windows.MessageBox.Show("Ops!.Antes de recrear el pdf de una factura debes selecionar un registro de alguna de ellas.", "(◑ω◐)¡Ops!.");

            }
            #endregion


        }
        //ventana de dialogo donde se escogera la ruta
        public string AbrirDialogo()
        {
            string rutaProvisional = string.Empty;
            string nombreFactura = string.Empty;
            string ruta = string.Empty;
            FolderBrowserDialog dialogoDirectorio = new FolderBrowserDialog();
            DialogResult resultado;

            string[] formandoFehchaCorrelativa = null;
            DateTime fec = new DateTime();

            if (TablaAcltualListada == "reparacion")
            {
                Reparacion repar = (Reparacion)Listado[0];
                formandoFehchaCorrelativa = repar.Fecha.Split('/');
                nombreFactura = @"\FacturaDe" + repar.IdCliente + "_";
                fec = DateTime.Parse(repar.Fecha);
            }
            if (TablaAcltualListada == "factura")
            {
                Factura miFactura = (Factura)SelecionRegistroAModificar;
                formandoFehchaCorrelativa = miFactura.Fecha.Split('/');
                nombreFactura = @"\FacturaDe" + miFactura.IdCliente + "_";
                fec = DateTime.Parse(miFactura.Fecha);
            }

            #region Formacion de ruta y nombre de fichero
            try
            {
                dialogoDirectorio.ShowNewFolderButton = true;
                resultado = dialogoDirectorio.ShowDialog();
                rutaProvisional = dialogoDirectorio.SelectedPath;
                // path = dialogoDirectorio.SelectedPath;
                // rutaParaSubdirectorio = path;
                dialogoDirectorio.Dispose();

                foreach (string item in formandoFehchaCorrelativa)
                {
                    nombreFactura += item;
                }
                // ruta = @rutaProvisional + nombreFactura;

                //Preparamos el nombre para el directorio el cual constara de numero de mes y  de año correlativos

                string fecha = fec.Month.ToString();
                fecha += "_" + fec.Year.ToString();
                //Crear directorio con numero de mes y año para la factura   
                if (!Directory.Exists(String.Concat(@rutaProvisional + @"\" + fecha)))//Si no existe el directorio lo crea
                {
                    Directory.CreateDirectory(String.Concat(@rutaProvisional + @"\" + fecha));
                }
                //Guardamos la ruta hasta el directorio creado
                ruta = String.Concat(@rutaProvisional + @"\" + fecha + nombreFactura);
                //------//

                if (File.Exists(string.Concat(ruta + ".pdf")))
                {
                    string rutaProvisional2 = ruta;
                    int contador = 0;
                    while (File.Exists(string.Concat(rutaProvisional2 + ".pdf")))
                    {
                        rutaProvisional2 = ruta;//Para que siempre parte de laruta orginal y si cambia el nombre del fichero sea solo añadiendo(1) o (2) etc.. pero no que no sea nombredelfichero.pfd(1)(2)(3) solo nombredelfichero.pfd(1) o nombredelfichero.pfd(2)etcc..
                        contador++;
                        rutaProvisional2 += "(" + contador + ")";

                    };
                    ruta = rutaProvisional2 + ".pdf";
                }
                else
                    ruta = ruta + ".pdf";

            }
            catch (Exception)
            {

                System.Windows.MessageBox.Show("Ocurrio un erro al crear la factura en formato PDF.\nIntentelo de nuevo más tarde, o pongase en contacto con el adminsitrador.", "(◑ω◐)¡Ops!.");
            }

            #endregion


            // return path;
            return ruta;
        }// Refactorizado Listo

   
        public string AbrirDialogParaCSV(string fechaSelecionada)
        {
            string rutaProvisional = string.Empty;
            string ruta = string.Empty;
            FolderBrowserDialog dialogoDirectorio = new FolderBrowserDialog();
            DialogResult resultado;
            dialogoDirectorio.ShowNewFolderButton = true;
            resultado = dialogoDirectorio.ShowDialog();
            rutaProvisional = dialogoDirectorio.SelectedPath;
            dialogoDirectorio.Dispose();

            string nombreFichersoCSV = "FacturasDel";

            if (!File.Exists(rutaProvisional))
            {

                if (fechaSelecionada != null)
                {
                    List<Factura> listFacturasMes = _dao.selectFacturaFiltroFechaMes(fechaSelecionada);
                    Factura miFactura = listFacturasMes[0];
                    string[] formandoFehchaCorrelativa = null;
                    formandoFehchaCorrelativa = miFactura.Fecha.Split('/');

                    foreach (string item in formandoFehchaCorrelativa)
                    {
                        nombreFichersoCSV += "_" + item;
                    }
                    // ruta = @rutaProvisional + nombreFactura;
                }
                else
                    nombreFichersoCSV = "TodasLasFacturas";

                //Guardamos la ruta hasta el directorio creado
                ruta = String.Concat(@rutaProvisional + @"\" + nombreFichersoCSV);
                //------//
                string rutaComprobadora = string.Concat(ruta + ".csv");
                if (File.Exists(rutaComprobadora))
                {
                    string rutaProvisional2 = ruta;
                    int contador = 0;
                    while (File.Exists(string.Concat(rutaProvisional2 + ".csv")))
                    {
                        rutaProvisional2 = ruta;//Para que siempre parte de laruta orginal y si cambia el nombre del fichero sea solo añadiendo(1) o (2) etc.. pero no que no sea nombredelfichero.pfd(1)(2)(3) solo nombredelfichero.pfd(1) o nombredelfichero.pfd(2)etcc..
                        contador++;
                        rutaProvisional2 += "(" + contador + ")";

                    };
                    ruta = rutaProvisional2 + ".csv";
                }
                else
                    ruta = ruta + ".csv";
            }

            return ruta;
        }

       
        public void ExtraerFacturasACsv()
        {
            bool siHayFacturas = false;
            List<Factura> listFacturas = new List<Factura>();
            string ruta = string.Empty;
           _dao.Conectar();//Debemos abrir una nueva conexion ya que estamos  abriendo otra nueva ventana

          
                // Si esta marcado el radiobutton de todas las facturas
                if (CbxCSVTodasLasFacturas)
                {

                    //comprobamos que hay facturas para ese mes de la fecha escogida
                    siHayFacturas = _dao.SelectExistenFacturas();

                    //Si hay factura abre el dialog,si no,no
                    if (siHayFacturas == true)
                    {
                        //Abrir Dialog
                        ruta = AbrirDialogParaCSV(null);
                        //Recogemos todas las facturas 
                        listFacturas = _dao.selectFacturas();
                    }

                }

                //Si el radiobutton marcado es la de facturas de un mes
                if (CbxCSVfacturaExtracfiltrarPorUnMes)
                {
                    //comprobamos que hay facturas para ese mes de la fecha escogida 
                    siHayFacturas = _dao.SelectExisteFacturasParaEseMes(FechaMesExtraerFacturasCsv.ToString("yyyy-MM-dd"));

                    //Si hay facturas, si no,no
                    if (siHayFacturas == true)
                    {
                        //Abrir dialog
                        ruta = AbrirDialogParaCSV(FechaMesExtraerFacturasCsv.ToString("yyyy-MM-dd"));
                        //Recogemos todas las facturas de un mes     
                        listFacturas = _dao.selectFacturaFiltroFechaMes(FechaMesExtraerFacturasCsv.ToString("yyyy-MM-dd"));
                    }
                }
                if (siHayFacturas == false)
                    System.Windows.MessageBox.Show("Ops!.¡Lo sentimos, pero no hay facturas registradas para ese mes!.", "(◑ω◐)¡Ops!.");


            if (siHayFacturas == true)
            {
                #region Escribimos fichero CSV
                List<string> encabezadoCSV = new List<string>();
                #region prepara encabezado del fichero CSV
                encabezadoCSV.Add("Numero de Factura");
                encabezadoCSV.Add("linea");
                encabezadoCSV.Add("Estado de la factura");
                encabezadoCSV.Add("Numero de factura anulada");
                encabezadoCSV.Add("Id Cliente");
                encabezadoCSV.Add("Nombre Cliente");
                encabezadoCSV.Add("Apellidos Cliente");
                encabezadoCSV.Add("Matricula del coche");
                encabezadoCSV.Add("Codigo de Servicio");
                encabezadoCSV.Add("Nombre de Servicio");
                encabezadoCSV.Add("Fecha");
                #endregion


                string cadenaEncabezadounida = string.Join(";", encabezadoCSV);

                //Empezamos ha escribir en el fichero
                try
                {
                    FileStream fichero = new FileStream(ruta, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter escritor = new StreamWriter(fichero);

                    using (escritor)
                    {
                        //Escribimos encabezados
                        escritor.WriteLine(cadenaEncabezadounida);

                        List<string> lineaDeCSV = new List<string>();
                        //Recorremo la lista de facturas y vamos escribiendo
                        foreach (Factura item in listFacturas)
                        {
                            lineaDeCSV.Add(item.NumeroFactura.ToString());
                            lineaDeCSV.Add(item.Linea.ToString());
                            lineaDeCSV.Add(item.EstadoFactura.ToString());
                            lineaDeCSV.Add(item.NumeroFacturaAnulada.ToString());
                            lineaDeCSV.Add(item.IdCliente.ToString());
                            lineaDeCSV.Add(item.NombreCliente.ToString());
                            lineaDeCSV.Add(item.ApellidosCliente.ToString());
                            lineaDeCSV.Add(item.Matricula.ToString());
                            lineaDeCSV.Add(item.CodServicio.ToString());
                            lineaDeCSV.Add(item.NombreServicio.ToString());
                            lineaDeCSV.Add(item.Fecha.ToString());
                            //Unimos las cadenas en una sola con un separador ";"
                            string cadenaLineaFacturaUnida = string.Join(";", lineaDeCSV);
                            escritor.WriteLine(cadenaLineaFacturaUnida);//Escribimos en el fichero la linea separadas por ;
                            lineaDeCSV.Clear();//Limpiamos para que no escriba en la siguiente pasada la linea anterior mas la nueva
                        }
                    }
                    System.Windows.MessageBox.Show("La extracción a fichero CSV fue un éxito.\nEstá alojado en la ruta:\n" + ruta, "Éxito◑‿◐");
                }
                catch (Exception e)
                {
                    _dao.Desconectar();
                    System.Windows.MessageBox.Show("Ops,Lo sentimos pero ocurrio un error inesperado.\nPongase en contacto con el administrador.\nERROR:"+e.Message, "(◑ω◐)¡Ops!.");
                }
                #endregion
            }

                _dao.Desconectar();
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
                            Listado = Conversion(_dao.selectCliente());
                            break;
                        case "servicio":

                            Listado = Conversion(_dao.selectServicio());
                            break;
                        case "reparacion":
                            Listado = Conversion(_dao.selectReparacion());
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
            IdCliInsert = 0;
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
            IdClirepaInsert = 0;
            MatriculaRepaInsert = string.Empty;
            ServicioRepa = string.Empty;//Usado para identificar el servicio en la tabla reparacion y en la tabla factura
            FechaRepaInser = DateTime.Now;
            CodServicioRepa = 0;
            NumRepaInsert = 1;
            EsCorrectoInsert = -1;
            listReparacionesPorAnadir.Clear();

            //Otros
            Mensaje = string.Empty;
            #endregion

        }

        private void VolverAtrasMod()
        {
            #region Limpieza de propiedades
            //Cliente
            MensajeActualizacion = string.Empty;
            IdCliMod = 0;
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

            //Facturas
            BloquearCbxIdClienteFactura = true;
            BloquearCbxMatriculaFactura = true;
            BloquearDataPickerFechaFactura = true;
            listLineasFacturaSutituta.Clear();
            //Otros
            Mensaje = string.Empty;
            #endregion
        }

        private void RestablecerFiltros()
        {
            FiltroMatriculaSelecionado = null;
            FiltrarCalculoTotalMes = false;
            FiltrarFechaConcreta = false;
            FiltrarMesFecha = false;
            FiltroFecha = DateTime.Now;
            ResultadoCalculoTotalMes = 0;
            if (TablaAcltualListada is "reparacion")
                Listado = Conversion(_dao.selectReparacion());
            if (TablaAcltualListada is "factura")
                Listado = Conversion(_dao.selectFacturas());
            VisibleBtnExtraerFacturasPdf = "Hidden";
        }

        private void AplicarFiltros()
        {
            switch (TablaAcltualListada)
            {
                #region caso de tabla reparacion
                case "reparacion":
                    if (FiltrarFechaConcreta && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarFechaConcreta'y una matricula y no un idCliente
                        Listado = Conversion(_dao.selectReparacionFiltroFecha(FiltroMatriculaSelecionado, FiltroFecha.ToString("yyyy-MM-dd")));

                    //Si ha selecionado radiobuttom FiltrarFechaConcreta pero si una matricula y un idCliente
                    if (FiltrarFechaConcreta && !FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado != null)
                    {
                        Listado = Conversion(_dao.selectReparacionUnIdCliUnaMatriculaEnFecha(FiltroMatriculaSelecionado, int.Parse(FiltroIDClienteSelecionado), FiltroFecha.ToString("yyyy-MM-dd")));
                        //Activacion de facturas
                        if (Listado.Count != 0 && Listado != null)//Si se encontro algo se activa el boton si no, no
                            VisibleBtnExtraerFacturasPdf = "Visible";
                        else
                            VisibleBtnExtraerFacturasPdf = "Hidden";
                    }

                    if (FiltrarFechaConcreta && FiltroMatriculaSelecionado is null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarFechaConcreta'y no una matricula ni un IdCliente
                        Listado = Conversion(_dao.selectReparacionFiltroFecha(FiltroFecha.ToString("yyyy-MM-dd")));

                    if (FiltrarMesFecha && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarMesFecha'y una matricula pero no un idCliente
                        Listado = Conversion(_dao.selectReparacionFiltroFechaMes(FiltroMatriculaSelecionado, FiltroFecha.ToString("yyyy-MM-dd")));

                    if (FiltrarMesFecha && FiltroMatriculaSelecionado is null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarMesFecha'y no una matricula ni un IdCliente
                        Listado = Conversion(_dao.selectReparacionFiltroFechaMes(FiltroFecha.ToString("yyyy-MM-dd")));

                    //Si ha selecionado radiobuttom FiltrarMesFecha pero si una matricula y un idCliente
                    if (!FiltrarFechaConcreta && FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado != null)
                    {
                        Listado = Conversion(_dao.selectReparacionUnIdCliUnaMatriculaEnMes(FiltroMatriculaSelecionado, int.Parse(FiltroIDClienteSelecionado), FiltroFecha.ToString("yyyy-MM-dd")));

                    }

                    //Si no ha selecionado ninguna radiobuttom pero si una matricula y no un idCliente
                    if (!FiltrarFechaConcreta && !FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado is null)
                        Listado = Conversion(_dao.selectReparacion(FiltroMatriculaSelecionado));

                    //Si no ha selecionado ninguna radiobuttom pero si una matricula y un idCliente
                    if (!FiltrarFechaConcreta && !FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado != null)
                        Listado = Conversion(_dao.selectReparacionUnIdCliUnaMatricula(FiltroMatriculaSelecionado, int.Parse(FiltroIDClienteSelecionado)));
                    //_________________//

                    //Si ha selecionado radiobuttom 'FiltrarCalculoTotalMes'
                    if (FiltrarCalculoTotalMes)
                    {
                        ResultadoCalculoTotalMes = _dao.selectReparacionFiltroCalculoMes(FiltroFecha.ToString("yyyy-MM-dd"));
                        Listado = Conversion(_dao.selectReparacionFiltroFechaMes(FiltroFecha.ToString("yyyy-MM-dd")));
                    }
                    break;
                #endregion

                case "factura":
                    //Filtros para facturas 
                    if (FiltrarFechaConcreta && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarFechaConcreta'y una matricula y no un idCliente
                        Listado = Conversion(_dao.selectFacturaFiltroFecha(FiltroMatriculaSelecionado, FiltroFecha.ToString("yyyy-MM-dd")));

                    //Si ha selecionado radiobuttom FiltrarFechaConcreta pero si una matricula y un idCliente
                    if (FiltrarFechaConcreta && !FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado != null)
                    {
                        Listado = Conversion(_dao.selectFacturaUnIdCliUnaMatriculaEnFecha(FiltroMatriculaSelecionado, int.Parse(FiltroIDClienteSelecionado), FiltroFecha.ToString("yyyy-MM-dd")));
                        //Activacion de facturas
                        if (Listado.Count != 0 && Listado != null)//Si se encontro algo se activa el boton si no, no facturas de esa fecha y de ese cliente
                            VisibleBtnExtraerFacturasPdf = "Visible";
                        else
                            VisibleBtnExtraerFacturasPdf = "Hidden";
                    }

                    if (FiltrarFechaConcreta && FiltroMatriculaSelecionado is null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarFechaConcreta'y no una matricula ni un IdCliente
                        Listado = Conversion(_dao.selectFacturaFiltroFecha(FiltroFecha.ToString("yyyy-MM-dd")));

                    if (FiltrarMesFecha && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarMesFecha'y una matricula pero no un idCliente
                        Listado = Conversion(_dao.selectFacturaFiltroFechaMes(FiltroMatriculaSelecionado, FiltroFecha.ToString("yyyy-MM-dd")));

                    if (FiltrarMesFecha && FiltroMatriculaSelecionado is null && FiltroIDClienteSelecionado is null)//Si ha selecionado radiobuttom 'FiltrarMesFecha'y no una matricula ni un IdCliente
                        Listado = Conversion(_dao.selectFacturaFiltroFechaMes(FiltroFecha.ToString("yyyy-MM-dd")));

                    //Si ha selecionado radiobuttom FiltrarMesFecha pero si una matricula y un idCliente
                    if (!FiltrarFechaConcreta && FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado != null)
                    {
                        Listado = Conversion(_dao.selectFacturaUnIdCliUnaMatriculaEnMes(FiltroMatriculaSelecionado, int.Parse(FiltroIDClienteSelecionado), FiltroFecha.ToString("yyyy-MM-dd")));

                    }

                    //Si no ha selecionado ninguna radiobuttom pero si una matricula y no un idCliente
                    if (!FiltrarFechaConcreta && !FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado is null)
                        Listado = Conversion(_dao.selectFactura(FiltroMatriculaSelecionado));

                    //Si no ha selecionado ninguna radiobuttom pero si una matricula y un idCliente
                    if (!FiltrarFechaConcreta && !FiltrarMesFecha && !FiltrarCalculoTotalMes && FiltroMatriculaSelecionado != null && FiltroIDClienteSelecionado != null)
                        Listado = Conversion(_dao.selectFacturaUnIdCliUnaMatricula(FiltroMatriculaSelecionado, int.Parse(FiltroIDClienteSelecionado)));
                    //_________________//

                    //Si ha selecionado radiobuttom 'FiltrarCalculoTotalMes'
                    if (FiltrarCalculoTotalMes)
                    {
                        ResultadoCalculoTotalMes = _dao.selectFacturaFiltroCalculoMes(FiltroFecha.ToString("yyyy-MM-dd"));
                        Listado = Conversion(_dao.selectFacturaFiltroFechaMesNoANULADAS(FiltroFecha.ToString("yyyy-MM-dd")));
                    }


                    break;
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

        public RelayCommand RegistroFacturas_click
        {
            get { return new RelayCommand(listadoFac => ListadoFacturas(), ListadoFac => true); }
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

        public RelayCommand CreacionDeFactura_click
        {
            get { return new RelayCommand(factura => CreacionDeFactura(), factura => true); }
        }

        public RelayCommand AnadirLineaFacturaSustituta_click
        {
            get { return new RelayCommand(facturasustituta => AnadirLineaFacturaSustituta(), facturasustituta => true); }
        }
        public RelayCommand AnadirReparacionALalistaDePreparacion_click
        {
            get { return new RelayCommand(reparacion => AnadirReparacionALalistaDePreparacion(), reparacion => true); }
        }

        public RelayCommand ExtraerFacturasACsv_click
        {
            get { return new RelayCommand(extraerACSV => ExtraerFacturasACsv(), extraerACSV => true); }
        }




        //----Fin Listado de registros---
        #endregion


        #region Conversion de tipo de listas a lista object
        /*Estos metodos son usados para convertir las listas de objetos concretos que nos devuelve el objeto
         dao tra hacer la conexion y  consulta a la BD
         en lista de object para poder tratarlos todos por la misma propiedad "Listado" a la que el datagrid
         esta bindeada para que muestre todos los listados que sean soliciatdos*/

        public List<object> Conversion(List<Cliente> lcliente)
        {
            List<object> listobjetos = new List<object>();
            foreach (Cliente cli in lcliente)
            {
                listobjetos.Add(cli);
            }
            return listobjetos;
        }

        public List<object> Conversion(List<Servicio> lservicios)
        {
            List<object> listobjetos = new List<object>();
            foreach (Servicio miServicio in lservicios)
            {
                listobjetos.Add(miServicio);
            }
            return listobjetos;
        }

        public List<object> Conversion(List<Reparacion> lreparacio)
        {
            List<object> listobjetos = new List<object>();
            foreach (Reparacion miReparacion in lreparacio)
            {
                listobjetos.Add(miReparacion);
            }
            return listobjetos;
        }

        public List<object> Conversion(List<Factura> lfactura)
        {
            List<object> listobjetos = new List<object>();
            foreach (Factura fac in lfactura)
            {
                listobjetos.Add(fac);
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
        private void MensajeInformacionAnulaFactura()
        {
            MensajeActualizacion = "Linea de factura añadida correctamente a la lista de preparación.";
            Thread.Sleep(3000);
            MensajeActualizacion = string.Empty;

        }

        private void MensjInfoReparacionAnadidaEnListaDeInsercion()
        {
            MensajeInsercion = "Reparación añadida correctamente a la lista de preparación.";
            Thread.Sleep(3000);
            MensajeInsercion = string.Empty;

        }

        private void MensajeInformacionModCorrec()
        {
            Mensaje = "Modificacion realizada Correctamente";
            Thread.Sleep(3000);
            Mensaje = string.Empty;

        }

        private void MensajeInAnulacionFacturaCorrecta()
        {
            Mensaje = "La Factura se anulo correctamente";
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


 //Tareas pendientes:
 ---Actualmente en curso---
1º-Crear una nueva factura al crear un pdf factura filtrando por una fecha determinada un cliente y una matricula en reparacion
2º-Terminar apartado de Facturas en  ModificacionRegistro  para poder anular una factura existente


--En cola---
 -Copia de seguridad BD automatica

//Tareas Finalizadas:
 -Conectar
 -Inserciones
 -Modificaciones.
 -Eliminacion.
 -Ver otros registros
 -Filtros
 -Generar facturas en pdf;


//Extras:
1-Numero de tlf , este campo solo admite numeros de una determinada longitud
2-Campo precio solo admite numeros 



 */
