using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Lien vers la dll MySQL
using MySql.Data.MySqlClient;
using System.Data;
using MySqlStoredProcedure.Model;
using Microsoft.Win32.SafeHandles;

namespace MySqlStoredProcedure.Data
{
    class MySqlProcedureExecutor
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
        public MySqlProcedureExecutor(string database)
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
        public MySqlProcedureExecutor(string server, string userId, string database)
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
        /// <param name="procedureName">Nom de la procédure stockée</param>
        /// <param name="Parameters">Liste des paramètres necessaires à l'appel de la procédure stockée</param>
        public void executeProcedure(string procedureName, List<MySqlParameter> parameters)
        {
            try
            {
                // Ouverture d'une connexion à la base de données
                this.connect.Open();

                // Création et Execution de la requête
                this.command = new MySqlCommand(procedureName, this.connect);
                this.command.CommandType = CommandType.StoredProcedure;

                foreach (MySqlParameter parameter in parameters)
                {
                    this.command.Parameters.Add(parameter);
                }

                this.dataReader = this.command.ExecuteReader();
                int nbCol = dataReader.FieldCount;

                while (dataReader.Read())
                {
                    for(int i = 0; i < nbCol; i++)
                    {
                        Console.WriteLine(dataReader[i]);
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
        }

        /// <summary>
        /// Permet la récupération de plusieurs procédures de la base de données courante.
        /// </summary>
        /// <returns></returns>
        public List<Procedure> getProcedures()
        {
            List<Procedure> lesProcedures = new List<Procedure>();

            try
            {
                connect.Open(); // Ouverture de la connexion

                // 1 - Création et Execution de la requête pour la récupération des procédures
                string selectProcedure = string.Format(
                    "SELECT * FROM mysql.proc WHERE type = 'PROCEDURE' AND db = '{0}'",
                    this.connect.Database.ToString()
                );

                this.command = new MySqlCommand(selectProcedure, this.connect); // 
                this.dataReader = this.command.ExecuteReader();

                while (dataReader.Read())
                {
                    lesProcedures.Add(new Procedure((string)dataReader["specific_name"], (Byte[])dataReader["param_list"]));
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

        #endregion
    }
}
