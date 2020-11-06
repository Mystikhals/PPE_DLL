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
                if (para.Length != 0)
                {
                    Parameters.Add(this.buildParameter(para));
                }
            }

            return Parameters;
        }

        /// <summary>
        /// Contruit un objet <see cref="MySqlParameter"/> à partir d'un objet <see cref="string"/> 
        /// de la forme <code>[IN | OUT | INOUT] [Nom du paramètre]([Taille])</code>
        /// </summary>
        /// <param name="stringParameter"></param>
        /// <returns></returns>
        private MySqlParameter buildParameter(string stringParameter)
        {
            MySqlParameter parameter = new MySqlParameter(); // Le paramètre à retourner

            List<string> decompose = new List<string>(stringParameter.Split(new char[] {' '})); // On décompe le paramètre en mots clefs

            // Anti caractère espace
            if (decompose.Count == 4)
            {
                decompose.RemoveAt(0);
            }

            int i = 0;
            foreach (string keyWord in decompose)
            {
                if (keyWord == "IN" | keyWord == "OUT" | keyWord == "INOUT")
                {
                    parameter.Direction = this.getParameterDirection(keyWord);
                    continue;
                }
                if (i == 0)
                {
                    parameter.ParameterName = keyWord;
                    i++;
                    continue;
                }
                if (i == 1)
                {
                    string[] typeDecompose = keyWord.Split(new char[] { '(', ')' });
                    int k = 0;
                    foreach (string typeArg in typeDecompose)
                    {
                        if (k == 0)
                        {
                            parameter.MySqlDbType = this.getDbTypeByString(typeDecompose[k]);
                            k++;
                        }
                        else
                        {
                            if (int.Parse(typeDecompose[k]) != 0)
                                parameter.Size = int.Parse(typeDecompose[k]);
                            break;
                        }
                    }
                    i++;
                }
            }

            return parameter;
        }


        /// <summary>
        /// Retourne un objet <see cref="ParameterDirection"/> à partir d'un <see cref="string"/> 
        /// </summary>
        /// <param name="direction">Direction du paramètre en <see cref="string"/></param>
        /// <returns></returns>
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

                // String Types
                case ("CHAR"): return MySqlDbType.VarChar;
                case ("VARCHAR"): return MySqlDbType.VarChar;
                case ("BINARY"): return MySqlDbType.Binary;
                case ("VARBINARY"): return MySqlDbType.VarBinary;
                case ("BLOB"):  return MySqlDbType.Blob;
                case ("TEXT"):  return MySqlDbType.Text;
                case ("ENUM"):  return MySqlDbType.Enum;
                case ("SET"):  return MySqlDbType.Set;

                // Numeric Types
                case ("TINYINT"):  return MySqlDbType.Byte;
                case ("SMALLINT"):  return  MySqlDbType.Int16;
                case ("MEDIUMINT"): return MySqlDbType.Int24;
                case ("INTEGER"): return MySqlDbType.Int32;
                case ("INT"): return MySqlDbType.Int32;
                case ("BIGINT"): return MySqlDbType.Int64;
                case ("DECIMAL"):  return MySqlDbType.Decimal;

                // Time Types
                case ("DATE"): return MySqlDbType.Date;
                case ("TIME"):  return MySqlDbType.Time;
                case ("DATETIME"):  return MySqlDbType.DateTime;
                case ("TIMESTAMP"):  return MySqlDbType.Timestamp;
                case ("YEAR"):  return MySqlDbType.Year;

                // JSON Type
                case ("JSON"): return MySqlDbType.JSON;
            }
            return MySqlDbType.Binary;
        }

        public override string ToString()
        {
            string aRetourner = "";
            aRetourner += this.specific_name;
            aRetourner += "\nParamètres : ";
            foreach (MySqlParameter parameter in this.param_list)
            {
                if (parameter.Size != 0)
                    aRetourner += "\n\t" + parameter.Direction + " " + parameter.ParameterName + " " + parameter.MySqlDbType.ToString() + "(" + parameter.Size + ")";
                else
                    aRetourner += "\n\t" + parameter.Direction + " " + parameter.ParameterName + " " + parameter.MySqlDbType.ToString();
            }

            return aRetourner;
        }

        #endregion


    }
}
