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
        public DaoSqlite()
        {
            Conectar();
        }
       

        private bool Conectar()
        {
            try
            {
                string db = "mecanicasqlite";
                string cadenaConecxion = string.Format("Data Source=" + db + ";Version=3;FailIfMissing=true;");
                conexion = new SQLiteConnection(cadenaConecxion);
                conexion.Open();
                return true;
            }
            catch {
                return false;
            }
        }

        public bool EstadoConexion()
        {
            if (conexion != null && conexion.State == ConnectionState.Open)
                return true;
            else
                return false;
        }

        public bool Desconectar()
        {
            try
            {
                if (conexion != null)
                {
                    conexion.Close();
                    return true;
                }
                return false;
            }
            catch
            {           
                return false;
            }
        }

        

    }
}
