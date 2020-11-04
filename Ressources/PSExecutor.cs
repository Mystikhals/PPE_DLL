using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Lien vers la dll MySQL
using MySql.Data.MySqlClient;
using System.Data;

namespace devDLL
{
    class MySqlStoredProcedure
    {
        // Attributs
        private MySqlConnection connect;  // Objet CONNEXION
        private MySqlCommand command;      // Objet COMMANDE
        private MySqlDataReader dataReader;


        // Constructeur Sans Paramètres
        public MySqlStoredProcedure()
        {
            try
            {
                // Instancer l'objet connexion
                connect = new MySqlConnection("server=localhost;User Id=root;database=mabd");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (connect != null)
                    connect.Close();
            }
        }

        // Constructeur Avec Paramètres
        public MySqlStoredProcedure(string server, string userId, string database)
        {
            try
            {
                // Instancer l'objet connexion
                connect = new MySqlConnection(string.Format("server={0};User Id={1};database={2}", server, userId, database));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (connect != null)
                    connect.Close();
            }
        }

        
        public void executeProcedure(string PSName, List<MySqlParameter> Parameters)
        {
            try
            {
                this.connect.Open();
                this.command = new MySqlCommand(PSName, this.connect);
                this.command.CommandType = CommandType.StoredProcedure;
                foreach (MySqlParameter parameter in Parameters)
                {
                    this.command.Parameters.Add(parameter);
                }
                this.dataReader = this.command.ExecuteReader();
                int nombrecol = dataReader.FieldCount;

                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader.GetValue(0));
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (connect != null)
                    connect.Close();
            }
        }
        

    }
}
