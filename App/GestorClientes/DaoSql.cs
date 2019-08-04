using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Añadido sql
using System.Data;
using MySql.Data.MySqlClient;
//Para coger datos como la cadena conexion de el archivo de configuracion app.config
using GestorClientes.Properties;


namespace GestorClientes
{
    class DaoSql
    {
        public MySqlConnection conexion;

        public bool Conectar()
        {

            //string cadenaConecxion = string.Format("server=localhost;port=3306;database=mecanica;uid=root;pwd=123;");
            string cadenaConecxion = Settings.Default.cadeanaConexion;//Boton derecho  en GestorClientes de el explorador de soluciones y dentrod e hay en "configuracion" ahi esta asignada la cadena de conexion
            try
            {              
                conexion = new MySqlConnection(cadenaConecxion);
                conexion.Open();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool Desconectar()
        {
            try
            {
                conexion.Close();
                conexion = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EstadoConexion()
        {
            if (conexion != null && conexion.State == ConnectionState.Open)
            {
                return true;
            }
            else
                return false;
        }

        public List<Cliente> selectCliente()
        {
            List<Cliente> lClientes = new List<Cliente>();
            string sql = "select idCliente,nombre,apellidos,tlf,matricula,marca,modelo from cliente;";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente miCliente = new Cliente();
                    miCliente.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miCliente.Nombre = lector["nombre"].ToString();
                    miCliente.Apellidos = lector["apellidos"].ToString();
                    miCliente.Tlf = int.Parse(lector["tlf"].ToString());
                    miCliente.Matricula = lector["matricula"].ToString();
                    miCliente.Marca = lector["marca"].ToString();
                    miCliente.Modelo = lector["modelo"].ToString();
                    lClientes.Add(miCliente);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lClientes;

        }

        public List<string> selectClienteIdCliente()
        {
            List<string> lClientes = new List<string>();
            string sql = "select idCliente from cliente;";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    lClientes.Add(lector["idCliente"].ToString());
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lClientes;

        }

        public List<string> selectClienteMatricula(int idCliente)
        {
            List<string> listMatricula = new List<string>();
            string sql = "select matricula from cliente where idCliente='" + idCliente + "';";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente cli = new Cliente();
                    cli.Matricula = lector["matricula"].ToString();
                    listMatricula.Add(cli.Matricula);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMatricula;

        }

        //Dado un idCliente devuelvo su nombre
        public string selectClienteNombre(int idCliente)
        {
            string nombre = string.Empty;
            string sql = "select nombre from cliente where idCliente='" + idCliente + "';";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    nombre = lector["nombre"].ToString();
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return nombre;

        }

        public List<Reparacion> selectReparacion()
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r order by fecha and order by idCliente;
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r order by fecha,idCliente;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa= lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        public List<Servicio> selectServicio()
        {
            List<Servicio> lServicio = new List<Servicio>();
            string sql = "select * from servicio;";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Servicio miServicio = new Servicio();
                    miServicio.Codigo = int.Parse(lector["codigo"].ToString());
                    miServicio.Descripcion = lector["descripcion"].ToString();
                    miServicio.Precio = double.Parse(lector["precio"].ToString());


                    lServicio.Add(miServicio);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lServicio;

        }

        public List<string> selectServicioDescripcion()
        {
            List<string> lServicio = new List<string>();
            string sql = "select descripcion from servicio;";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    lServicio.Add(lector["descripcion"].ToString());
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lServicio;

        }

        public int selectServicioCodigo(string descripcion)
        {
            Servicio miservicio = new Servicio();
            string sql = "select codigo from servicio where descripcion='" + descripcion + "';";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    miservicio.Codigo = int.Parse(lector["codigo"].ToString());
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return miservicio.Codigo;

        }

        public int selectNumRepara(int idCliRepara, string matriculaRepa, string fecha)
        {
            int nreparaciones = -1;
            //select count(*)as cantidad from reparacion where idCliente=1 and matriCoche='2019OPL' and fecha='2019-07-11';
            string sql = "select count(*)as cantidad from reparacion where idCliente='" + idCliRepara + "'and matriCoche='" + matriculaRepa + "'and fecha='" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "'";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    nreparaciones = int.Parse(lector["cantidad"].ToString());
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return nreparaciones;

        }

        public List<string> VerTablas()
        {
            List<string> lTablas = new List<string>();
            string sql = "show tables";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);
            //SELECT name FROM sqlite_master WHERE type = "table" and name !='sqlite_sequence'; <== Esto equivale desde .NET a ==>  .tables
            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    string m = lector["Tables_in_mecanica"].ToString();

                    lTablas.Add(m);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lTablas;

        }

        public bool InsertCliente(string nombre, string apellidos, int tlf, string matricula, string marca, string modelo)
        {
            try
            {
                if (matricula is null || matricula == " ")
                    throw new Exception("El campo matricula no pueden estar vacios!");
                else
                {
                 
                    //Insercion
                    string sql;
                    sql = "INSERT INTO cliente (nombre,apellidos,tlf,matricula,marca,modelo) values ('" + nombre.ToUpper() + "','" + apellidos.ToUpper() + "'," + tlf + ",'" + matricula.ToUpper() + "','" + marca.ToUpper() + "','" + modelo.ToUpper() + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

        public bool InsertServicio(string descripcion, double precio)
        {
            try
            {
                //Estas lineas estan para modificar la culturalizacion de forma que indicamos el separador de numperos decimales es el . y no la, para que ala hora de la insercion no de fallos al insertar numbreservicio, 20,05 si no que inserte 20.05
                System.Globalization.CultureInfo c = new System.Globalization.CultureInfo("es-ES");
                c.NumberFormat.NumberDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                c.NumberFormat.PercentDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = c;
                //----------//

                if (descripcion is null || descripcion == " ")
                    throw new Exception();
                string sql;
                sql = "INSERT INTO servicio (descripcion,precio) values ('" + descripcion.ToUpper() + "'," + precio + ")";
                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            return true;

        }

        public bool InsertReparacion(int idReparacion, int idCliente, string matriculaCoche, int codServicio, string fecha)
        {
            try
            {
                if (idReparacion <= 0 || idCliente <= 0 || matriculaCoche is null || codServicio < 0 || fecha == null)
                    throw new Exception();
                string sql;
                sql = "INSERT INTO reparacion (numReparacion,idCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + idCliente + "','" + matriculaCoche + "'," + codServicio + ",'" + fecha + "')";

                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

        public bool UpdateCliente(int idCliente, string nombre, string apellidos, int tlf, string matricula, string marca, string modelo)
        {
            try
            {
                if (matricula is null || matricula == " ")
                    throw new Exception("El campo matricula no pueden estar vacios!");
                else
                {
                    //update cliente set nombre='Fran',apellidos='Lanzat' where idCliente=1;
                    //Actualizacion
                    string sql;
                    sql = "update cliente set nombre='" + nombre.ToUpper() + "',apellidos='" + apellidos.ToUpper() + "',tlf=" + tlf + ",matricula='" + matricula.ToUpper() + "',marca='" + marca.ToUpper() + "',modelo='" + modelo.ToUpper() + "' where idCliente=" + idCliente + "";
                    MySqlCommand cmd = new MySqlCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }
        //cambiado
        public bool UpdateServicio(int codiServicio, string descripcion, double precio)
        {
            try
            {
                //Estas lineas estan para modificar la culturalizacion de forma que indicamos el separador de numperos decimales es el . y no la, para que ala hora de la insercion no de fallos al insertar numbreservicio, 20,05 si no que inserte 20.05
                System.Globalization.CultureInfo c = new System.Globalization.CultureInfo("es-ES");
                c.NumberFormat.NumberDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                c.NumberFormat.PercentDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = c;
                //----------//
                if (descripcion == " " || descripcion is null)
                    throw new Exception("El campo descripcion no puede estar vacio!");
                else
                {

                    //Actualizacion
                    string sql;
                    sql = "update servicio set descripcion='" + descripcion.ToUpper() + "',precio=" + precio + " where codigo=" + codiServicio + "";
                    MySqlCommand cmd = new MySqlCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }


        /*No se podra Actualizacion reparacion por consenso ya que es como una factura y necesita de los 4 
         * campos para  modificarla,es mejor crear y eliminarla y crear una nueva reparacion*/

        //Eliminar
        //--------------//
        public bool DeleteCliente(int idCliente, string matricula)
        {
            //delete from cliente where idCliente=1 and matricula='2218CL';
            try
            {
                string sql;
                sql = "delete from cliente where idCliente='" + idCliente + "' and matricula='" + matricula + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch
            {
                throw;
            }
            return true;
        }

        public bool DeleteServicio(int codigo)
        {
            //delete from cliente where idCliente=1 and matricula='2218CL';
            try
            {
                string sql;
                sql = "delete from servicio where codigo=" + codigo + "";
                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        public bool DeleteReparacion(int numReparacion, int idCliente, string matriculaCoche, string fecha)
        {
            //delete from cliente where idCliente=1 and matricula='2218CL';
            //// "INSERT INTO reparacion (numReparacion,idCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + idCliente + "','"+matriculaCoche+ "',"+codServicio+ ",'"+DateTime.Parse(fecha).ToShortDateString()+"')"
            try
            {
                string sql;
                sql = "delete from reparacion where numReparacion=" + numReparacion + " and idCliente='" + idCliente + "'" + " and matriCoche='" + matriculaCoche + "' and fecha='" + Convert.ToDateTime(DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        //Todas las reparaciones de un cliente en una fecha con coche determinado
        public bool DeleteReparacion(int idCliente, string matriculaCoche, string fecha)
        {
            //delete from cliente where idCliente=1 and matricula='2218CL';
            //// "INSERT INTO reparacion (numReparacion,idCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + idCliente + "','"+matriculaCoche+ "',"+codServicio+ ",'"+DateTime.Parse(fecha).ToShortDateString()+"')"
            try
            {
                string sql;
                sql = "delete from reparacion where and idCliente='" + idCliente + "'" + " and matriCoche='" + matriculaCoche + "' and fecha='" + Convert.ToDateTime(DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        //Preparacion Filtros
        //------------------------//

        public List<string> selectMatriculasCocheClientes()
        {
            List<string> listMatricula = new List<string>();
            string sql = "select distinct(matricula) from cliente";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente cli = new Cliente();
                    cli.Matricula = lector["matricula"].ToString();
                    listMatricula.Add(cli.Matricula);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMatricula;

        }

        //Dada una matricula, IdCliente de clientes relacionada con ella
        public List<int> selectIdClienteClientes(string matricula)
        {
            List<int> listIdCliente = new List<int>();
            string sql = "select idCliente from cliente where matricula='" + matricula + "'";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente cli = new Cliente();
                    cli.IdCliente = int.Parse(lector["idCliente"].ToString());
                    listIdCliente.Add(cli.IdCliente);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listIdCliente;

        }

        //----------//

        //Consultas de filtro:
        //------------------------//

        //Con un idCliente selecionado 
        public List<Reparacion> selectReparacionFiltroFecha(string matriculaCoche, string fecha)
        {
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='" + matriculaCoche + "' and fecha='" + fecha + "' order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //Sin idCliente selecionado ni matricula
        public List<Reparacion> selectReparacionFiltroFecha(string fecha)
        {
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where  fecha='" + fecha + "' order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //sin idCliente selecioando pero si con matricula para un mes 
        public List<Reparacion> selectReparacionFiltroFechaMes(string matriculaCoche, string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where idCliente=1 and strftime('%m','2019-07-10')= strftime('%m',fecha);
            //select numReparacion,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='2218CL' and strftime('%m',fecha)=strftime('%m','2019-06-01');
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select numReparacion,idCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='" + matriculaCoche + "' and month(fecha)=month('" + fecha + "')";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='" + matriculaCoche + "' and month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "') order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //Sin idCliente selecionado ni matricula para un mes
        public List<Reparacion> selectReparacionFiltroFechaMes(string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where idCliente=1 and strftime('%m','2019-07-10')= strftime('%m',fecha);
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "') order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //Selecionando solo matricula y sin fecha marcada 
        public List<Reparacion> selectReparacion(string matricula)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='" + matricula + "' order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        public List<Reparacion> selectReparacionUnIdCliUnaMatricula(string matricula, int idCliente)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where idCliente=" + idCliente + " and matriCoche='" + matricula + "' order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        public List<Reparacion> selectReparacionUnIdCliUnaMatriculaEnMes(string matricula, int idCliente, string fecha)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";  strftime('%m',fecha)=strftime('%m','" + fecha + "')"
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where idCliente=" + idCliente + " and matriCoche='" + matricula + "' and  month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "') order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }


        public List<Reparacion> selectReparacionUnIdCliUnaMatriculaEnFecha(string matricula, int idCliente, string fecha)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";  strftime('%m',fecha)=strftime('%m','" + fecha + "')"
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,(select apellidos from cliente where idCliente=r.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where idCliente=" + idCliente + " and matriCoche='" + matricula + "' and fecha='" + fecha + "' order by idCliente,fecha desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.ApellidosCliRepa = lector["apellidos"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        public double selectReparacionFiltroCalculoMes(string fecha)
        {
            double total = 0;
            //select round(sum(precio),2)as total from servicio where codigo in(select codServicio from reparacion where month('2019-07-10')= month(fecha));
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select sum((select precio from servicio where codigo=r.codServicio))as total from reparacion r where month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "')";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    double.TryParse(lector["total"].ToString(), out total);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return total;

        }

        //------------------------//

        //------Tabla Factura-------//

        public List<Factura> selectFacturas()
        {
            List<Factura> lFactura = new List<Factura>();
            
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f order by numeroFactura,Fecha,idCliente desc;";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);
            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente= lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lFactura;

        }

        public List<Factura> selectFacturas(int numeroFactura)
        {
            List<Factura> lFactura = new List<Factura>();

            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where numeroFactura="+numeroFactura+"  order by numeroFactura,Fecha,idCliente desc;";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);
            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }

        public string selectEstadoFacturas(int numeroFactura)
        {
            string estado = string.Empty;

            string sql = "select discinct(estadoFactura) from factura where numeroFactura="+numeroFactura;
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);
            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {                  
                    estado = lector["estadoFactura"].ToString();                               
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return estado;

        }
        #region Filtros Factura
        //Filtros para facturas
        //-------------------------
        public List<Factura> selectFacturaFiltroFecha(string matriculaCoche, string fecha)
        {
            //select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,numeroFacturaAnulada from factura f order by numeroFactura,Fecha,idCliente desc;
            List<Factura> lFactura = new List<Factura>();
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where matriCoche='" + matriculaCoche + "' and fecha='" + fecha + "' order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);
            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }

        //Sin idCliente selecionado ni matricula
        public List<Factura> selectFacturaFiltroFecha(string fecha)
        {
            List<Factura> lFactura = new List<Factura>();
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where  fecha='" + fecha + "' order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }

        //sin idCliente selecioando pero si con matricula para un mes 
        public List<Factura> selectFacturaFiltroFechaMes(string matriculaCoche, string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where idCliente=1 and strftime('%m','2019-07-10')= strftime('%m',fecha);
            //select numReparacion,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='2218CL' and strftime('%m',fecha)=strftime('%m','2019-06-01');
            List<Factura> lFactura = new List<Factura>();
            //string sql = "select numReparacion,idCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='" + matriculaCoche + "' and month(fecha)=month('" + fecha + "')";
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where matriCoche='" + matriculaCoche + "' and month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "') order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }

        //Sin idCliente selecionado ni matricula para un mes
        public List<Factura> selectFacturaFiltroFechaMes(string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where idCliente=1 and strftime('%m','2019-07-10')= strftime('%m',fecha);
            List<Factura> lFactura = new List<Factura>();
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "')  order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }

        //Selecionando solo matricula y sin fecha marcada 
        public List<Factura> selectFactura(string matricula)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Factura> lFactura = new List<Factura>();
            //string sql = "select * from reparacion;";
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where matriCoche='" + matricula + "' order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }

        public List<Factura> selectFacturaUnIdCliUnaMatricula(string matricula, int idCliente)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Factura> lFactura = new List<Factura>();
            //string sql = "select * from reparacion;";
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where idCliente=" + idCliente + " and matriCoche='" + matricula + "' order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }

        public List<Factura> selectFacturaUnIdCliUnaMatriculaEnMes(string matricula, int idCliente, string fecha)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Factura> lFactura = new List<Factura>();
            //string sql = "select * from reparacion;";  strftime('%m',fecha)=strftime('%m','" + fecha + "')"
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where idCliente=" + idCliente + " and matriCoche='" + matricula + "' and  month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "') order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }


        public List<Factura> selectFacturaUnIdCliUnaMatriculaEnFecha(string matricula, int idCliente, string fecha)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Factura> lFactura = new List<Factura>();
            //string sql = "select * from reparacion;";  strftime('%m',fecha)=strftime('%m','" + fecha + "')"
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where idCliente=" + idCliente + " and matriCoche='" + matricula + "' and fecha='" + fecha + "' order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura= lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;
        }
       
        public double selectFacturaFiltroCalculoMes(string fecha)
        {
            double total = 0;
            //select sum((select precio from servicio where codigo=f.codServicio))as total from factura f where month(fecha)=month('2019-06-01') and  year(fecha)=year('2019-06-01') and estadoFactura !='ANULADA';
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select sum((select precio from servicio where codigo=f.codServicio))as total from factura f where month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "') and estadoFactura !='ANULADA'";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    double.TryParse(lector["total"].ToString(), out total);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return total;

        }

        public List<Factura> selectFacturaFiltroFechaMesNoANULADAS(string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where idCliente=1 and strftime('%m','2019-07-10')= strftime('%m',fecha);
            List<Factura> lFactura = new List<Factura>();
            string sql = "select numeroFactura,linea,idCLiente,(select nombre from cliente where idCliente=f.idCliente)as nombre,(select apellidos from cliente where idCliente=f.idCliente)as apellidos,matriCoche,codServicio,(select descripcion from servicio where codigo=f.codServicio)as servicio,fecha,estadoFactura,numeroFacturaAnulada from factura f where month(fecha)=month('" + fecha + "') and  year(fecha)=year('" + fecha + "') and estadoFactura!='ANULADA'  order by numeroFactura,Fecha,idCliente desc";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Factura mifactura = new Factura();
                    mifactura.NumeroFactura = int.Parse(lector["numeroFactura"].ToString());
                    mifactura.Linea = int.Parse(lector["linea"].ToString());
                    mifactura.IdCliente = int.Parse(lector["idCliente"].ToString());
                    mifactura.NombreCliente = lector["nombre"].ToString();
                    mifactura.ApellidosCliente = lector["apellidos"].ToString();
                    mifactura.Matricula = lector["matriCoche"].ToString();
                    mifactura.CodServicio = int.Parse(lector["codServicio"].ToString());
                    mifactura.NombreServicio = lector["servicio"].ToString();
                    mifactura.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
                    mifactura.EstadoFactura = lector["estadoFactura"].ToString();
                    mifactura.NumeroFacturaAnulada = lector["numeroFacturaAnulada"].ToString();

                    lFactura.Add(mifactura);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lFactura;

        }



        //------------------------//
        #endregion

            //Comprobacion de si una factura exists sin saber su numero  de factura
        public bool SelectExisteEstaFactura(int idCliente, string matricula, string fecha)
        {
            int resultado = -1;
            try
            {
                //select exists(select * from factura where fecha='2019-06-01' and matriCoche='2218CL' and estadoFactura='VIGENTE');
                string sql;
                sql = "select exists(select * from factura where fecha='"+ (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "' and matriCoche='"+matricula+"' and estadoFactura='VIGENTE' and idCliente="+idCliente+") as facturaExistente; ";
                MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

                MySqlDataReader lector = null;
               
                try
                {
                    lector = sqlYconec.ExecuteReader();
                    while (lector.Read())
                    {
                        //si devuelve uno 1 es verdad si devuelve 0 es falso
                        resultado = int.Parse(lector["facturaExistente"].ToString());
                    }
                    lector.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
             

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            if (resultado == 1)
                return true;
            else
                return false;

        }

        
        //SI HAY UNA FACTURA CON LOS MISMOS DATOS QUE OTRA PERO CON DISTINTO NUMERO DE FACTURA Y ESTADO VIGENTE
        public bool SelectExisteEstaFacturaVigente(int idCliente, string matricula, string fecha ,int codServicio)
        {
            int resultado = -1;
            try
            {
                //select exists(select * from factura where fecha='2019-06-01' and matriCoche='2218CL' and estadoFactura='VIGENTE');
                string sql;
                sql = "select exists(select * from factura where fecha='" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "' and matriCoche='" + matricula + "' and estadoFactura='VIGENTE' and idCliente=" + idCliente + " and codServicio="+codServicio+") as facturaExistente; ";
                MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

                MySqlDataReader lector = null;

                try
                {
                    lector = sqlYconec.ExecuteReader();
                    while (lector.Read())
                    {
                        //si devuelve uno 1 es verdad si devuelve 0 es falso
                        resultado = int.Parse(lector["facturaExistente"].ToString());
                    }
                    lector.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            if (resultado == 1)
                return true;
            else
                return false;

        }

        public bool SelectExisteEstaLineaDeEstaFactura(int idCliente, string matricula, string fecha,int linea,string codServicio)
        {
            int resultado = -1;
            try
            {
                //select exists(select * from factura where fecha='2019-06-01' and matriCoche='2218CL' and estadoFactura='VIGENTE');
                string sql;
                sql = "select exists(select * from factura where fecha='" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "' and matriCoche='" + matricula + "' and estadoFactura='VIGENTE' and idCliente=" + idCliente + " and linea="+linea+" and codServicio="+codServicio+") as facturaExistente; ";
                MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

                MySqlDataReader lector = null;
             
                try
                {
                    lector = sqlYconec.ExecuteReader();
                    while (lector.Read())
                    {
                        //si devuelve uno 1 es verdad si devuelve 0 es falso
                        resultado = int.Parse(lector["facturaExistente"].ToString());
                    }
                    lector.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
              

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            if (resultado == 1)
                return true;
            else
                return false;
        }
       

        public int SelectNumeroFactura(int idCliente, string matricula, string fecha)
        {
            int resultado = -1;
            try
            {
                //select distinct(numeroFactura) from factura where matriCoche='2218CL' and fecha='2019-06-01'and estadoFactura='VIGENTE' and idCliente=1;
                string sql;
                sql = "select distinct(numeroFactura) from factura where fecha='" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "' and matriCoche='" + matricula + "' and estadoFactura='VIGENTE' and idCliente=" + idCliente;
                MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

                MySqlDataReader lector = null;

                try
                {
                    lector = sqlYconec.ExecuteReader();
                    while (lector.Read())
                    {
                        //si devuelve uno 1 es verdad si devuelve 0 es falso
                        resultado = int.Parse(lector["numeroFactura"].ToString());
                    }
                    lector.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }          
                return resultado;          

        }

        // CORREGIR caso en el que no haya ninguna factura y el max no devuelva nada
        //Devuelve ya el numero de la siguiente factura o sea uno mas de el numero de la ultima factura
        public int selectUltimoNumeroFactura()
        {
            int numeroUltimaFactura = -1;
            //select round(sum(precio),2)as total from servicio where codigo in(select codServicio from reparacion where month('2019-07-10')= month(fecha));
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select max(numeroFactura)as ultimaFactura from factura;";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    int.TryParse(lector["ultimaFactura"].ToString(), out numeroUltimaFactura);
                }
                lector.Close();
                if (numeroUltimaFactura == -1)
                    numeroUltimaFactura = 1;
                else
                    numeroUltimaFactura++;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return numeroUltimaFactura;

        }

        #region Creacion de Factura
        
        public bool InsertarFacturaLimpia(int numeroFactura, int linea, int idCliente, string matricula, int codServicio, string fecha )
        {
            try
            {
                //Insercion
                string sql;
                sql = "INSERT INTO factura (numeroFactura,linea,idCliente,matriCoche,codServicio,fecha,estadoFactura,numeroFacturaAnulada) values ('" + numeroFactura + "','" + linea + "'," + idCliente + ",'" + matricula.ToUpper() + "','" + codServicio + "','" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "','VIGENTE',NULL)";
                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }


        #endregion

        #region Anulacion Factura    

        
        public bool InsertarFacturaSustitutaPorAnulada(int numeroFacturaSustituta,int linea,int idCliente,string matricula,int codServicio,string fecha,string numeroFacturaAnulada)
        {
            try
            {
                #region comprobacion de si existe
                //Primero comprobar si existe un resgitro en la tabla reparacion con estos datos, si no es asi darlo de alta, y despues de esta comprobacion, insertar la nueva factura
                //select exists(select * from reparacion where idCliente=1 and fecha='2019-06-01' and matriCoche='2218CL' and numReparacion=1 and codServicio=4);
                string sql = "select exists(select * from reparacion where idCliente="+idCliente+" and fecha='"+fecha+"' and matriCoche='"+matricula+"' and numReparacion="+linea+" and codServicio="+codServicio+")as existe";
                MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);
                MySqlDataReader lector = null;
                int siExiste = -1;//1 es true, 0 es false
                try
                {
                    lector = sqlYconec.ExecuteReader();
                    while (lector.Read())
                    {
                        int.TryParse(lector["existe"].ToString(), out siExiste);
                    }
                    lector.Close();

                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                #endregion

                //Accion segun si existe ya en reparaciones o no
                if (siExiste == 1)//Si existe ya en la tabla reparaciones
                {
                    //Eliminamos la reparacion existente y la volvemos a insertar
                    DeleteReparacion(idCliente, matricula, fecha);
                    InsertReparacion(linea, idCliente, matricula, codServicio, fecha);
                    //Insercion factura sustituta
                    string sql2;
                    sql2 = "INSERT INTO factura (numeroFactura,linea,idCliente,matriCoche,codServicio,fecha,estadoFactura,numeroFacturaAnulada) values ('" + numeroFacturaSustituta + "','" + linea + "'," + idCliente + ",'" + matricula.ToUpper() + "','" + codServicio + "','" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "','VIGENTE','" + numeroFacturaAnulada + "')";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conexion);
                    cmd2.ExecuteNonQuery();                    

                    //Despues realizamos la actualizacion de la antigua factura "numeroFacturaAnulada"
                    ActualizarAFacturaAnulada(numeroFacturaAnulada);

                }
                else if(siExiste==0)//Si no existe en la tabla reparaciones
                {
                    //Lo insertamos en la tabla reparaciones
                    InsertReparacion(linea, idCliente, matricula, codServicio, fecha);

                    //Lo instertamos en factura
                    //Insercion
                    string sql2;
                    sql2 = "INSERT INTO factura (numeroFactura,linea,idCliente,matriCoche,codServicio,fecha,estadoFactura,numeroFacturaAnulada) values ('" + numeroFacturaSustituta + "','" + linea + "'," + idCliente + ",'" + matricula.ToUpper() + "','" + codServicio + "','" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "','VIGENTE','" + numeroFacturaAnulada + "')";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conexion);
                    cmd2.ExecuteNonQuery();

                    //Despues realizamos la actualizacion de la antigua factura "numeroFacturaAnulada"
                    ActualizarAFacturaAnulada(numeroFacturaAnulada);
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

        public bool ActualizarAFacturaAnulada(string numeroFacturaAnulada)
        {
            try
            {
                //Insercion
                string sql;
                sql = "update factura set estadoFactura='ANULADA' where numeroFactura="+numeroFacturaAnulada;
                MySqlCommand cmd = new MySqlCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

        #endregion
        //Facturacion
        //-----------------//

        //Puede que termine siendo redundante al tener la tabla factura
        public double selectServicioPrecio(string descripcion)
        {
            Servicio miservicio = new Servicio();
            string sql = "select precio from servicio where descripcion='" + descripcion + "';";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    miservicio.Precio = double.Parse(lector["precio"].ToString());
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return miservicio.Precio;

        }

        public string selectClienteApellidos(int idCliente)
        {
            string apellidos = string.Empty;
            string sql = "select apellidos from cliente where idCliente='" + idCliente + "';";
            MySqlCommand sqlYconec = new MySqlCommand(sql, conexion);

            MySqlDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    apellidos = lector["apellidos"].ToString();
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return apellidos;

        }
        //-//

 

        //-------------------//


        //Copia de seguridad .output backupTaller.sqlite
        //POR AQUI ->EN PROCESO COPIA DE SEGURIDAD
        public bool CopiaSeguridad()
        {
            try
            {
              
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

    }

}
