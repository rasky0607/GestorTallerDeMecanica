using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Añadido
using System.Data;
using System.Data.SQLite;
using System.Collections;


namespace GestorClientes
{
    class DaoSqlite
    {
        public SQLiteConnection conexion;

        public bool Conectar()
        {
            string db = @".\taller";
            string cadenaConecxion = string.Format("Data Source=" + db + ";Version=3;FailIfMissing=true;foreign keys=true;");//PRobando foreign keys=true;
            try
            {
                conexion = new SQLiteConnection(cadenaConecxion);
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
            string sql = "select idCliente,dni,nombre,apellidos,tlf,matricula,marca,modelo from cliente;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente miCliente = new Cliente();
                    miCliente.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miCliente.Dni = lector["dni"].ToString();
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
            catch(Exception e) {
                throw new Exception(e.Message);
            }
            return lClientes;

        }

        public List<string> selectClienteDni()
        {
            List<string> lClientes = new List<string>();
            string sql = "select dni from cliente;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {                               
                    lClientes.Add(lector["dni"].ToString());              
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lClientes;

        }

        public List<string> selectClienteMatricula(string dni)
        {
            List<string> listMatricula = new List<string>();
            string sql = "select matricula from cliente where dni='"+dni+"';";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente cli = new Cliente();
                    cli.Matricula=lector["matricula"].ToString();
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

        public List<Reparacion> selectReparacion()
        {          
            //select numReparacion,dniCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select numReparacion,dniCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.DniCliente = lector["dniCliente"].ToString();
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
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

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
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

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
            string sql = "select codigo from servicio where descripcion='"+descripcion+"';";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

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

        public int selectNumRepara(string dniCliRepara,string matriculaRepa, string fecha)
        {
            int nreparaciones = -1;
            //select count(*)as cantidad from reparacion where dniCliente='12365478C' and matriCoche='2019OPL' and fecha='2019-07-11';
            string sql = "select count(*)as cantidad from reparacion where dniCliente='" + dniCliRepara + "'and matriCoche='"+matriculaRepa+ "'and fecha='" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "'";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

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
            string sql = "SELECT name FROM sqlite_master WHERE type =\"table\" and name !=\"sqlite_sequence\"";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);
            //SELECT name FROM sqlite_master WHERE type = "table" and name !='sqlite_sequence'; <== Esto equivale desde .NET a ==>  .tables
            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    string m = lector["name"].ToString();

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

        public bool InsertCliente(string dni,string nombre,string apellidos, int tlf,string matricula, string marca, string modelo)
        {
            try
            {
                if (dni == " " || dni is null || matricula is null || matricula == " ")
                    throw new Exception("El campo dni y el campo matricula no pueden estar vacios!");
                else
                {
                    //Obtencion  del IdCliente autoincremental buscando el max(idcleinte) o el id mas alto  y sumandole 1 para el siguiente cliente
                    #region obtencion
                    string sql1;
                    sql1 = "select max(idCliente)as maximoid from cliente";
                    SQLiteCommand cmd1 = new SQLiteCommand(sql1, conexion);
                    int idCliente=0;
                    SQLiteDataReader lector = null;
                  
                    try
                    {
                        lector = cmd1.ExecuteReader();
                        while (lector.Read())
                        {
                           idCliente = int.Parse(lector["maximoid"].ToString());
                        }
                        lector.Close();
                        idCliente ++;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    #endregion
                    //Insercion
                    string sql;
                    sql = "INSERT INTO cliente (idCliente,dni,nombre,apellidos,tlf,matricula,marca,modelo) values ("+idCliente+",'" + dni + "','" + nombre + "','" + apellidos + "'," + tlf + ",'" + matricula + "','" + marca + "','" + modelo + "');";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }        
            return true;

        }
   
        public bool InsertServicio(string descripcion, double precio)
        {
            try
            {
                if (descripcion is null || descripcion== " " || precio <0)
                    throw new Exception();
                string sql;
                sql = "INSERT INTO servicio (descripcion,precio) values ('" + descripcion + "'," + precio + ")";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            return true;

        }

        public bool InsertReparacion(int idReparacion, string dniCliente,string matriculaCoche, int codServicio,string fecha)
        {
            try
            {
                if (idReparacion <= 0 || dniCliente  is null || matriculaCoche is null || codServicio < 0 || fecha == null)
                    throw new Exception();
                string sql;            
                sql = "INSERT INTO reparacion (numReparacion,dniCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + dniCliente + "','"+matriculaCoche+ "',"+codServicio+ ",'"+fecha+"')";
               
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

        public bool UpdateCliente(int idCliente,string dni, string nombre, string apellidos, int tlf, string matricula, string marca, string modelo)
        {
            try
            {
                if (dni == " " || dni is null || matricula is null || matricula == " ")
                    throw new Exception("El campo dni y el campo matricula no pueden estar vacios!");
                else
                {
                    //update cliente set nombre='Fran',apellidos='Lanzat' where idCliente=1;
                    //Actualizacion
                    string sql;
                    sql = "update cliente set dni='"+dni+"',nombre='"+nombre+"',apellidos='"+apellidos+"',tlf="+tlf+",matricula='"+matricula+"',marca='"+marca+"',modelo='"+modelo+"' where idCliente="+idCliente+"";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

        public bool UpdateServicio(int codiServicio,string descripcion, double precio)
        {
            try
            {
                if (descripcion == " " || descripcion is null)
                    throw new Exception("El campo descripcion no puede estar vacio!");
                else
                {
                  
                    //Actualizacion
                    string sql;
                   sql = "update servicio set descripcion='"+descripcion+"',precio="+precio+" where codigo="+codiServicio+"";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
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
        public bool DeleteCliente(string dni,string matricula)
        {
            //delete from cliente where dni='12345678A' and matricula='2218CL';
            try
            {                              
                    string sql;
                    sql = "delete from cliente where dni='"+dni+"' and matricula='"+matricula+"'";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
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
            //delete from cliente where dni='12345678A' and matricula='2218CL';
            try
            {
                string sql;
                sql = "delete from servicio where codigo=" +codigo + "";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        public bool DeleteReparacion(int numReparacion, string dniCliente, string matriculaCoche, string fecha)
        {
            //delete from cliente where dni='12345678A' and matricula='2218CL';
            //// "INSERT INTO reparacion (numReparacion,dniCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + dniCliente + "','"+matriculaCoche+ "',"+codServicio+ ",'"+DateTime.Parse(fecha).ToShortDateString()+"')"
            try
            {
                string sql;
                sql = "delete from reparacion where numReparacion=" + numReparacion + " and dniCliente='" + dniCliente + "'" + " and matriCoche='" + matriculaCoche+ "' and fecha='" + Convert.ToDateTime(DateTime.Parse(fecha)).ToString("yyyy-MM-dd")+"'";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        //Consultas de filtro:
        //PROBAR AUN
        //Con un DNI selecionado
        public List<Reparacion> selectReparacionFiltroFecha(string dnicliRepa, string fecha)
        {           
            List<Reparacion> lReparacion = new List<Reparacion>();            
            string sql = "select numReparacion,dniCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where dniCliente='"+dnicliRepa+"' and fecha='"+fecha+"'";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.DniCliente = lector["dniCliente"].ToString();
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
        //Sin DNI selecionado
        public List<Reparacion> selectReparacionFiltroFecha(string fecha)
        {
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,dniCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where  fecha='" + fecha + "'";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.DniCliente = lector["dniCliente"].ToString();
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
        //PROBAR AUN
        //Con DNI selecioando
        public List<Reparacion> selectReparacionFiltroFechaMes(string dnicliRepa, string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where dniCliente='12365478C' and strftime('%m','2019-07-10')= strftime('%m',fecha);
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,dniCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where dniCliente='" + dnicliRepa + "' and strftime('%m',fecha)=strftime('%m','" + fecha + "')";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.DniCliente = lector["dniCliente"].ToString();
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
        //Sin DNI selecionado
        public List<Reparacion> selectReparacionFiltroFechaMes(string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where dniCliente='12365478C' and strftime('%m','2019-07-10')= strftime('%m',fecha);
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,dniCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where strftime('%m',fecha)=strftime('%m','" + fecha + "')";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.DniCliente = lector["dniCliente"].ToString();
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

        //PENDIENTE DE MODIFICAR AUN
        public double selectReparacionFiltroCalculoMes(string fecha)
        {
            double total = 0;
            //select round(sum(precio),2)as total from servicio where codigo in(select codServicio from reparacion where strftime('%m','2019-07-10')= strftime('%m',fecha));
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select round(sum(precio),2)as total from servicio where codigo in(select codServicio from reparacion where strftime('%m','"+fecha+"')= strftime('%m',fecha));";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {

                    total = double.Parse(lector["total"].ToString());
                  
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return total;

        }


    }

}
