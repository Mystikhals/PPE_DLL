using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Data;

namespace MySqlStoredProcedure.Model
{
    class Procedure
    {
        #region ATTRIBUTS

        private string specific_name;
        private List<MySqlParameter> param_list;

        #endregion


        #region CONSTRUCTEURS

        /// <summary>
        /// Initialisation d'une instance de la classe <see cref="Procedure"/>.
        /// </summary>
        /// <param name="name">Nom de la procédure</param>
        public Procedure(string name)
        {
            this.specific_name = name;
        }

        /// <summary>
        /// Initialisation d'une instance de la classe <see cref="Procedure"/> 
        /// et récupération des paramètres par un fichier binaire.
        /// </summary>
        /// <param name="name">Nom de la procédure</param>
        /// <param name="parameters">Fichier binaire permettant la récupération des paramètres</param>
        public Procedure(string name, Byte[] parameters)
        {
            this.specific_name = name;
            this.param_list = this.getProcedureParameters(parameters);
        }

        /// <summary>
        /// Initialisation d'une instance de la classe <see cref="Procedure"/> 
        /// et récupération des paramètres par une liste <see cref="MySqlParameterCollection"/>.
        /// </summary>
        /// <param name="name">Nom de la procédure</param>
        /// <param name="parameters">Liste des paramètres de la procédure</param>
        public Procedure(string name, List<MySqlParameter> parameters)
        {
            this.specific_name = name;
            this.param_list = parameters;
        }

        #endregion

        #region METHODES

        private List<MySqlParameter> getProcedureParameters(Byte[] source)
        {
            List<MySqlParameter> Parameters = new List<MySqlParameter>();
            
            string stringParameters = System.Text.Encoding.UTF8.GetString(source);
            string[] param = stringParameters.Split(new char[] {','});

            foreach (string para in param)
            {
                Console.WriteLine(para);
            }

            return Parameters;
        }

        private MySqlParameter buildParameter(string stringParameter)
        {
            MySqlParameter parameter = new MySqlParameter();

            string[] decompose = stringParameter.Split(new char[] {' '});

            int i = 0;
            foreach (string arg in decompose)
            {
                if (arg=="IN" | arg=="OUT" | arg == "INOUT")
                {
                    parameter.Direction = this.getParameterDirection(arg);
                    continue;
                }
                if (i==0)
                {
                    parameter.ParameterName = arg;
                    i++;
                    continue;
                }
                if (i == 2)
                {
                    string[] typeDecompose = arg.Split(new char[] { '(', ')' });
                    for(int j=0; j < typeDecompose.Length; j++)
                    {
                        if (j == 0)
                        {

                        }
                        else
                        {

                        }
                    }  
                }

            }

            return parameter;
        }

        private ParameterDirection getParameterDirection(string direction)
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

        /// <summary>
        /// Permet la convertion d'un type de donnée sous forme de <see cref="string"/> en <see cref="MySqlDbType"/>
        /// </summary>
        /// <param name="type">Type de donnée sous forme de string</param>
        /// <returns></returns>
        private MySqlDbType getDbTypeByString(string type)
        {
            switch (type.ToUpper())
            {
                case ("VARCHAR"):
                    return MySqlDbType.VarChar;

                case ("INT"):
                    return MySqlDbType.Int16;

                case ("SMALLINT"):
                    return MySqlDbType.Int16;

                case ("DECIMAL"):
                    return MySqlDbType.Int16;

                case ("DATE"):
                    return MySqlDbType.Int16;
            }
            return MySqlDbType.Int16;
        }

        #endregion


    }
}
