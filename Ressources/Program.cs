using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDLL;
using MySql.Data.MySqlClient;

namespace devDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlStoredProcedure mssp = new MySqlStoredProcedure("localhost", "root", "mabd");

            List<MySqlParameter> parametres = new List<MySqlParameter>();
            MySqlParameter para = new MySqlParameter("@pcomp", MySqlDbType.VarChar, 4);
            para.Value = "AF";
            //parametres.Add(para);

            mssp.executeProcedure("ComptePilotes", parametres);

            Console.ReadKey();
        }
    }
}
