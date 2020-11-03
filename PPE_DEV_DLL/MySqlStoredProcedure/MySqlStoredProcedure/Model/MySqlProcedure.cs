using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Lien vers la dll MySQL
using MySql.Data.MySqlClient;
using System.Data;

namespace MySqlStoredProcedure.Data
{
    class MySqlProcedure
    {
        #region ATTRIBUTS

        private MySqlConnection connect;    // Objet CONNEXION
        private MySqlCommand command;       // Objet COMMANDE
        private MySqlDataReader dataReader; // Objet READER

        #endregion

        #region CONSTRUCTEURS

        /// <summary>
        /// Création d'une connexion à un serveur de base données "localhost" avec l'utilisateur "root" par défaut.
        /// </summary>
        /// <param name="database">Nom de la base de données à utiliser sur le serveur courant</param>
        public MySqlProcedure(string database)
        {
            try
            {
                connect = new MySqlConnection(string.Format("server=localhost;User Id=root;database={0}", database));
                
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

        /// <summary>
        /// Création d'une connexion à un serveur de base données.
        /// </summary>
        /// <param name="server">Adresse du serveur de base de données</param>
        /// <param name="userId">Nom d'utilisateur pour l'accès au serveur de base de données</param>
        /// <param name="database">Nom de la base de données à utiliser sur le serveur courant</param>
        public MySqlProcedure(string server, string userId, string database)
        {
            try
            {
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

        #endregion

        #region METHODES
        
        /// <summary>
        /// Permet l'exécution d'une procédure stockée sur la base de données courante.
        /// </summary>
        /// <param name="PSName">Nom de la procédure stockée</param>
        /// <param name="Parameters">Liste des paramètres necessaires à l'appel de la procédure stockée</param>
        public void executeProcedure(string PSName, List<MySqlParameter> Parameters)
        {
            try
            {
                // Ouverture d'une connexion à la base de données
                this.connect.Open();

                // Création et Execution de la requête
                this.command = new MySqlCommand(PSName, this.connect);
                this.command.CommandType = CommandType.StoredProcedure;

                foreach (MySqlParameter parameter in Parameters)
                {
                    this.command.Parameters.Add(parameter);
                }

                this.dataReader = this.command.ExecuteReader();

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

        /// <summary>
        /// Permet la récupération de plusieurs procédures de la base de données courante.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<MySqlParameter>> getProcedures()
        {
            Dictionary<string, List<MySqlParameter>> lesProcedures = new Dictionary<string, List<MySqlParameter>>();

            try
            {
                // Ouverture d'une connexion à la base de données
                connect.Open();

                // 1 - Création et Execution de la requête pour la récupération des procédures
                string selectProcedure = "SELECT * FROM information_schema.routines WHERE routine_type = 'PROCEDURE'";
                this.command = new MySqlCommand(selectProcedure, this.connect);
                this.dataReader = this.command.ExecuteReader();

                while (dataReader.Read())
                {
                    lesProcedures.Add(dataReader["SPECIFIC_NAME"].ToString(), new List<MySqlParameter>());
                }

                this.dataReader.Close();

                // 2 - Création et Execution de la requête pour la récupération des paramètres de chaque procédure
                string selectParameters = "SELECT * FROM information_schema.parameters";
                this.command = new MySqlCommand(selectParameters, this.connect);
                this.dataReader = this.command.ExecuteReader();

                while (dataReader.Read())
                {
                    foreach (KeyValuePair<string, List<MySqlParameter>> couple in lesProcedures)
                    {
                        if (dataReader["SPECIFIC_NAME"].Equals(couple.Key))
                        {
                            MySqlParameter parameter = new MySqlParameter();
                            parameter.ParameterName = dataReader["PARAMETER_NAME"].ToString();
                            couple.Value.Add()
                        }
                    }
                }

                this.dataReader.Close();
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

            return lesProcedures;
        }


        /// <summary>
        /// Vérifie l'existence d'une table sur la base de données courante.
        /// </summary>
        /// <param name="name">Nom de la table à vérifier.</param>
        /// <returns>grgerg</returns>
        private bool existTable(string name)
        {
            try
            {
                // Ouverture d'une connexion à la base de données
                this.connect.Open();
                string req = string.Format("SHOW TABLES FROM {0}", this.connect.Database);
                this.command = new MySqlCommand(req, this.connect);
                this.dataReader = this.command.ExecuteReader();

                while (dataReader.Read())
                {
                    string columnName = string.Format("Tables_in_{0}", this.connect.Database);
                    if (name.Equals(dataReader[columnName]))
                    {
                        return true;
                    }
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
            return false;
        }

        private MySqlDbType getDbTypeByString(string type)
        {
            switch (type)
            {
                case ("varchar"):
                    return MySqlDbType.VarChar;

                case ("int"):
                    return MySqlDbType.Int16;
            }
            return MySqlDbType.Int16;
        }

        private ParameterDirection getParameterDirectionByString(string direction)
        {
            switch (direction)
            {
                case ("IN"):
                    return ParameterDirection.Input;
                case ("OUT"):
                    return ParameterDirection.Output;
                case ("INOUT"):
                    return ParameterDirection.InputOutput;
            }
            return ParameterDirection.Input;
        }

        #endregion
    }
}
