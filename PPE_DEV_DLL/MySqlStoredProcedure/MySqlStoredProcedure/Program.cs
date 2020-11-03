using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using MySqlStoredProcedure.Data;

namespace MySqlSP
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlProcedure mssp = new MySqlProcedure("localhost", "root", "compagnie");

            List<MySqlParameter> parametres = new List<MySqlParameter>();
            MySqlParameter para = new MySqlParameter("@pcomp", MySqlDbType.VarChar, 4);
            para.Value = "AF";
            //parametres.Add(para);

            mssp.executeProcedure("ComptePilotes", parametres);
            mssp.getProcedures();

            Console.ReadKey();
        }
    }
}
