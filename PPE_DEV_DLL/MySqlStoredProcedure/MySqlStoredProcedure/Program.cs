using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using MySqlStoredProcedure.Data;
using MySqlStoredProcedure.Model;

namespace MySqlSP
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlProcedureExecutor mssp = new MySqlProcedureExecutor("localhost", "root", "sio");

            List<MySqlParameter> parametres = new List<MySqlParameter>();
            MySqlParameter para = new MySqlParameter("@pcomp", MySqlDbType.VarChar, 4);
            para.Value = "AF";
            parametres.Add(para);

            mssp.executeProcedure("ComptePilotes", parametres);
            foreach (Procedure procedure in mssp.getProcedures()) 
            {
                Console.WriteLine("----------------------------");
                Console.WriteLine(procedure.ToString());
            }

            Console.ReadKey();
        }
    }
}
